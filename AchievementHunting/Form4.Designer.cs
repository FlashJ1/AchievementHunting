namespace AchievementHunting
{
    partial class Form4
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form4));
            lbLogin = new Label();
            label1 = new Label();
            tbSteamID = new TextBox();
            btLogin = new Button();
            SuspendLayout();
            // 
            // lbLogin
            // 
            lbLogin.AutoSize = true;
            lbLogin.Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold, GraphicsUnit.Point, 204);
            lbLogin.Location = new Point(95, 9);
            lbLogin.Name = "lbLogin";
            lbLogin.Size = new Size(101, 45);
            lbLogin.TabIndex = 0;
            lbLogin.Text = "Login";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label1.Location = new Point(12, 112);
            label1.Name = "label1";
            label1.Size = new Size(113, 32);
            label1.TabIndex = 1;
            label1.Text = "Steam ID";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tbSteamID
            // 
            tbSteamID.Location = new Point(12, 147);
            tbSteamID.Name = "tbSteamID";
            tbSteamID.Size = new Size(271, 23);
            tbSteamID.TabIndex = 2;
            // 
            // btLogin
            // 
            btLogin.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            btLogin.Location = new Point(95, 176);
            btLogin.Name = "btLogin";
            btLogin.Size = new Size(101, 41);
            btLogin.TabIndex = 3;
            btLogin.Text = "Login";
            btLogin.UseVisualStyleBackColor = true;
            btLogin.Click += btLogin_Click;
            // 
            // Form4
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(295, 450);
            Controls.Add(btLogin);
            Controls.Add(tbSteamID);
            Controls.Add(label1);
            Controls.Add(lbLogin);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form4";
            Text = "Form4";
            Load += Form4_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbLogin;
        private Label label1;
        private TextBox tbSteamID;
        private Button btLogin;
    }
}