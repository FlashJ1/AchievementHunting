using AchievementHunting.Models;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace AchievementHunting
{
    public partial class MainMenu : Form
    {
        private readonly Random random = new Random();
        private readonly HttpClient _httpClient = new HttpClient();
        List<Game> games = new List<Game>();
        List<Game> randGames = new List<Game>();
        List<Game> gamesToSave = new List<Game>();
        User user = new User();
        Game? bestGame = null;
        Game? randGame = null;
        Game? selectedGame = null;
        private ListGame? formListGame;
        private AchieveList? formAchieveList;
        private Profile? formProfile;
        private FriendList? formFriendList;
        private readonly GetGamesData? ggd;
        private readonly Dictionary<string, List<GameAchievement>> achievementCache = new();
        private bool isRefreshing = false;
        private bool autoRefreshEnabled = false;
        private AchievementMonitor? achievementMonitor;
        private Task? libraryLoadTask;
        public MainMenu()
        {
            InitializeComponent();
            user = SaveData.LoadUser();
            if (!string.IsNullOrWhiteSpace(user.SteamID))
                ggd = new GetGamesData(user.SteamID);
            SetupAchievementMonitor();
        }
        public MainMenu(GetGamesData ggd)
        {
            InitializeComponent();
            this.ggd = ggd;
            SetupAchievementMonitor();
        }
        private void SetupAchievementMonitor()
        {
            if (ggd == null)
                return;
            achievementMonitor = new AchievementMonitor(ggd);
            achievementMonitor.AchievementsUnlocked += AchievementMonitor_AchievementsUnlocked;
            FormClosed += (_, _) => achievementMonitor?.Dispose();
        }
        private async void AchievementMonitor_AchievementsUnlocked(object? sender, AchievementUnlockedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => AchievementMonitor_AchievementsUnlocked(sender, e));
                return;
            }
            var affectedGames = games.Concat(ggd?.Games ?? new List<Game>()).Where(g => g.ID == e.AppID).Distinct().ToList();
            if (selectedGame != null && selectedGame.ID == e.AppID && !affectedGames.Contains(selectedGame))
                affectedGames.Add(selectedGame);
            if (affectedGames.Count == 0)
                return;
            var newlyUnlocked = new List<GameAchievement>();
            foreach (var game in affectedGames)
            {
                foreach (var unlocked in e.NewAchievements)
                {
                    var achievement = game.Achievements.FirstOrDefault(a => a.APIName == unlocked.APIName);
                    if (achievement == null || achievement.Achieved)
                        continue;
                    achievement.Achieved = true;
                    achievement.AchievedAt = unlocked.UnlockedAt;
                    if (!newlyUnlocked.Any(a => a.APIName == achievement.APIName)) newlyUnlocked.Add(achievement);
                }
            }
            if (newlyUnlocked.Count == 0)
                return;
            if (ggd != null && newlyUnlocked.Any(a => string.IsNullOrWhiteSpace(a.GlobalPercent)))
                await ggd.LoadGlobalAchievementPercentagesAsync(e.AppID, newlyUnlocked);
            foreach (var achievement in newlyUnlocked)
            {
                SoundPlayer.PlayByRarity(achievement.GlobalPercent);
            }
            foreach (var game in affectedGames)
            {
                game.UnlockedAchievements = Math.Max(0, game.TotalAchievements - game.Achievements.Count(a => !a.Achieved));
                game.Percent = game.TotalAchievements == 0 ? 0 : (int)(game.UnlockedAchievements * 100.0 / game.TotalAchievements);
            }
            var updatedGame = affectedGames.First();
            if (updatedGame.Percent >= 100)
            {
                var lastAchievement = updatedGame.Achievements.Where(a => a.Achieved && a.AchievedAt.HasValue)
                                                              .OrderByDescending(a => a.AchievedAt).FirstOrDefault();
                foreach (var game in affectedGames) game.BeatedAt = lastAchievement?.AchievedAt;
                var beatenGames = SaveData.LoadBeatenGames();
                var beatenGame = beatenGames.FirstOrDefault(g => g.ID == updatedGame.ID);
                if (beatenGame == null)
                {
                    beatenGames.Add(new BeatenGame
                    {
                        ID = updatedGame.ID,
                        BeatedAt = updatedGame.BeatedAt
                    });
                }
                else beatenGame.BeatedAt = updatedGame.BeatedAt;
                SaveData.Save(beatenGames);
                games.RemoveAll(g => g.ID == e.AppID);
                if (selectedGame?.ID == e.AppID)
                {
                    selectedGame = null;
                    lbTitle.Text = "";
                    imageHeader.Image = null;
                    btnLaunchGame.Enabled = false;
                    btnAchieveList.Enabled = false;
                }
                if (formAchieveList != null && !formAchieveList.IsDisposed)
                {
                    formAchieveList.Close();
                    formAchieveList = null;
                }
                if (formProfile != null && !formProfile.IsDisposed)
                    formProfile.RefreshProfile();
            }
            else if (selectedGame?.ID == e.AppID)
            {
                lbTitle.Text = $"{selectedGame.Name} - {selectedGame.UnlockedAchievements}/{selectedGame.TotalAchievements} ({selectedGame.Percent}%)";
                AutoResizeTitleFont();
                if (formAchieveList != null && !formAchieveList.IsDisposed)
                    formAchieveList.RefreshAchievements();
            }
            randGames = games.Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements).ToList();
            bestGame = randGames.OrderByDescending(g => g.Percent).FirstOrDefault();
            gamesToSave = games.Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements && g.Achievements.Any(a => !a.Achieved)).Select(g => new Game
            {
                Name = g.Name,
                ID = g.ID,
                ImgIconUrl = g.ImgIconUrl,
                HasAchievements = g.HasAchievements,
                TotalAchievements = g.TotalAchievements,
                UnlockedAchievements = g.UnlockedAchievements,
                Percent = g.Percent,
                ImgIcon = g.ImgIcon,
                Achievements = g.Achievements
                        .Where(a => !a.Achieved)
                        .ToList()
            }).ToList();
            SaveData.Save(gamesToSave);
        }

        public void LaunchGame(string appID)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = $"steam://rungameid/{appID}",
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Achievement Hunting";
            btnListGames.Enabled = false;
            btnMaxPercentGame.Enabled = false;
            btnRandomGame.Enabled = false;
            btnLaunchGame.Enabled = false;
            btnLaunchGame.Visible = false;
            btnAchieveList.Enabled = false;
            btnAchieveList.Visible = false;
            btnRefresh.Enabled = false;
            btnFriendList.Enabled = false;
            profileImage.Enabled = false;
            imageHeader.Visible = false;
            btnAutoRefresh.Enabled = false;
            games = SaveData.Load();
            if (games.Count > 0)
            {
                if (ggd != null)
                    libraryLoadTask = ggd.GetGamesDataFromSteamWithoutSaving();
                await LoadGameIconsAsync();
                await LoadAchievementIconsAsync();
                await ggd.GetFriendListAsync();
                randGames = games.Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements).ToList();
                bestGame = randGames.OrderByDescending(g => g.Percent).FirstOrDefault();
                btnListGames.Enabled = true;
                btnMaxPercentGame.Enabled = true;
                btnRandomGame.Enabled = true;
                btnRefresh.Enabled = true;
                profileImage.Enabled = true;
                btnAutoRefresh.Enabled = true;
                btnFriendList.Enabled = true;
            }
            profileImage.ImageLocation = user.ProfileImageUrl;
            lbNickname.Text = user.Nickname;
        }
        private void btnMaxPercentGame_Click(object sender, EventArgs e)
        {
            if (bestGame != null)
            {
                if (imageHeader.Visible == false)
                {
                    imageHeader.Visible = true;
                    btnLaunchGame.Visible = true;
                    btnAchieveList.Visible = true;
                }
                selectedGame = bestGame;
                imageHeader.ImageLocation = GetHeaderImageUrl(int.Parse(bestGame.ID));
                btnLaunchGame.Enabled = true;
                lbTitle.Text = $"{selectedGame.Name} - {selectedGame.UnlockedAchievements}/{selectedGame.TotalAchievements} ({selectedGame.Percent}%)";
                AutoResizeTitleFont();
                btnAchieveList.Enabled = true;
            }
        }
        public string GetHeaderImageUrl(int appID)
        {
            return $"https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/{appID}/library_600x900.jpg";
        }
        private void btnLaunchGame_Click(object sender, EventArgs e)
        {
            if (selectedGame != null)
                LaunchGame(selectedGame.ID);
        }
        private void btnRandomGame_Click(object sender, EventArgs e)
        {
            if (imageHeader.Visible == false)
            {
                imageHeader.Visible = true;
                btnLaunchGame.Visible = true;
                btnAchieveList.Visible = true;
            }
            randGame = randGames[random.Next(randGames.Count)];
            selectedGame = randGame;
            imageHeader.ImageLocation = GetHeaderImageUrl(int.Parse(randGame.ID));
            btnLaunchGame.Enabled = true;
            lbTitle.Text = $"{selectedGame.Name} - {selectedGame.UnlockedAchievements}/{selectedGame.TotalAchievements} ({selectedGame.Percent}%)";
            AutoResizeTitleFont();
            btnAchieveList.Enabled = true;
        }
        private void btnListGames_Click(object sender, EventArgs e)
        {
            if (formListGame != null && !formListGame.IsDisposed)
            {
                formListGame.BringToFront();
                return;
            }
            var sortedGames = games.Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements)
                                   .OrderByDescending(g => g.Percent).ToList();
            formListGame = new ListGame(sortedGames);
            formListGame.FormClosed += (s, args) => formListGame = null;
            formListGame.GameSelected += OnGameSelected;
            formListGame.Show();
        }
        private void OnGameSelected(Game game)
        {
            if (imageHeader.Visible == false)
            {
                imageHeader.Visible = true;
                btnLaunchGame.Visible = true;
                btnAchieveList.Visible = true;
            }
            selectedGame = game;
            imageHeader.ImageLocation = GetHeaderImageUrl(int.Parse(game.ID));
            lbTitle.Text = $"{selectedGame.Name} - {selectedGame.UnlockedAchievements}/{selectedGame.TotalAchievements} ({selectedGame.Percent}%)";
            AutoResizeTitleFont();
            btnLaunchGame.Enabled = true;
            btnAchieveList.Enabled = true;
        }
        private void btnAchieveList_Click(object sender, EventArgs e)
        {
            if (selectedGame == null)
                return;
            if (formAchieveList != null && !formAchieveList.IsDisposed)
            {
                formAchieveList.UpdateGame(selectedGame);
                formAchieveList.BringToFront();
                return;
            }
            formAchieveList = new AchieveList(selectedGame);
            formAchieveList.FormClosed += (s, args) => formAchieveList = null;
            formAchieveList.Show();
        }
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            btnRefresh.Enabled = false;
            try
            {
                await SyncNewGamesAsync();
                await RefreshAsync();
            }
            finally
            {
                btnRefresh.Enabled = true;
            }
        }
        public async Task SyncNewGamesAsync()
        {
            if (ggd == null)
                return;
            try
            {
                if (libraryLoadTask != null)
                    await libraryLoadTask;
                await ggd.UpdateSteamGamesAsync();
                var knownIDs = new HashSet<string>(games.Select(g => g.ID));
                var toAdd = ggd.Games
                    .Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements && !knownIDs.Contains(g.ID))
                    .ToList();
                if (toAdd.Count == 0)
                    return;
                var needRarity = toAdd.Where(g => g.Achievements.Any(a => string.IsNullOrWhiteSpace(a.GlobalPercent))).ToList();
                if (needRarity.Count > 0)
                    await ggd.LoadAchievementDetailsAsync(needRarity);
                var needIcons = toAdd.Where(g => g.Achievements.Any(a => a.Icon == null)).ToList();
                if (needIcons.Count > 0)
                    await ggd.LoadAchievementIconsAsync(needIcons);
                games.AddRange(toAdd);
                randGames = games.Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements).ToList();
                bestGame = randGames.OrderByDescending(g => g.Percent).FirstOrDefault();
                gamesToSave = games.Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements && g.Achievements.Any(a => !a.Achieved))
                                   .Select(g => new Game()
                                   {
                                       Name = g.Name,
                                       ID = g.ID,
                                       ImgIconUrl = g.ImgIconUrl,
                                       HasAchievements = g.HasAchievements,
                                       TotalAchievements = g.TotalAchievements,
                                       UnlockedAchievements = g.UnlockedAchievements,
                                       Percent = g.Percent,
                                       ImgIcon = g.ImgIcon,
                                       Achievements = g.Achievements.Where(a => !a.Achieved).ToList()
                                   }).ToList();
                SaveData.Save(gamesToSave);
                if (randGames.Count > 0)
                {
                    btnListGames.Enabled = true;
                    btnMaxPercentGame.Enabled = true;
                    btnRandomGame.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SyncNewGamesAsync: {ex.Message}");
            }
        }
        public async Task RefreshAsync()
        {
            if (selectedGame == null || ggd == null)
                return;
            if (isRefreshing)
                return;
            isRefreshing = true;
            try
            {
                if (!achievementCache.TryGetValue(selectedGame.ID, out List<GameAchievement>? achievements))
                {
                    achievements = await ggd.LoadAchievementSchemaAsync(selectedGame.ID);
                    achievementCache[selectedGame.ID] = achievements;
                }
                var oldStates = selectedGame.Achievements.ToDictionary(a => a.APIName, a => a.Achieved);
                bool success = await ggd.LoadPlayerAchievementStatusAsync(selectedGame.ID, achievements);
                if (!success)
                    return;
                bool hasChanges = achievements.Any(a => oldStates.TryGetValue(a.APIName, out bool wasAchieved) && !wasAchieved && a.Achieved);
                if (!hasChanges)
                    return;
                await ggd.LoadGlobalAchievementPercentagesAsync(selectedGame.ID, achievements);
                foreach (GameAchievement newAchievement in achievements)
                {
                    GameAchievement? oldAchievement = selectedGame.Achievements.FirstOrDefault(a => a.APIName == newAchievement.APIName);
                    if (oldAchievement == null)
                        continue;
                    newAchievement.Icon = oldAchievement.Icon;
                    if (oldStates.TryGetValue(newAchievement.APIName, out bool wasAchieved) && !wasAchieved && newAchievement.Achieved)
                        SoundPlayer.PlayByRarity(newAchievement.GlobalPercent);
                }
                selectedGame.Achievements = achievements;
                selectedGame.TotalAchievements = achievements.Count;
                selectedGame.UnlockedAchievements = achievements.Count(a => a.Achieved);
                selectedGame.Percent = (int)(selectedGame.UnlockedAchievements * 100.0 / selectedGame.TotalAchievements);
                if (selectedGame.Percent >= 100)
                {
                    var lastAchievement = selectedGame.Achievements.Where(a => a.Achieved && a.AchievedAt.HasValue)
                                                                   .OrderByDescending(a => a.AchievedAt).FirstOrDefault();
                    selectedGame.BeatedAt = lastAchievement?.AchievedAt;
                    List<BeatenGame> beatenGames = SaveData.LoadBeatenGames();
                    BeatenGame? existing = beatenGames.FirstOrDefault(g => g.ID == selectedGame.ID);
                    if (existing == null)
                    {
                        beatenGames.Add(new BeatenGame
                        {
                            ID = selectedGame.ID,
                            BeatedAt = selectedGame.BeatedAt
                        });
                    }
                    else
                        existing.BeatedAt = selectedGame.BeatedAt;
                    SaveData.Save(beatenGames);
                    if (ggd != null)
                    {
                        var gameInGgd = ggd.Games.FirstOrDefault(g => g.ID == selectedGame.ID);
                        if (gameInGgd != null)
                        {
                            gameInGgd.BeatedAt = selectedGame.BeatedAt;
                            gameInGgd.Percent = selectedGame.Percent;
                            gameInGgd.UnlockedAchievements = selectedGame.UnlockedAchievements;
                            gameInGgd.TotalAchievements = selectedGame.TotalAchievements;
                            gameInGgd.Achievements = selectedGame.Achievements;
                        }
                    }
                    if (formAchieveList != null && !formAchieveList.IsDisposed)
                    {
                        formAchieveList.Close();
                        formAchieveList = null;
                    }
                    if (formProfile != null && !formProfile.IsDisposed)
                        formProfile.RefreshProfile();
                    games.Remove(selectedGame);
                    selectedGame = null;
                    lbTitle.Text = "";
                    imageHeader.Image = null;
                    btnAchieveList.Enabled = false;
                    btnLaunchGame.Enabled = false;
                }
                else
                {
                    lbTitle.Text = $"{selectedGame.Name} - {selectedGame.UnlockedAchievements}/{selectedGame.TotalAchievements} ({selectedGame.Percent}%)";
                    AutoResizeTitleFont();
                    if (formAchieveList != null && !formAchieveList.IsDisposed)
                        formAchieveList.RefreshAchievements();
                }
                randGames = games.Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements).ToList();
                bestGame = randGames.OrderByDescending(g => g.Percent).FirstOrDefault();
                gamesToSave = games.Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements && g.Achievements.Any(a => !a.Achieved)).Select(g => new Game()
                {
                    Name = g.Name,
                    ID = g.ID,
                    ImgIconUrl = g.ImgIconUrl,
                    HasAchievements = g.HasAchievements,
                    TotalAchievements = g.TotalAchievements,
                    UnlockedAchievements = g.UnlockedAchievements,
                    Percent = g.Percent,
                    ImgIcon = g.ImgIcon,
                    Achievements = g.Achievements.Where(a => !a.Achieved).ToList()
                }).ToList();
                SaveData.Save(gamesToSave);
            }
            finally
            {
                isRefreshing = false;
            }
        }
        public async Task LoadGameIconsAsync()
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(10);
            var tasks = games.Select(async game =>
            {
                await semaphore.WaitAsync();
                try
                {
                    byte[] bytes = await _httpClient.GetByteArrayAsync(game.ImgIconUrl);
                    using MemoryStream ms = new MemoryStream(bytes);
                    game.ImgIcon = Image.FromStream(ms);
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
        public async Task LoadAchievementIconsAsync()
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(10);
            var achievements = games.Where(g => g.HasAchievements).SelectMany(g => g.Achievements);
            var tasks = achievements.Select(async achievement =>
            {
                await semaphore.WaitAsync();
                try
                {
                    string url = achievement.IconURL;
                    byte[] bytes = await _httpClient.GetByteArrayAsync(url);
                    using MemoryStream ms = new MemoryStream(bytes);
                    achievement.Icon = Image.FromStream(ms);
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
        private void AutoResizeTitleFont()
        {
            float maxSize = 18f;
            float minSize = 8f;
            for (float size = maxSize; size >= minSize; size -= 0.5f)
            {
                Font font = new Font(lbTitle.Font.FontFamily, size, lbTitle.Font.Style);
                Size textSize = TextRenderer.MeasureText(lbTitle.Text, font);
                if (textSize.Width <= lbTitle.ClientSize.Width)
                {
                    lbTitle.Font = font;
                    return;
                }
            }
            lbTitle.Font = new Font(lbTitle.Font.FontFamily, minSize, lbTitle.Font.Style);
        }
        private void profileImage_Click(object sender, EventArgs e)
        {
            if (formProfile != null && !formProfile.IsDisposed)
            {
                formProfile.BringToFront();
                return;
            }
            var gamesToPass = (ggd != null && ggd.Games.Count > 0) ? ggd.Games : games;
            formProfile = new Profile(gamesToPass);
            formProfile.FormClosed += (s, args) => formProfile = null;
            formProfile.Show();
        }
        private void btnAutoRefresh_Click(object sender, EventArgs e)
        {
            if (!autoRefreshEnabled)
            {
                if (achievementMonitor == null || !achievementMonitor.Start())
                {
                    MessageBox.Show("Не вдалося знайти папку Steam\\appcache\\stats. Auto Refresh недоступний.");
                    return;
                }
                autoRefreshEnabled = true;
                btnAutoRefresh.Text = "Auto Refresh: ON";
            }
            else
            {
                achievementMonitor?.Stop();
                autoRefreshEnabled = false;
                btnAutoRefresh.Text = "Auto Refresh: OFF";
            }
        }

        private void btnFriendList_Click(object sender, EventArgs e)
        {
            if (ggd == null)
                return;
            if (formFriendList != null && !formFriendList.IsDisposed)
            {
                formFriendList.BringToFront();
                return;
            }
             formFriendList = new FriendList(ggd.Friends);
            formFriendList.FormClosed += (s, args) => formFriendList = null;
            formFriendList.Show();
        }
    }
}