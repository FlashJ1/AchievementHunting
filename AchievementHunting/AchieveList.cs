using AchievementHunting.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AchievementHunting
{
    public partial class AchieveList : Form
    {
        private Game game;
        private readonly HttpClient _httpClient = new HttpClient();
        public AchieveList(Game game)
        {
            InitializeComponent();
            this.game = game;
        }

        public void RefreshAchievements()
        {
            listViewAchievements.Items.Clear();
            var achievements = game.Achievements.Where(a => !a.Achieved).OrderByDescending(a => { double.TryParse(a.GlobalPercent, out double global); return global; }).ToList();
            foreach (GameAchievement achievement in achievements)
            {
                ListViewItem item = new ListViewItem(achievement.Name);
                item.SubItems.Add(achievement.Desc);
                item.SubItems.Add($"{achievement.GlobalPercent}%");
                if (achievement.Icon != null)
                {
                    imageList1.Images.Add(achievement.Icon);
                    item.ImageIndex = imageList1.Images.Count - 1;
                }
                listViewAchievements.Items.Add(item);
            }

        }
        public void UpdateGame(Game newGame)
        {
            game = newGame;
            Text = $"{game.Name} - Achievements";
            RefreshAchievements();
            listViewAchievements.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewAchievements.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private async void AchieveList_Load(object sender, EventArgs e)
        {
            Text = $"{game.Name} - Achievements";
            imageList1.ImageSize = new Size(64, 64);
            RefreshAchievements();
            listViewAchievements.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewAchievements.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

        }
    }
}
