using AchievementHunting.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AchievementHunting
{
    public class GetGamesData
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string APIKey = "1CF2DA3D10A0FA93C7FFB0D50D5A3E40";
        public List<Game> Games { get; private set; } = new List<Game>();
        List<Game> randGames = new List<Game>();
        List<Game> gamesToSave = new List<Game>();
        User user = new User();
        Game? bestGame = null;
        Game? randGame = null;
        Game? selectedGame = null;

        private string steamID;

        public GetGamesData(string steamID)
        {
            this.steamID = steamID;
        }

        public async Task GetGamesDataFromSteam()
        {
            Games.Clear();
            randGames.Clear();
            bestGame = null;
            randGame = null;
            selectedGame = null;
            await GetOwnedGamesAsync();
            await GetPlayerSummariesAsync();
            await Task.WhenAll(LoadGameIconsAsync(), LoadGameAchievementsAsync());
            await LoadAchievementDetailsAsync();
            await LoadAchievementIconsAsync();
            gamesToSave = Games.Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements && g.Achievements.Any(a => !a.Achieved)).Select(g => new Game { Name = g.Name, ID = g.ID, ImgIconUrl = g.ImgIconUrl, HasAchievements = g.HasAchievements, TotalAchievements = g.TotalAchievements, UnlockedAchievements = g.UnlockedAchievements, Percent = g.Percent, ImgIcon = g.ImgIcon, Achievements = g.Achievements.Where(a => !a.Achieved).ToList() }).ToList();
            SaveData.Save(gamesToSave);
            SaveData.Save(user);
        }

        public string GetSteamID()
        {
            if (string.IsNullOrWhiteSpace(steamID))
            {
                MessageBox.Show("Please Enter your SteamID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return steamID;
        }

        public async Task<List<Game>> GetOwnedGamesAsync()
        {
            string steamID = GetSteamID();
            string url = $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={APIKey}&steamid={steamID}&include_appinfo=true";
            string json = await _httpClient.GetStringAsync(url);
            JsonDocument doc = JsonDocument.Parse(json);
            foreach (JsonElement game in doc.RootElement.GetProperty("response").GetProperty("games").EnumerateArray())
            {
                Games.Add(new Game
                {
                    Name = game.GetProperty("name").GetString(),
                    ID = game.GetProperty("appid").GetInt32().ToString(),
                    ImgIconUrl = $"https://media.steampowered.com/steamcommunity/public/images/apps/{game.GetProperty("appid").GetInt32()}/{game.GetProperty("img_icon_url").GetString()}.jpg",
                });
            }
            return Games;
        }

        public async Task GetPlayerSummariesAsync()
        {
            string steamID = GetSteamID();
            string url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={APIKey}&steamids={steamID}";
            string json = await _httpClient.GetStringAsync(url);
            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement player = doc.RootElement.GetProperty("response").GetProperty("players")[0];
            user.SteamID = player.GetProperty("steamid").GetString();
            user.Nickname = player.GetProperty("personaname").GetString();
            user.ProfileImageUrl = player.GetProperty("avatarmedium").GetString();
        }

        public async Task LoadGameIconsAsync()
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(10);
            var tasks = Games.Select(async game =>
            {
                await semaphore.WaitAsync();
                try
                {
                    byte[] bytes = await _httpClient.GetByteArrayAsync(game.ImgIconUrl);
                    using MemoryStream ms = new MemoryStream(bytes);
                    using Image temp = Image.FromStream(ms);
                    game.ImgIcon = new Bitmap(temp);
                }
                catch
                {
                    game.ImgIcon = null;
                }
                finally
                {
                    semaphore.Release();
                }
            });
            await Task.WhenAll(tasks);
        }

        public async Task LoadGameAchievementsAsync()
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(10);
            var tasks = Games.Select(async game =>
            {
                await semaphore.WaitAsync();
                try
                {
                    List<GameAchievement> achievements = await LoadAchievementSchemaAsync(game.ID);
                    if (achievements.Count == 0)
                    {
                        game.HasAchievements = false;
                        return;
                    }
                    bool success = await LoadPlayerAchievementStatusAsync(game.ID, achievements);
                    if (!success)
                    {
                        game.HasAchievements = false;
                        return;
                    }
                    game.Achievements = achievements;
                    game.TotalAchievements = achievements.Count;
                    game.UnlockedAchievements = achievements.Count(a => a.Achieved);
                    game.HasAchievements = game.TotalAchievements > 0;
                    game.Percent = (int)(game.UnlockedAchievements * 100.0 / game.TotalAchievements);
                }
                finally
                {
                    semaphore.Release();
                }
            });
            await Task.WhenAll(tasks);
            randGames = Games.Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements).ToList();
            bestGame = randGames.OrderByDescending(g => g.Percent).FirstOrDefault();
        }

        public async Task<List<GameAchievement>> LoadAchievementSchemaAsync(string appID)
        {
            string url = $"https://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v2/?key={APIKey}&appid={appID}&l=ukrainian";
            List<GameAchievement> achievements = new List<GameAchievement>();
            try
            {
                string json = await _httpClient.GetStringAsync(url);
                using JsonDocument doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("game", out JsonElement game)) return achievements;
                if (!game.TryGetProperty("availableGameStats", out JsonElement stats)) return achievements;
                if (!stats.TryGetProperty("achievements", out JsonElement schemaAchievements)) return achievements;
                foreach (JsonElement schema in schemaAchievements.EnumerateArray())
                {
                    string apiName = schema.TryGetProperty("name", out JsonElement nameElement) ? nameElement.GetString() ?? "" : "";
                    string displayName = schema.TryGetProperty("displayName", out JsonElement displayNameElement) ? displayNameElement.GetString() ?? "" : "";
                    string description = schema.TryGetProperty("description", out JsonElement descriptionElement) ? descriptionElement.GetString() ?? "" : "";
                    string icon = schema.TryGetProperty("icon", out JsonElement iconElement) ? iconElement.GetString() ?? "" : "";
                    string iconGray = schema.TryGetProperty("icongray", out JsonElement grayElement) ? grayElement.GetString() ?? "" : "";
                    int hidden = schema.TryGetProperty("hidden", out JsonElement hiddenElement) ? hiddenElement.GetInt32() : 0;
                    achievements.Add(new GameAchievement
                    {
                        APIName = apiName,
                        Name = displayName,
                        Desc = description,
                        IconURL = icon,
                        IconGrayURL = iconGray,
                        Hidden = hidden == 1
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Schema error for {appID}: {ex.Message}");
            }
            return achievements;
        }

        public async Task<bool> LoadPlayerAchievementStatusAsync(string appID, List<GameAchievement> achievements)
        {
            string steamID = GetSteamID();
            string url = $"https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key={APIKey}&steamid={steamID}&appid={appID}";
            try
            {
                string json = await _httpClient.GetStringAsync(url);
                using JsonDocument doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("playerstats", out JsonElement playerStats)) return false;
                if (!playerStats.TryGetProperty("success", out JsonElement successElement)) return false;
                if (!successElement.GetBoolean()) return false;
                if (!playerStats.TryGetProperty("achievements", out JsonElement playerAchievements)) return false;
                foreach (JsonElement playerAchievement in playerAchievements.EnumerateArray())
                {
                    string apiName = playerAchievement.GetProperty("apiname").GetString() ?? "";
                    int achieved = playerAchievement.GetProperty("achieved").GetInt32();
                    GameAchievement? achievement = achievements.FirstOrDefault(a => a.APIName == apiName);
                    if (achievement != null) achievement.Achieved = achieved == 1;
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Player achievements error {appID}: {ex.Message}");
                return false;
            }
        }

        public async Task LoadAchievementDetailsAsync()
        {
            var gamesWithAchievements = Games.Where(g => g.HasAchievements).ToList();
            foreach (Game game in gamesWithAchievements)
            {
                await LoadGlobalAchievementPercentagesAsync(game.ID, game.Achievements);
            }
        }

        public async Task LoadGlobalAchievementPercentagesAsync(string appID, List<GameAchievement> achievements)
        {
            string url = $"https://api.steampowered.com/ISteamUserStats/GetGlobalAchievementPercentagesForApp/v2/?gameid={appID}";
            try
            {
                string json = await _httpClient.GetStringAsync(url);
                using JsonDocument doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty("achievementpercentages", out JsonElement percentages)) return;
                if (!percentages.TryGetProperty("achievements", out JsonElement globalAchievements)) return;
                foreach (JsonElement global in globalAchievements.EnumerateArray())
                {
                    string apiName = global.GetProperty("name").GetString() ?? "";
                    string percent = global.GetProperty("percent").GetString() ?? "";
                    GameAchievement? achievement = achievements.FirstOrDefault(a => a.APIName == apiName);
                    if (achievement != null) achievement.GlobalPercent = percent;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Global achievement error for {appID}: {ex.Message}");
            }
        }

        public async Task LoadAchievementIconsAsync()
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(10);
            var achievements = Games.Where(g => g.HasAchievements).SelectMany(g => g.Achievements);
            var tasks = achievements.Select(async achievement =>
            {
                await semaphore.WaitAsync();
                try
                {
                    string url = achievement.IconURL;
                    byte[] bytes = await _httpClient.GetByteArrayAsync(url);
                    using MemoryStream ms = new MemoryStream(bytes);
                    using Image temp = Image.FromStream(ms);
                    achievement.Icon = new Bitmap(temp);
                }
                catch (Exception)
                {
                    achievement.Icon = null;
                }
                finally
                {
                    semaphore.Release();
                }
            });
            await Task.WhenAll(tasks);
        }
    }
}
