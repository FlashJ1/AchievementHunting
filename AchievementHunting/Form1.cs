using AchievementHunting.Models;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace AchievementHunting
{
    public partial class Form1 : Form
    {
        private readonly Random random = new Random();
        private readonly HttpClient _httpClient = new HttpClient();
        private const string APIKey = "1CF2DA3D10A0FA93C7FFB0D50D5A3E40";
        List<Game> games = new List<Game>();
        List<Game> randGames = new List<Game>();
        List<Game> gamesToSave = new List<Game>();
        User user = new User();
        Game? bestGame = null;
        Game? randGame = null;
        Game? selectedGame = null;
        private Form2? form2;
        private Form3? form3;
        private readonly GetGamesData? ggd;

        public Form1()
        {
            InitializeComponent();
            user = SaveData.LoadUser();

            if (!string.IsNullOrWhiteSpace(user.SteamID)) ggd = new GetGamesData(user.SteamID);
        }

        public Form1(GetGamesData ggd)
        {
            InitializeComponent();
            this.ggd = ggd;
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
            btnAchieveList.Enabled = false;
            games = SaveData.Load();
            if (games.Count > 0)
            {
                if (ggd != null)
                {
                    ggd.Games.Clear();
                    ggd.Games.AddRange(games);

                    await ggd.LoadGameIconsAsync();
                    await ggd.LoadAchievementIconsAsync();
                }
                await LoadGameIconsAsync();
                await LoadAchievementIconsAsync();
                randGames = games.Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements).ToList();
                bestGame = randGames.OrderByDescending(g => g.Percent).FirstOrDefault();
                btnListGames.Enabled = true;
                btnMaxPercentGame.Enabled = true;
                btnRandomGame.Enabled = true;
            }
            profileImage.ImageLocation = user.ProfileImageUrl;
            lbNickname.Text = user.Nickname;
        }
        private void btnMaxPercentGame_Click(object sender, EventArgs e)
        {
            if (bestGame != null)
            {
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
            if (selectedGame != null) LaunchGame(selectedGame.ID);
        }
        private void btnRandomGame_Click(object sender, EventArgs e)
        {
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
            if (form2 != null && !form2.IsDisposed)
            {
                form2.BringToFront();
                return;
            }
            var sortedGames = games.Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements).OrderByDescending(g => g.Percent).ToList();
            form2 = new Form2(sortedGames);
            form2.FormClosed += (s, args) => form2 = null;
            form2.GameSelected += OnGameSelected;
            form2.Show();
        }
        private void OnGameSelected(Game game)
        {
            selectedGame = game;
            imageHeader.ImageLocation = GetHeaderImageUrl(int.Parse(game.ID));
            lbTitle.Text = $"{selectedGame.Name} - {selectedGame.UnlockedAchievements}/{selectedGame.TotalAchievements} ({selectedGame.Percent}%)";
            AutoResizeTitleFont();
            btnLaunchGame.Enabled = true;
            btnAchieveList.Enabled = true;
        }
        private void btnAchieveList_Click(object sender, EventArgs e)
        {
            if (selectedGame == null) return;
            if (form3 != null && !form3.IsDisposed)
            {
                form3.BringToFront();
                return;
            }
            form3 = new Form3(selectedGame);
            form3.FormClosed += (s, args) => form3 = null;
            form3.Show();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            if (selectedGame == null || ggd == null) return;
            if (selectedGame == null) return;
            btnRefresh.Enabled = false;
            try
            {
                List<GameAchievement> achievements = await ggd.LoadAchievementSchemaAsync(selectedGame.ID);
                bool success = await ggd.LoadPlayerAchievementStatusAsync(selectedGame.ID, achievements);
                if (!success) return;
                await ggd.LoadGlobalAchievementPercentagesAsync(selectedGame.ID, achievements);
                foreach (GameAchievement newAchievement in achievements)
                {
                    GameAchievement? oldAchievement = selectedGame.Achievements.FirstOrDefault(a => a.APIName == newAchievement.APIName);
                    if (oldAchievement != null) newAchievement.Icon = oldAchievement.Icon;
                }
                selectedGame.Achievements = achievements;
                selectedGame.TotalAchievements = achievements.Count;
                selectedGame.UnlockedAchievements = achievements.Count(a => a.Achieved);
                selectedGame.Percent = (int)(selectedGame.UnlockedAchievements * 100.0 / selectedGame.TotalAchievements);
                if (selectedGame.Percent >= 100)
                {
                    if (form3 != null && !form3.IsDisposed)
                    {
                        form3.Close();
                        form3 = null;
                    }
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
                    if (form3 != null && !form3.IsDisposed) form3.RefreshAchievements();
                }
                await ggd.UpdateSteamGamesAsync();
                randGames = games.Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements).ToList();
                bestGame = randGames.OrderByDescending(g => g.Percent).FirstOrDefault();
                gamesToSave = games.Where(g => g.HasAchievements && g.UnlockedAchievements < g.TotalAchievements && g.Achievements.Any(a => !a.Achieved)).Select(g => new Game { Name = g.Name, ID = g.ID, ImgIconUrl = g.ImgIconUrl, HasAchievements = g.HasAchievements, TotalAchievements = g.TotalAchievements, UnlockedAchievements = g.UnlockedAchievements, Percent = g.Percent, ImgIcon = g.ImgIcon, Achievements = g.Achievements.Where(a => !a.Achieved).ToList() }).ToList();
            }
            finally
            {
                SaveData.Save(gamesToSave);
                btnRefresh.Enabled = true;
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
    }
}