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

    public partial class ListGame : Form
    {
        private readonly List<Game> games;
        private readonly HttpClient httpClient = new HttpClient();
        public event Action<Game>? GameSelected;
        public ListGame(List<Game> games)
        {
            InitializeComponent();

            this.games = games;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = "List Games";
            imageList1.ImageSize = new Size(32, 32);
            foreach (Game game in games)
            {
                AddGame(game);
            }
            listBoxGames.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listBoxGames.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        public void AddGame(Game game)
        {
            ListViewItem item = new ListViewItem(game.Name);
            item.Tag = game;
            if (game.ImgIcon != null)
            {
                imageList1.Images.Add(game.ImgIcon);
                item.ImageIndex = imageList1.Images.Count - 1;
            }
            item.SubItems.Add($"{game.Percent}%");
            item.SubItems.Add($"{game.UnlockedAchievements}/{game.TotalAchievements}");
            listBoxGames.Items.Add(item);
        }

        private void listBoxGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxGames.SelectedItems.Count == 0) return;
            Game? selectedGame = listBoxGames.SelectedItems[0].Tag as Game;
            if (selectedGame == null) return;
            GameSelected?.Invoke(selectedGame);
        }
    }
}
