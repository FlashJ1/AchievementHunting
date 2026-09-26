namespace AchievementHunting
{
    partial class AchieveList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AchieveList));
            imageList1 = new ImageList(components);
            listViewAchievements = new ListView();
            Name = new ColumnHeader();
            Description = new ColumnHeader();
            GlobalPercent = new ColumnHeader();
            SuspendLayout();
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // listViewAchievements
            // 
            listViewAchievements.Columns.AddRange(new ColumnHeader[] { Name, Description, GlobalPercent });
            listViewAchievements.Dock = DockStyle.Fill;
            listViewAchievements.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            listViewAchievements.FullRowSelect = true;
            listViewAchievements.Location = new Point(0, 0);
            listViewAchievements.Name = "listViewAchievements";
            listViewAchievements.Size = new Size(800, 450);
            listViewAchievements.SmallImageList = imageList1;
            listViewAchievements.TabIndex = 0;
            listViewAchievements.UseCompatibleStateImageBehavior = false;
            listViewAchievements.View = View.Details;
            // 
            // Name
            // 
            Name.Text = "Name";
            // 
            // Description
            // 
            Description.Text = "Description";
            Description.Width = 120;
            // 
            // GlobalPercent
            // 
            GlobalPercent.Text = "Global %";
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(listViewAchievements);
            Icon = (Icon)resources.GetObject("$this.Icon");
            //Name = "Form3";
            Text = "Form3";
            Load += AchieveList_Load;
            ResumeLayout(false);
        }

        #endregion

        private ImageList imageList1;
        private ListView listViewAchievements;
        private ColumnHeader Name;
        private ColumnHeader Description;
        private ColumnHeader GlobalPercent;
    }
}