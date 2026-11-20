using AchievementHunting.Models;
using Steamworks;
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
        ListView listBoxGames = new ListView();
        FlowLayoutPanel flowLayoutPanel1 = new FlowLayoutPanel();
        private bool sheetsCreated = false;
        public event Action<Game>? GameSelected;
        public ListGame(List<Game> games)
        {
            InitializeComponent();
            this.games = games;
            flowLayoutPanel1.SizeChanged += (s, e) => ResizeRows();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = "List Games";
            imageList1.ImageSize = new Size(32, 32);
            this.AutoScroll = true;
            cbOrientation.SelectedIndex = 0;
        }
        public void AddGameBySheets(Game game)
        {
                ListViewItem item = new ListViewItem(game.Name);
                item.Font = new Font("Segoe UI Semibold", 12,FontStyle.Bold);
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

        public void AddGameByOrientation(Game game)
        {
            if (cbOrientation.SelectedIndex == 1)
            {
                Panel panel = new Panel();
                panel.Size = new Size(RowWidth(), 108);
                panel.Margin = new Padding(5);

                PictureBox header = new PictureBox();
                header.Size = new Size(230, 108);
                header.ImageLocation = $"https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/{game.ID}/header.jpg";
                header.SizeMode = PictureBoxSizeMode.StretchImage;
                header.Location = new Point(5, 5);
                header.Click += (sender, e) =>
                {
                    GameSelected?.Invoke(game);
                };

                Label lbGameName = new Label();
                lbGameName.Text = game.Name;
                lbGameName.AutoSize = true;
                lbGameName.Size = new Size(ClientSize.Width, 45);
                lbGameName.BackColor = Color.Transparent;
                lbGameName.Location = new Point(240, 10);
                lbGameName.Font = new Font("Segoe UI Semibold", 24, FontStyle.Bold);

                Label lbAchieves = new Label();
                lbAchieves.Text = $"{game.UnlockedAchievements}/{game.TotalAchievements} ({game.Percent}%)";
                lbAchieves.AutoSize = true;
                lbAchieves.Size = new Size(ClientSize.Width, 60);
                lbAchieves.Location = new Point(240, 50);
                lbAchieves.BackColor = Color.Transparent;
                lbAchieves.Font = new Font("Segoe UI Semibold", 18, FontStyle.Bold);

                panel.Controls.Add(header);
                panel.Controls.Add(lbGameName);
                panel.Controls.Add(lbAchieves);
                flowLayoutPanel1.Controls.Add(panel);
            }
            else if (cbOrientation.SelectedIndex == 2)
            {
                Panel panel = new Panel();
                panel.Size = new Size(197, 350);
                panel.Margin = new Padding(5);

                PictureBox cover = new PictureBox();
                cover.Size = new Size(197, 301);
                cover.ImageLocation = $"https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/{game.ID}/library_600x900.jpg";
                cover.SizeMode = PictureBoxSizeMode.StretchImage;
                cover.Location = new Point(5, 5);
                cover.Click += (sender, e) =>
                {
                    GameSelected?.Invoke(game);
                };

                Label lbAchieves = new Label();
                lbAchieves.Text = $"{game.UnlockedAchievements}/{game.TotalAchievements} ({game.Percent}%)";
                lbAchieves.AutoSize = true;
                lbAchieves.Size = new Size(ClientSize.Width, 60);
                lbAchieves.Location = new Point(5, 321);
                lbAchieves.BackColor = Color.Transparent;
                lbAchieves.Font = new Font("Segoe UI Semibold", 18, FontStyle.Bold);

                panel.Controls.Add(cover);
                panel.Controls.Add(lbAchieves);
                flowLayoutPanel1.Controls.Add(panel);
            }       
        }

        private int RowWidth()
        {
            return Math.Max(flowLayoutPanel1.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 10, 320);
        }

        private void ResizeRows()
        {
            if (cbOrientation.SelectedIndex != 1)
                return;
            int width = RowWidth();
            foreach (Control row in flowLayoutPanel1.Controls)
                row.Width = width;
        }

        private void ClearFlowLayout()
        {
            flowLayoutPanel1.SuspendLayout();
            foreach (Control c in flowLayoutPanel1.Controls.Cast<Control>().ToList())
            {
                flowLayoutPanel1.Controls.Remove(c);
                c.Dispose();
            }
            flowLayoutPanel1.ResumeLayout();
        }

        private void listBoxGames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxGames.SelectedItems.Count == 0)
                return;
            Game? selectedGame = listBoxGames.SelectedItems[0].Tag as Game;
            if (selectedGame == null)
                return;
            GameSelected?.Invoke(selectedGame);
        }

        private void cbOrientation_SelectedIndexChanged(object sender, EventArgs e)
        {
            Controls.Remove(listBoxGames);
            Controls.Remove(flowLayoutPanel1);
            ClearFlowLayout();
            if (cbOrientation.SelectedIndex == 0)
            {
                CreateSheets();
                listBoxGames.Items.Clear();
                imageList1.Images.Clear();
                Controls.Add(listBoxGames);
                foreach (Game game in games)
                    AddGameBySheets(game);
                listBoxGames.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                listBoxGames.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            else
            {
                Controls.Remove(flowLayoutPanel1);
                CreateFlowLayout();
                foreach (Game game in games)
                    AddGameByOrientation(game);
            }
        }
        public void CreateSheets()
        {
            if (sheetsCreated)
                return;
            listBoxGames.Size = new Size(ClientSize.Width, ClientSize.Height);
            listBoxGames.Location = new Point(0, 41);
            listBoxGames.View = View.Details;
            listBoxGames.FullRowSelect = true;
            listBoxGames.GridLines = true;
            listBoxGames.SmallImageList = imageList1;
            listBoxGames.Columns.Add("Game", 300);
            listBoxGames.Columns.Add("Progress", 100);
            listBoxGames.Columns.Add("Achievements", 150);
            listBoxGames.Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold);
            listBoxGames.SelectedIndexChanged += listBoxGames_SelectedIndexChanged;
            sheetsCreated = true;
        }
        private void CreateFlowLayout()
        {
            flowLayoutPanel1.Size = new Size(ClientSize.Width, ClientSize.Height);
            flowLayoutPanel1.Location = new Point(0, 41);

            flowLayoutPanel1.AutoScroll = true;
            bool landscape = cbOrientation.SelectedIndex == 1;
            flowLayoutPanel1.FlowDirection = landscape ? FlowDirection.TopDown : FlowDirection.LeftToRight;
            flowLayoutPanel1.WrapContents = !landscape;
            flowLayoutPanel1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

            Controls.Add(flowLayoutPanel1);
        }
    }
}
