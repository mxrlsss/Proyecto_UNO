namespace UNO_Game
{
    partial class FormPartida
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
            this.PanelMazoP1 = new System.Windows.Forms.FlowLayoutPanel();
            this.PanelMazoP2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnVolverMenu = new System.Windows.Forms.Button();
            this.MazoRobar = new System.Windows.Forms.PictureBox();
            this.Pozo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.MazoRobar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pozo)).BeginInit();
            this.SuspendLayout();
            // 
            // PanelMazoP1
            // 
            this.PanelMazoP1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.PanelMazoP1.AutoScroll = true;
            this.PanelMazoP1.Location = new System.Drawing.Point(2, 12);
            this.PanelMazoP1.Name = "PanelMazoP1";
            this.PanelMazoP1.Size = new System.Drawing.Size(1438, 250);
            this.PanelMazoP1.TabIndex = 6;
            this.PanelMazoP1.WrapContents = false;
            this.PanelMazoP1.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelMazoP1_Paint);
            // 
            // PanelMazoP2
            // 
            this.PanelMazoP2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.PanelMazoP2.AutoScroll = true;
            this.PanelMazoP2.Location = new System.Drawing.Point(2, 429);
            this.PanelMazoP2.Name = "PanelMazoP2";
            this.PanelMazoP2.Size = new System.Drawing.Size(1438, 250);
            this.PanelMazoP2.TabIndex = 7;
            this.PanelMazoP2.WrapContents = false;
            this.PanelMazoP2.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelMazoP2_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1632, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "P1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Cursor = System.Windows.Forms.Cursors.PanNE;
            this.label2.Location = new System.Drawing.Point(1754, 982);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "P2";
            // 
            // btnVolverMenu
            // 
            this.btnVolverMenu.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnVolverMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolverMenu.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnVolverMenu.Location = new System.Drawing.Point(12, 12);
            this.btnVolverMenu.Name = "btnVolverMenu";
            this.btnVolverMenu.Size = new System.Drawing.Size(67, 54);
            this.btnVolverMenu.TabIndex = 0;
            this.btnVolverMenu.Text = "✕";
            this.btnVolverMenu.UseVisualStyleBackColor = false;
            this.btnVolverMenu.Click += new System.EventHandler(this.btnVolverMenu_Click);
            // 
            // MazoRobar
            // 
            this.MazoRobar.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.MazoRobar.Image = global::UNO_Game.Properties.Resources.Reverso;
            this.MazoRobar.Location = new System.Drawing.Point(926, 175);
            this.MazoRobar.Name = "MazoRobar";
            this.MazoRobar.Size = new System.Drawing.Size(204, 295);
            this.MazoRobar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.MazoRobar.TabIndex = 10;
            this.MazoRobar.TabStop = false;
            this.MazoRobar.Click += new System.EventHandler(this.MazoRobar_Click);
            // 
            // Pozo
            // 
            this.Pozo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Pozo.Image = global::UNO_Game.Properties.Resources.Rojo1;
            this.Pozo.Location = new System.Drawing.Point(218, 196);
            this.Pozo.Name = "Pozo";
            this.Pozo.Size = new System.Drawing.Size(291, 343);
            this.Pozo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Pozo.TabIndex = 5;
            this.Pozo.TabStop = false;
            // 
            // FormPartida
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UNO_Game.Properties.Resources.Background_partida;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1194, 707);
            this.Controls.Add(this.MazoRobar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PanelMazoP2);
            this.Controls.Add(this.PanelMazoP1);
            this.Controls.Add(this.Pozo);
            this.Controls.Add(this.btnVolverMenu);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormPartida";
            this.Text = "UNO";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            this.Load += new System.EventHandler(this.FormPartida_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MazoRobar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pozo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox Pozo;
        private System.Windows.Forms.FlowLayoutPanel PanelMazoP1;
        private System.Windows.Forms.FlowLayoutPanel PanelMazoP2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox MazoRobar;
        private System.Windows.Forms.Button btnVolverMenu;
    }
}