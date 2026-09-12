namespace AchievementHunting
{
    partial class Form2
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            listBoxGames = new ListView();
            Game = new ColumnHeader();
            Percent = new ColumnHeader();
            imageList1 = new ImageList(components);
            Achievements = new ColumnHeader();
            SuspendLayout();
            // 
            // listBoxGames
            // 
            listBoxGames.Columns.AddRange(new ColumnHeader[] { Game, Percent, Achievements });
            listBoxGames.Dock = DockStyle.Fill;
            listBoxGames.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            listBoxGames.FullRowSelect = true;
            listBoxGames.Location = new Point(0, 0);
            listBoxGames.Name = "listBoxGames";
            listBoxGames.Size = new Size(800, 450);
            listBoxGames.SmallImageList = imageList1;
            listBoxGames.TabIndex = 0;
            listBoxGames.UseCompatibleStateImageBehavior = false;
            listBoxGames.View = View.Details;
            listBoxGames.SelectedIndexChanged += listBoxGames_SelectedIndexChanged;
            // 
            // Game
            // 
            Game.Text = "Game";
            Game.Width = 350;
            // 
            // Percent
            // 
            Percent.Text = "Percent";
            Percent.TextAlign = HorizontalAlignment.Center;
            Percent.Width = 80;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // Achievements
            // 
            Achievements.Text = "Achievements";
            Achievements.TextAlign = HorizontalAlignment.Center;
            Achievements.Width = 200;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(listBoxGames);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form2";
            Text = "Form2";
            Load += Form2_Load;
            ResumeLayout(false);
        }

        #endregion

        private ListView listBoxGames;
        private ImageList imageList1;
        private ColumnHeader Game;
        private ColumnHeader Percent;
        private ColumnHeader Achievements;
    }
}