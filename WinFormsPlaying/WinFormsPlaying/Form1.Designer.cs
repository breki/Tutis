namespace WinFormsPlaying
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.brekiTabPanel1 = new WinFormsPlaying.BrekiTabsFlow();
            this.tabButton4 = new WinFormsPlaying.TabButton(null);
            this.tabButton2 = new WinFormsPlaying.TabButton(null);
            this.closeButton1 = new WinFormsPlaying.CloseButton();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(121, 82);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(22, 22);
            this.button1.TabIndex = 0;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button2.Location = new System.Drawing.Point(278, 92);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button2.Name = "button2";
            this.button2.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.button2.Size = new System.Drawing.Size(185, 35);
            this.button2.TabIndex = 1;
            this.button2.Text = "Map Sources";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // brekiTabPanel1
            // 
            this.brekiTabPanel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.brekiTabPanel1.Location = new System.Drawing.Point(231, 241);
            this.brekiTabPanel1.Name = "brekiTabPanel1";
            this.brekiTabPanel1.Size = new System.Drawing.Size(505, 48);
            this.brekiTabPanel1.TabIndex = 7;
            // 
            // tabButton4
            // 
            this.tabButton4.AutoSize = true;
            this.tabButton4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tabButton4.CanClose = false;
            this.tabButton4.Location = new System.Drawing.Point(53, 133);
            this.tabButton4.Margin = new System.Windows.Forms.Padding(0);
            this.tabButton4.Name = "tabButton4";
            this.tabButton4.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.tabButton4.Size = new System.Drawing.Size(107, 34);
            this.tabButton4.TabIndex = 6;
            this.tabButton4.Text = "tabButton4";
            this.tabButton4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tabButton4.UseCompatibleTextRendering = true;
            this.tabButton4.UseVisualStyleBackColor = true;
            // 
            // tabButton2
            // 
            this.tabButton2.AutoSize = true;
            this.tabButton2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tabButton2.Location = new System.Drawing.Point(535, 52);
            this.tabButton2.Margin = new System.Windows.Forms.Padding(0);
            this.tabButton2.Name = "tabButton2";
            this.tabButton2.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.tabButton2.Size = new System.Drawing.Size(112, 34);
            this.tabButton2.TabIndex = 4;
            this.tabButton2.Text = "tabButton2";
            this.tabButton2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tabButton2.UseCompatibleTextRendering = true;
            this.tabButton2.UseVisualStyleBackColor = true;
            // 
            // closeButton1
            // 
            this.closeButton1.AutoSize = true;
            this.closeButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.closeButton1.BackColor = System.Drawing.Color.Transparent;
            this.closeButton1.FlatAppearance.BorderSize = 0;
            this.closeButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.closeButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton1.Image = ((System.Drawing.Image)(resources.GetObject("closeButton1.Image")));
            this.closeButton1.Location = new System.Drawing.Point(463, 203);
            this.closeButton1.Name = "closeButton1";
            this.closeButton1.Size = new System.Drawing.Size(22, 22);
            this.closeButton1.TabIndex = 3;
            this.closeButton1.TabStop = false;
            this.closeButton1.Text = null;
            this.closeButton1.UseCompatibleTextRendering = true;
            this.closeButton1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(806, 359);
            this.Controls.Add(this.brekiTabPanel1);
            this.Controls.Add(this.tabButton2);
            this.Controls.Add(this.closeButton1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private CloseButton closeButton1;
        private TabButton tabButton2;
        private TabButton tabButton4;
        private BrekiTabsFlow brekiTabPanel1;
    }
}

