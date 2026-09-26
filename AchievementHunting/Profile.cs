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

        public Profile(List<Game> games)
        {
            InitializeComponent();
            user = SaveData.LoadUser();
            this.games = games;
        }

        private void Profile_Load(object sender, EventArgs e)
        {
            bigProfileImage.ImageLocation = user.ProfileFullImageUrl;
            lbNickname.Text = user.Nickname;
            int totalGames = games.Count(g => g.HasAchievements);
            int unlockedGames = games.Count(g => g.HasAchievements && g.UnlockedAchievements >= g.TotalAchievements);
            lbTotalGames.Text = $"Total Games: {totalGames}";
            lbUnlockedGames.Text = $"Beaten Games: {unlockedGames}/{totalGames}";
            Game? lastBeatenGame = games.Where(g => g.HasAchievements && g.UnlockedAchievements >= g.TotalAchievements && g.BeatedAt.HasValue).OrderByDescending(g => g.BeatedAt).FirstOrDefault();
            if (lastBeatenGame != null)
            {
                LBGImage.ImageLocation = $"https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/{lastBeatenGame.ID}/header.jpg";
                lbLastBeatenGame.Text = $"Last Beaten Game: {lastBeatenGame.Name}";
            }
            else lbLastBeatenGame.Text = "Last Beaten Game: N/A";
        }
    }
}