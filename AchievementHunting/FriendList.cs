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
    public partial class FriendList : Form
    {
        public List<Friend> friends = new List<Friend>();
        public FriendList()
        {
            InitializeComponent();
        }
        public FriendList(List<Friend> friends)
        {
            InitializeComponent();
            this.friends = friends;
        }

        private void FriendList_Load(object sender, EventArgs e)
        {
            this.Text = "Friend List";
            foreach (var friend in friends)
            {
               AddItem(friend);
            }
        }

        private void AddItem(Friend friend)
        {
            Panel panel = new Panel();
            panel.Size = new Size(flowLayoutPanel1.ClientSize.Width - 25, 74);

            PictureBox avatar = new PictureBox();
            avatar.Size = new Size(64, 64);
            avatar.ImageLocation = friend.ProfileImageUrl;
            avatar.SizeMode = PictureBoxSizeMode.Normal;
            avatar.Location = new Point(5, 5);
            avatar.Click += (sender, e) =>
            {
                Profile profile = new Profile(friend);
                profile.Show();
            };

            Label lbNickname = new Label();
            lbNickname.Text = friend.Nickname;
            lbNickname.AutoSize = true;
            lbNickname.Location = new Point(80, 21);
            lbNickname.Font = new Font("Segoe UI Semibold", 24, FontStyle.Bold);
            lbNickname.TextAlign = ContentAlignment.MiddleCenter;

            panel.Controls.Add(avatar);
            panel.Controls.Add(lbNickname);

            flowLayoutPanel1.Controls.Add(panel);
        }
    }
}