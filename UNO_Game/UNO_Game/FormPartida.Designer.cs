using System;
using System.Net.Http;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPartida));
            this.PanelMazoP2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnVolverMenu = new System.Windows.Forms.Button();
            this.MazoRobar = new System.Windows.Forms.PictureBox();
            this.Pozo = new System.Windows.Forms.PictureBox();
            this.P2_avatar = new System.Windows.Forms.PictureBox();
            this.btnVolver = new System.Windows.Forms.Button();
            this.btnSonido = new System.Windows.Forms.Button();
            this.P2_Nick = new System.Windows.Forms.PictureBox();
            this.ColorCambioP2 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.btnUNO = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.MazoRobar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pozo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.P2_avatar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.P2_Nick)).BeginInit();
            this.SuspendLayout();
            // 
            // PanelMazoP2
            // 
            this.PanelMazoP2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.PanelMazoP2.AutoScroll = true;
            this.PanelMazoP2.BackColor = System.Drawing.Color.Transparent;
            this.PanelMazoP2.Location = new System.Drawing.Point(367, 772);
            this.PanelMazoP2.Name = "PanelMazoP2";
            this.PanelMazoP2.Size = new System.Drawing.Size(1438, 250);
            this.PanelMazoP2.TabIndex = 7;
            this.PanelMazoP2.WrapContents = false;
            this.PanelMazoP2.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelMazoP2_Paint);
            // 
            // btnVolverMenu
            // 
            this.btnVolverMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVolverMenu.AutoSize = true;
            this.btnVolverMenu.BackColor = System.Drawing.Color.Transparent;
            this.btnVolverMenu.BackgroundImage = global::UNO_Game.Properties.Resources.Menu;
            this.btnVolverMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnVolverMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolverMenu.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnVolverMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolverMenu.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnVolverMenu.Location = new System.Drawing.Point(1836, 12);
            this.btnVolverMenu.Name = "btnVolverMenu";
            this.btnVolverMenu.Size = new System.Drawing.Size(79, 69);
            this.btnVolverMenu.TabIndex = 0;
            this.btnVolverMenu.UseVisualStyleBackColor = false;
            this.btnVolverMenu.Click += new System.EventHandler(this.btnVolverMenu_Click);
            // 
            // MazoRobar
            // 
            this.MazoRobar.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.MazoRobar.BackColor = System.Drawing.Color.Transparent;
            this.MazoRobar.Image = global::UNO_Game.Properties.Resources.Reverso;
            this.MazoRobar.Location = new System.Drawing.Point(1656, 347);
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
            this.Pozo.BackColor = System.Drawing.Color.Transparent;
            this.Pozo.Image = global::UNO_Game.Properties.Resources.Rojo1;
            this.Pozo.Location = new System.Drawing.Point(804, 325);
            this.Pozo.Name = "Pozo";
            this.Pozo.Size = new System.Drawing.Size(291, 343);
            this.Pozo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Pozo.TabIndex = 5;
            this.Pozo.TabStop = false;
           
            // 
            // btnVolver
            // 
            this.btnVolver.BackColor = System.Drawing.Color.Transparent;
            this.btnVolver.BackgroundImage = global::UNO_Game.Properties.Resources.Inicio;
            this.btnVolver.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnVolver.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnVolver.Location = new System.Drawing.Point(2073, 87);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(82, 59);
            this.btnVolver.TabIndex = 14;
            this.btnVolver.UseVisualStyleBackColor = false;
            this.btnVolver.Visible = false;
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            // 
            // btnSonido
            // 
            this.btnSonido.AutoSize = true;
            this.btnSonido.BackColor = System.Drawing.Color.Transparent;
            this.btnSonido.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSonido.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.btnSonido.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSonido.Location = new System.Drawing.Point(2070, 169);
            this.btnSonido.Name = "btnSonido";
            this.btnSonido.Size = new System.Drawing.Size(82, 59);
            this.btnSonido.TabIndex = 15;
            this.btnSonido.UseVisualStyleBackColor = false;
            this.btnSonido.Visible = false;
            // 
            // P2_Nick
            // 
            this.P2_Nick.BackColor = System.Drawing.Color.Transparent;
            this.P2_Nick.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.P2_Nick.Location = new System.Drawing.Point(226, 1045);
            this.P2_Nick.Name = "P2_Nick";
            this.P2_Nick.Size = new System.Drawing.Size(103, 49);
            this.P2_Nick.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.P2_Nick.TabIndex = 17;
            this.P2_Nick.TabStop = false;
            // 
            // ColorCambioP2
            // 
            this.ColorCambioP2.Font = new System.Drawing.Font("Bubblegum Sans", 14F, System.Drawing.FontStyle.Bold);
            this.ColorCambioP2.ForeColor = System.Drawing.Color.White;
            this.ColorCambioP2.Location = new System.Drawing.Point(838, 1045);
            this.ColorCambioP2.Name = "ColorCambioP2";
            this.ColorCambioP2.Size = new System.Drawing.Size(425, 49);
            this.ColorCambioP2.TabIndex = 18;
            this.ColorCambioP2.Text = "Color a tirar";
            this.ColorCambioP2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ColorCambioP2.Click += new System.EventHandler(this.label1_Click);
            // 
            // btnUNO
            // 
            this.btnUNO.BackColor = System.Drawing.Color.Firebrick;
            this.btnUNO.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnUNO.Font = new System.Drawing.Font("Bubblegum Sans", 14F, System.Drawing.FontStyle.Bold);
            this.btnUNO.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnUNO.Location = new System.Drawing.Point(2023, 1203);
            this.btnUNO.Name = "btnUNO";
            this.btnUNO.Size = new System.Drawing.Size(112, 72);
            this.btnUNO.TabIndex = 19;
            this.btnUNO.Text = "UNO";
            this.btnUNO.UseVisualStyleBackColor = false;
            this.btnUNO.Click += new System.EventHandler(this.btnUNO_Click);
            // 
            // FormPartida
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UNO_Game.Properties.Resources.Background_partida;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(2164, 1410);
            this.Controls.Add(this.btnUNO);
            this.ClientSize = new System.Drawing.Size(1924, 1050);
            this.Controls.Add(this.ColorCambioP2);
            this.Controls.Add(this.P2_Nick);
            this.Controls.Add(this.btnSonido);
            this.Controls.Add(this.btnVolver);
            this.Controls.Add(this.P2_avatar);
            this.Controls.Add(this.MazoRobar);
            this.Controls.Add(this.PanelMazoP2);
            this.Controls.Add(this.Pozo);
            this.Controls.Add(this.btnVolverMenu);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormPartida";
            this.Text = "UNO";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormPartida_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MazoRobar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pozo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.P2_avatar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.P2_Nick)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox Pozo;
        private System.Windows.Forms.FlowLayoutPanel PanelMazoP2;
        private System.Windows.Forms.PictureBox MazoRobar;
        private System.Windows.Forms.Button btnVolverMenu;
        private System.Windows.Forms.PictureBox P2_avatar;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Button btnSonido;
        private System.Windows.Forms.PictureBox P2_Nick;
        private System.Windows.Forms.Label ColorCambioP2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button btnUNO;
    }
}