namespace AchievementHunting
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnListGames = new Button();
            btnMaxPercentGame = new Button();
            btnRandomGame = new Button();
            imageHeader = new PictureBox();
            btnLaunchGame = new Button();
            lbTitle = new Label();
            profileImage = new PictureBox();
            lbNickname = new Label();
            lbGamesProgress = new Label();
            btnAchieveList = new Button();
            btnRefresh = new Button();
            ((System.ComponentModel.ISupportInitialize)imageHeader).BeginInit();
            ((System.ComponentModel.ISupportInitialize)profileImage).BeginInit();
            SuspendLayout();
            // 
            // btnListGames
            // 
            btnListGames.Location = new Point(12, 92);
            btnListGames.Name = "btnListGames";
            btnListGames.Size = new Size(247, 23);
            btnListGames.TabIndex = 3;
            btnListGames.Text = "Список ігр з нездобутими досягненнями";
            btnListGames.UseVisualStyleBackColor = true;
            btnListGames.Click += btnListGames_Click;
            // 
            // btnMaxPercentGame
            // 
            btnMaxPercentGame.Location = new Point(12, 121);
            btnMaxPercentGame.Name = "btnMaxPercentGame";
            btnMaxPercentGame.Size = new Size(299, 23);
            btnMaxPercentGame.TabIndex = 4;
            btnMaxPercentGame.Text = "Максимальний відсоток досягнень до Perfect Game";
            btnMaxPercentGame.UseVisualStyleBackColor = true;
            btnMaxPercentGame.Click += btnMaxPercentGame_Click;
            // 
            // btnRandomGame
            // 
            btnRandomGame.Location = new Point(12, 150);
            btnRandomGame.Name = "btnRandomGame";
            btnRandomGame.Size = new Size(75, 23);
            btnRandomGame.TabIndex = 5;
            btnRandomGame.Text = "Рандом";
            btnRandomGame.UseVisualStyleBackColor = true;
            btnRandomGame.Click += btnRandomGame_Click;
            // 
            // imageHeader
            // 
            imageHeader.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            imageHeader.BorderStyle = BorderStyle.FixedSingle;
            imageHeader.Location = new Point(745, 121);
            imageHeader.Name = "imageHeader";
            imageHeader.Size = new Size(200, 300);
            imageHeader.SizeMode = PictureBoxSizeMode.StretchImage;
            imageHeader.TabIndex = 6;
            imageHeader.TabStop = false;
            // 
            // btnLaunchGame
            // 
            btnLaunchGame.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnLaunchGame.Location = new Point(745, 427);
            btnLaunchGame.Name = "btnLaunchGame";
            btnLaunchGame.Size = new Size(200, 23);
            btnLaunchGame.TabIndex = 7;
            btnLaunchGame.Text = "Запустити гру";
            btnLaunchGame.UseVisualStyleBackColor = true;
            btnLaunchGame.Click += btnLaunchGame_Click;
            // 
            // lbTitle
            // 
            lbTitle.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            lbTitle.Location = new Point(12, 376);
            lbTitle.Name = "lbTitle";
            lbTitle.Size = new Size(727, 45);
            lbTitle.TabIndex = 8;
            lbTitle.TextAlign = ContentAlignment.MiddleLeft;
            lbTitle.UseMnemonic = false;
            // 
            // profileImage
            // 
            profileImage.Location = new Point(12, 12);
            profileImage.Name = "profileImage";
            profileImage.Size = new Size(64, 64);
            profileImage.TabIndex = 10;
            profileImage.TabStop = false;
            // 
            // lbNickname
            // 
            lbNickname.AutoSize = true;
            lbNickname.Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold);
            lbNickname.Location = new Point(82, 21);
            lbNickname.Name = "lbNickname";
            lbNickname.Size = new Size(0, 45);
            lbNickname.TabIndex = 11;
            lbNickname.Tag = "";
            // 
            // lbGamesProgress
            // 
            lbGamesProgress.AutoSize = true;
            lbGamesProgress.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, 204);
            lbGamesProgress.Location = new Point(262, 285);
            lbGamesProgress.Name = "lbGamesProgress";
            lbGamesProgress.Size = new Size(0, 45);
            lbGamesProgress.TabIndex = 13;
            lbGamesProgress.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnAchieveList
            // 
            btnAchieveList.Location = new Point(745, 92);
            btnAchieveList.Name = "btnAchieveList";
            btnAchieveList.Size = new Size(200, 23);
            btnAchieveList.TabIndex = 14;
            btnAchieveList.Text = "Нездобуті досягнення";
            btnAchieveList.UseVisualStyleBackColor = true;
            btnAchieveList.Click += btnAchieveList_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(870, 10);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(75, 23);
            btnRefresh.TabIndex = 15;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(957, 450);
            Controls.Add(btnRefresh);
            Controls.Add(btnAchieveList);
            Controls.Add(lbGamesProgress);
            Controls.Add(lbNickname);
            Controls.Add(profileImage);
            Controls.Add(lbTitle);
            Controls.Add(btnLaunchGame);
            Controls.Add(imageHeader);
            Controls.Add(btnRandomGame);
            Controls.Add(btnMaxPercentGame);
            Controls.Add(btnListGames);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)imageHeader).EndInit();
            ((System.ComponentModel.ISupportInitialize)profileImage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnListGames;
        private Button btnMaxPercentGame;
        private Button btnRandomGame;
        private PictureBox imageHeader;
        private Button btnLaunchGame;
        private Label lbTitle;
        private PictureBox profileImage;
        private Label lbNickname;
        private Label lbGamesProgress;
        private Button btnAchieveList;
        private Button btnRefresh;
    }
}
