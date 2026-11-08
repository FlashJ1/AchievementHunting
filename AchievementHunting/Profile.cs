using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AchievementHunting.Models;

namespace AchievementHunting
{
    public partial class Profile : Form
    {
        User user = new User();
        List<Game> games = new List<Game>();
        private readonly GetGamesData? ggd;
        private readonly Friend? friend;

        public Profile(List<Game> games)
        {
            InitializeComponent();
            user = SaveData.LoadUser();
            this.games = games;
        }

        public Profile(Friend friend)
        {
            InitializeComponent();
            this.friend = friend;
            user = friend;
            ggd = new GetGamesData(friend.SteamID);
        }

        private async void Profile_Load(object sender, EventArgs e)
        {
            lbNickname.Visible = false;
            lbCountGames.Visible = false;
            lbLastBeatenGame.Visible = false;
            if (friend != null && ggd != null)
            {
                try
                {
                    await ggd.GetOwnedGamesAsync();
                    await ggd.GetPlayerSummariesAsync();

                    await Task.WhenAll(ggd.LoadGameIconsAsync(), ggd.LoadGameAchievementsAsync());

                    await ggd.LoadAchievementDetailsAsync();
                    await ggd.LoadAchievementIconsAsync();

                    games = ggd.Games;
                    user = ggd.Player;
                    CalculateBeatenGames();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Не вдалося завантажити профіль друга: {ex.Message}"
                    );
                }
            }
            lbNickname.Visible = true;
            lbCountGames.Visible = true;
            lbLastBeatenGame.Visible = true;
            bigProfileImage.ImageLocation = user.ProfileFullImageUrl;
            lbNickname.Text = user.Nickname;
            RefreshProfile();
        }
        public void RefreshProfile()
        {
            int totalGames = games.Count(g => g.HasAchievements);
            int unlockedGames = games.Count(g => g.HasAchievements && g.UnlockedAchievements >= g.TotalAchievements);
            lbCountGames.Text = $"Beaten/Total Games: {unlockedGames}/{totalGames}";
            Game? lastBeatenGame = games.Where(g => g.HasAchievements && g.UnlockedAchievements >= g.TotalAchievements && g.BeatedAt.HasValue)
                                        .OrderByDescending(g => g.BeatedAt).FirstOrDefault();
            if (lastBeatenGame != null)
            {
                LBGImage.ImageLocation = $"https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/{lastBeatenGame.ID}/header.jpg";
                lbLastBeatenGame.Text = $"Last Beaten Game: {lastBeatenGame.Name}";
            }
            else lbLastBeatenGame.Text = "Last Beaten Game: N/A";
        }
        private void CalculateBeatenGames()
        {
            foreach (Game game in games)
            {
                if (!game.HasAchievements)
                    continue;
                if (game.UnlockedAchievements < game.TotalAchievements)
                    continue;
                GameAchievement? lastAchievement = game.Achievements
                    .Where(a => a.Achieved && a.AchievedAt.HasValue)
                    .OrderByDescending(a => a.AchievedAt).FirstOrDefault();
                game.BeatedAt = lastAchievement?.AchievedAt;
            }
        }
    }
}