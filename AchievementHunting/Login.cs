using AchievementHunting.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AchievementHunting
{
    public partial class Login : Form
    {
        public string SteamID => tbSteamID.Text.Trim();
        private GetGamesData ggd;
        private List<Game> games = new List<Game>();

        private MainMenu? form1;
        public Login()
        {
            InitializeComponent();
        }

        private async void btLogin_Click(object sender, EventArgs e)
        {
            btLogin.Enabled = false;
            ggd = new GetGamesData(SteamID);
            await ggd.GetGamesDataFromSteam();
            form1 = new MainMenu(ggd);
            form1.FormClosed += (s, args) => form1 = null;
            form1.Show();
            this.Hide();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            this.Text = "Achievement Hunting - Login";
        }
    }
}
