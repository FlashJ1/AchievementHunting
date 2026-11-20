namespace AchievementHunting
{
    partial class ListGame
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListGame));
            imageList1 = new ImageList(components);
            cbOrientation = new ComboBox();
            SuspendLayout();
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // cbOrientation
            // 
            cbOrientation.DropDownStyle = ComboBoxStyle.DropDownList;
            cbOrientation.Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            cbOrientation.FormattingEnabled = true;
            cbOrientation.ImeMode = ImeMode.NoControl;
            cbOrientation.Items.AddRange(new object[] { "Sheets", "Landscape", "Portrait" });
            cbOrientation.Location = new Point(12, 12);
            cbOrientation.Name = "cbOrientation";
            cbOrientation.Size = new Size(121, 28);
            cbOrientation.TabIndex = 1;
            cbOrientation.SelectedIndexChanged += cbOrientation_SelectedIndexChanged;
            // 
            // ListGame
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(cbOrientation);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ListGame";
            Text = "Form2";
            Load += Form2_Load;
            ResumeLayout(false);
        }

        #endregion
        private ImageList imageList1;
        private ComboBox cbOrientation;
    }
}