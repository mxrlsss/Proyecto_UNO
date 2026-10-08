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
            this.PanelMazoP1 = new System.Windows.Forms.FlowLayoutPanel();
            this.PanelMazoP2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.btnVolverMenu = new System.Windows.Forms.Button();
            this.MazoRobar = new System.Windows.Forms.PictureBox();
            this.Pozo = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnVolver = new System.Windows.Forms.Button();
            this.btnSonido = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.MazoRobar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pozo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // PanelMazoP1
            // 
            this.PanelMazoP1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.PanelMazoP1.AutoScroll = true;
            this.PanelMazoP1.BackColor = System.Drawing.Color.Transparent;
            this.PanelMazoP1.Location = new System.Drawing.Point(487, 12);
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
            this.PanelMazoP2.BackColor = System.Drawing.Color.Transparent;
            this.PanelMazoP2.Location = new System.Drawing.Point(487, 1122);
            this.PanelMazoP2.Name = "PanelMazoP2";
            this.PanelMazoP2.Size = new System.Drawing.Size(1438, 250);
            this.PanelMazoP2.TabIndex = 7;
            this.PanelMazoP2.WrapContents = false;
            this.PanelMazoP2.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelMazoP2_Paint);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Cursor = System.Windows.Forms.Cursors.PanNE;
            this.label2.Font = new System.Drawing.Font("Bubblegum Sans", 16F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(221, 1049);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 37);
            this.label2.TabIndex = 9;
            this.label2.Text = "Player 2";
            // 
            // btnVolverMenu
            // 
            this.btnVolverMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVolverMenu.AutoSize = true;
            this.btnVolverMenu.BackColor = System.Drawing.Color.Transparent;
            this.btnVolverMenu.BackgroundImage = global::UNO_Game.Properties.Resources.Menu;
            this.btnVolverMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnVolverMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolverMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolverMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolverMenu.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnVolverMenu.Location = new System.Drawing.Point(2076, 12);
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
            this.MazoRobar.Location = new System.Drawing.Point(1896, 522);
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
            this.Pozo.Location = new System.Drawing.Point(924, 500);
            this.Pozo.Name = "Pozo";
            this.Pozo.Size = new System.Drawing.Size(291, 343);
            this.Pozo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Pozo.TabIndex = 5;
            this.Pozo.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Cursor = System.Windows.Forms.Cursors.PanNE;
            this.label1.Font = new System.Drawing.Font("Bubblegum Sans", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(208, 288);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 37);
            this.label1.TabIndex = 11;
            this.label1.Text = "player 1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::UNO_Game.Properties.Resources.Alexis_Profile;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(186, -14);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(182, 299);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImage = global::UNO_Game.Properties.Resources.Ivan_Profile;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(186, 1100);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(182, 299);
            this.pictureBox2.TabIndex = 13;
            this.pictureBox2.TabStop = false;
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
            this.btnVolver.Click += new System.EventHandler(this.btnVolver_Click);
            this.btnVolver.Visible = false;
            // 
            // btnSonido
            // 
            this.btnSonido.BackColor = System.Drawing.Color.Transparent;
            this.btnSonido.BackgroundImage = global::UNO_Game.Properties.Resources.Sonido;
            this.btnSonido.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSonido.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSonido.Location = new System.Drawing.Point(2070, 169);
            this.btnSonido.Name = "btnSonido";
            this.btnSonido.Size = new System.Drawing.Size(82, 59);
            this.btnSonido.TabIndex = 15;
            this.btnSonido.UseVisualStyleBackColor = false;
            this.btnSonido.Click += new System.EventHandler(this.btnSonido_Click);
            this.btnSonido.Visible = false;
            // 
            // FormPartida
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UNO_Game.Properties.Resources.Background_partida;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(2164, 1400);
            this.Controls.Add(this.btnSonido);
            this.Controls.Add(this.btnVolver);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MazoRobar);
            this.Controls.Add(this.label2);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox Pozo;
        private System.Windows.Forms.FlowLayoutPanel PanelMazoP1;
        private System.Windows.Forms.FlowLayoutPanel PanelMazoP2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox MazoRobar;
        private System.Windows.Forms.Button btnVolverMenu;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Button btnSonido;
    }
}