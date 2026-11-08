using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AchievementHunting
{
    public sealed class UnlockedAchievementInfo
    {
        public string APIName { get; }
        public DateTime UnlockedAt { get; }
        public UnlockedAchievementInfo(string apiName, DateTime unlockedAt)
        {
            APIName = apiName;
            UnlockedAt = unlockedAt;
        }
    }
    public class AchievementUnlockedEventArgs : EventArgs
    {
        public string AppID { get; }
        public List<UnlockedAchievementInfo> NewAchievements { get; }
        public AchievementUnlockedEventArgs(string appID, List<UnlockedAchievementInfo> newAchievements)
        {
            AppID = appID;
            NewAchievements = newAchievements;
        }
    }
    public class AchievementMonitor : IDisposable
    {
        private static readonly Regex StatsFileRegex = new(@"^UserGameStats_(\d+)_(\d+)\.bin$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private const long SteamId64Base = 76561197960265728L;
        private const int DebounceMs = 60;
        private const int ReadRetries = 10;
        private const int ReadRetryDelayMs = 50;
        private const long RecentWindowSeconds = 120;
        private readonly GetGamesData ggd;
        private readonly object sync = new();
        private readonly Dictionary<string, HashSet<(int Stat, int Bit)>> knownUnlocked = new();
        private readonly Dictionary<string, Dictionary<(int Stat, int Bit), string>> schemaCache = new();
        private readonly Dictionary<string, CancellationTokenSource> pending = new();
        private FileSystemWatcher? watcher;
        private string? statsFolder;
        private long? accountId;
        private SynchronizationContext? uiContext;
        private bool running;
        private bool disposed;
        public event EventHandler<AchievementUnlockedEventArgs>? AchievementsUnlocked;
        public AchievementMonitor(GetGamesData ggd)
        {
            this.ggd = ggd;
        }
        public bool Start()
        {
            if (disposed)
                return false;
            if (running)
                return true;
            statsFolder = FindStatsFolder();
            if (statsFolder == null)
            {
                Debug.WriteLine("AchievementMonitor: Steam\\appcache\\stats not found.");
                return false;
            }
            accountId = GetAccountId();
            uiContext = SynchronizationContext.Current;
            lock (sync)
            {
                knownUnlocked.Clear();
                foreach (string file in Directory.EnumerateFiles(statsFolder, "UserGameStats_*_*.bin"))
                {
                    if (!TryGetAppId(Path.GetFileName(file), out string appId))
                        continue;
                    var snapshot = SteamStatsReader.ReadUnlocked(file);
                    if (snapshot != null)
                        knownUnlocked[appId] = new HashSet<(int Stat, int Bit)>(snapshot.Keys);
                }
            }
            watcher = new FileSystemWatcher(statsFolder, "UserGameStats_*_*.bin")
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName | NotifyFilters.CreationTime,
                InternalBufferSize = 64 * 1024
            };
            watcher.Changed += (_, e) => OnStatsFileEvent(e.Name);
            watcher.Created += (_, e) => OnStatsFileEvent(e.Name);
            watcher.Renamed += (_, e) => OnStatsFileEvent(e.Name);
            watcher.Error += (_, e) => Debug.WriteLine($"AchievementMonitor watcher error: {e.GetException().Message}");
            watcher.EnableRaisingEvents = true;
            running = true;
            Debug.WriteLine($"AchievementMonitor: watching {statsFolder} (account {accountId?.ToString() ?? "any"}, {knownUnlocked.Count} games in baseline)");
            return true;
        }
        public void Stop()
        {
            running = false;
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                watcher = null;
            }
            lock (sync)
            {
                foreach (CancellationTokenSource cts in pending.Values) cts.Cancel();
                pending.Clear();
                knownUnlocked.Clear();
            }
        }

        private void OnStatsFileEvent(string? fileName)
        {
            if (!running || string.IsNullOrEmpty(fileName) || statsFolder == null)
                return;
            if (!TryGetAppId(fileName, out string appId))
                return;
            var cts = new CancellationTokenSource();
            lock (sync)
            {
                if (pending.TryGetValue(appId, out CancellationTokenSource? previous)) previous.Cancel();
                pending[appId] = cts;
            }
            _ = ProcessAsync(appId, Path.Combine(statsFolder, fileName), cts);
        }

        private async Task ProcessAsync(string appId, string file, CancellationTokenSource cts)
        {
            try
            {
                await Task.Delay(DebounceMs, cts.Token);
                Dictionary<(int Stat, int Bit), long>? current = null;
                for (int attempt = 0; attempt < ReadRetries; attempt++)
                {
                    current = SteamStatsReader.ReadUnlocked(file);
                    if (current != null) break;
                    await Task.Delay(ReadRetryDelayMs, cts.Token);
                }
                if (current == null)
                    return;
                var fresh = new List<(int Stat, int Bit, long Time)>();
                lock (sync)
                {
                    bool hadSnapshot = knownUnlocked.TryGetValue(appId, out HashSet<(int Stat, int Bit)>? known);
                    long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    foreach (KeyValuePair<(int Stat, int Bit), long> kv in current)
                    {
                        bool isNew = hadSnapshot ? !known!.Contains(kv.Key) : now - kv.Value <= RecentWindowSeconds;
                        if (isNew)
                            fresh.Add((kv.Key.Stat, kv.Key.Bit, kv.Value));
                    }
                    knownUnlocked[appId] = new HashSet<(int Stat, int Bit)>(current.Keys);
                }
                if (fresh.Count == 0)
                    return;
                List<UnlockedAchievementInfo> unlocked = ResolveNames(appId, fresh);
                if (unlocked.Count == 0)
                {
                    Debug.WriteLine($"AchievementMonitor [{appId}]: {fresh.Count} new bit(s), but no names in the local schema.");
                    return;
                }
                Debug.WriteLine($"AchievementMonitor [{appId}]: unlocked {string.Join(", ", unlocked.Select(u => u.APIName))}");
                var args = new AchievementUnlockedEventArgs(appId, unlocked);
                if (uiContext != null) uiContext.Post(_ => AchievementsUnlocked?.Invoke(this, args), null);
                else AchievementsUnlocked?.Invoke(this, args);
            }
            catch (OperationCanceledException)
            {
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AchievementMonitor [{appId}]: {ex.Message}");
            }
            finally
            {
                lock (sync)
                {
                    if (pending.TryGetValue(appId, out CancellationTokenSource? latest) && latest == cts)
                        pending.Remove(appId);
                }
            }
        }
        private List<UnlockedAchievementInfo> ResolveNames(string appId, List<(int Stat, int Bit, long Time)> fresh)
        {
            var result = new List<UnlockedAchievementInfo>();
            Dictionary<(int Stat, int Bit), string>? schema = GetSchema(appId, forceReload: false);
            if (schema != null)
            {
                var known = schema;
                if (fresh.Any(x => !known.ContainsKey((x.Stat, x.Bit)))) schema = GetSchema(appId, forceReload: true) ?? schema;
            }
            if (schema == null)
                return result;
            foreach ((int Stat, int Bit, long Time) item in fresh.OrderBy(x => x.Time))
            {
                if (!schema.TryGetValue((item.Stat, item.Bit), out string? name))
                    continue;
                DateTime at = item.Time > 0 ? DateTimeOffset.FromUnixTimeSeconds(item.Time).LocalDateTime : DateTime.Now;
                result.Add(new UnlockedAchievementInfo(name, at));
            }
            return result;
        }
        private Dictionary<(int Stat, int Bit), string>? GetSchema(string appId, bool forceReload)
        {
            lock (sync)
            {
                if (!forceReload && schemaCache.TryGetValue(appId, out var cached))
                    return cached;
            }
            if (statsFolder == null)
                return null;
            var schema = SteamStatsReader.ReadSchema(Path.Combine(statsFolder, $"UserGameStatsSchema_{appId}.bin"));
            if (schema != null)
            {
                lock (sync) { schemaCache[appId] = schema; }
            }
            return schema;
        }
        private bool TryGetAppId(string fileName, out string appId)
        {
            appId = string.Empty;
            Match match = StatsFileRegex.Match(fileName);
            if (!match.Success) return false;
            if (accountId.HasValue && match.Groups[1].Value != accountId.Value.ToString()) return false;
            appId = match.Groups[2].Value;
            return true;
        }
        private long? GetAccountId()
        {
            try
            {
                string? steamId = ggd.GetSteamID();
                if (long.TryParse(steamId, out long id64) && id64 > SteamId64Base)
                    return id64 - SteamId64Base;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AchievementMonitor: cannot determine account id: {ex.Message}");
            }
            return null;
        }
        private static string? FindStatsFolder()
        {
            var candidates = new List<string?>
            {
                Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null) as string,
                Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null) as string,
                @"C:\Program Files (x86)\Steam"
            };
            foreach (string? steamPath in candidates)
            {
                if (string.IsNullOrWhiteSpace(steamPath))
                    continue;
                string folder = Path.Combine(steamPath, "appcache", "stats");
                if (Directory.Exists(folder))
                    return folder;
            }
            return null;
        }
        public void Dispose()
        {
            if (disposed)
                return;
            Stop();
            disposed = true;
            schemaCache.Clear();
        }
    }
}