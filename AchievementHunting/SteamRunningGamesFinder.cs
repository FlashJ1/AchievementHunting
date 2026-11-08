
using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AchievementHunting
{
    public static class SteamRunningGamesFinder
    {
        public static Dictionary<string, string> FindRunningGames()
        {
            Dictionary<string, string> runningGames = new();
            string? steamPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null) as string;
            if (string.IsNullOrWhiteSpace(steamPath))
                return runningGames;
            List<string> libraryPaths = new() { steamPath };
            string libraryFile = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
            if (File.Exists(libraryFile))
            {
                string content = File.ReadAllText(libraryFile);
                foreach (Match match in Regex.Matches(content, @"""path""\s*""([^""]+)"""))
                {
                    string path = match.Groups[1].Value.Replace(@"\\", @"\");
                    if (Directory.Exists(path) && !libraryPaths.Contains(path, StringComparer.OrdinalIgnoreCase))
                        libraryPaths.Add(path);
                }
            }
            Dictionary<string, string> installFolders = new();
            foreach (string library in libraryPaths)
            {
                string steamApps = Path.Combine(library, "steamapps");
                if (!Directory.Exists(steamApps))
                    continue;
                foreach (string manifest in Directory.GetFiles(steamApps, "appmanifest_*.acf"))
                {
                    string content = File.ReadAllText(manifest);
                    Match appIdMatch = Regex.Match(content, @"""appid""\s*""(\d+)""");
                    Match installDirMatch = Regex.Match(content, @"""installdir""\s*""([^""]+)""");
                    if (!appIdMatch.Success || !installDirMatch.Success)
                        continue;
                    string appId = appIdMatch.Groups[1].Value;
                    string installDir = Path.Combine(steamApps, "common", installDirMatch.Groups[1].Value);
                    if (Directory.Exists(installDir))
                        installFolders[appId] = installDir;
                }
            }

            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    string? exePath = process.MainModule?.FileName;
                    if (string.IsNullOrWhiteSpace(exePath))
                        continue;
                    foreach (var game in installFolders)
                    {
                        string folder = game.Value.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                        if (exePath.StartsWith(folder, StringComparison.OrdinalIgnoreCase))
                        {
                            runningGames[game.Key] = game.Value;
                            break;
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                    process.Dispose();
                }
            }
            return runningGames;
        }
    }
}
