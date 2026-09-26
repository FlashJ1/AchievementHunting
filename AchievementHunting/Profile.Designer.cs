namespace AchievementHunting
{
    partial class Profile
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Profile));
            bigProfileImage = new PictureBox();
            lbNickname = new Label();
            lbTotalGames = new Label();
            lbUnlockedGames = new Label();
            LBGImage = new PictureBox();
            lbLastBeatenGame = new Label();
            ((System.ComponentModel.ISupportInitialize)bigProfileImage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)LBGImage).BeginInit();
            SuspendLayout();
            // 
            // bigProfileImage
            // 
            bigProfileImage.Location = new Point(12, 12);
            bigProfileImage.Name = "bigProfileImage";
            bigProfileImage.Size = new Size(128, 128);
            bigProfileImage.SizeMode = PictureBoxSizeMode.StretchImage;
            bigProfileImage.TabIndex = 0;
            bigProfileImage.TabStop = false;
            // 
            // lbNickname
            // 
            lbNickname.AutoSize = true;
            lbNickname.Font = new Font("Segoe UI Semibold", 36F, FontStyle.Bold);
            lbNickname.Location = new Point(146, 12);
            lbNickname.Name = "lbNickname";
            lbNickname.Size = new Size(239, 65);
            lbNickname.TabIndex = 1;
            lbNickname.Text = "nickname";
            // 
            // lbTotalGames
            // 
            lbTotalGames.AutoSize = true;
            lbTotalGames.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            lbTotalGames.Location = new Point(146, 77);
            lbTotalGames.Name = "lbTotalGames";
            lbTotalGames.Size = new Size(125, 30);
            lbTotalGames.TabIndex = 2;
            lbTotalGames.Text = "totalGames";
            // 
            // lbUnlockedGames
            // 
            lbUnlockedGames.AutoSize = true;
            lbUnlockedGames.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            lbUnlockedGames.Location = new Point(146, 110);
            lbUnlockedGames.Name = "lbUnlockedGames";
            lbUnlockedGames.Size = new Size(173, 30);
            lbUnlockedGames.TabIndex = 3;
            lbUnlockedGames.Text = "UnlockedGames";
            // 
            // LBGImage
            // 
            LBGImage.Location = new Point(12, 223);
            LBGImage.Name = "LBGImage";
            LBGImage.Size = new Size(460, 215);
            LBGImage.SizeMode = PictureBoxSizeMode.StretchImage;
            LBGImage.TabIndex = 4;
            LBGImage.TabStop = false;
            // 
            // lbLastBeatenGame
            // 
            lbLastBeatenGame.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, 204);
            lbLastBeatenGame.Location = new Point(12, 143);
            lbLastBeatenGame.Name = "lbLastBeatenGame";
            lbLastBeatenGame.Size = new Size(460, 77);
            lbLastBeatenGame.TabIndex = 5;
            lbLastBeatenGame.Text = "Last Beaten Game";
            // 
            // Profile
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lbLastBeatenGame);
            Controls.Add(LBGImage);
            Controls.Add(lbUnlockedGames);
            Controls.Add(lbTotalGames);
            Controls.Add(lbNickname);
            Controls.Add(bigProfileImage);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Profile";
            Text = "Profile";
            Load += Profile_Load;
            ((System.ComponentModel.ISupportInitialize)bigProfileImage).EndInit();
            ((System.ComponentModel.ISupportInitialize)LBGImage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox bigProfileImage;
        private Label lbNickname;
        private Label lbTotalGames;
        private Label lbUnlockedGames;
        private PictureBox LBGImage;
        private Label lbLastBeatenGame;
    }
}