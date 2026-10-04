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
            this.btnVolverMenu = new System.Windows.Forms.Button();
            this.Mazo_P1 = new System.Windows.Forms.PictureBox();
            this.Mazo_P2 = new System.Windows.Forms.PictureBox();
            this.Pozo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Mazo_P1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mazo_P2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pozo)).BeginInit();
            this.SuspendLayout();
            // 
            // btnVolverMenu
            // 
            this.btnVolverMenu.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnVolverMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolverMenu.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnVolverMenu.Location = new System.Drawing.Point(1086, 12);
            this.btnVolverMenu.Name = "btnVolverMenu";
            this.btnVolverMenu.Size = new System.Drawing.Size(67, 54);
            this.btnVolverMenu.TabIndex = 0;
            this.btnVolverMenu.Text = "✕";
            this.btnVolverMenu.UseVisualStyleBackColor = false;
            this.btnVolverMenu.Click += new System.EventHandler(this.btnVolverMenu_Click);
            // 
            // Mazo_P1
            // 
            this.Mazo_P1.Image = global::UNO_Game.Properties.Resources.Red1;
            this.Mazo_P1.Location = new System.Drawing.Point(518, 556);
            this.Mazo_P1.Name = "Mazo_P1";
            this.Mazo_P1.Size = new System.Drawing.Size(96, 148);
            this.Mazo_P1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Mazo_P1.TabIndex = 3;
            this.Mazo_P1.TabStop = false;
            // 
            // Mazo_P2
            // 
            this.Mazo_P2.Image = global::UNO_Game.Properties.Resources.Red1;
            this.Mazo_P2.Location = new System.Drawing.Point(518, 23);
            this.Mazo_P2.Name = "Mazo_P2";
            this.Mazo_P2.Size = new System.Drawing.Size(96, 148);
            this.Mazo_P2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Mazo_P2.TabIndex = 4;
            this.Mazo_P2.TabStop = false;
            // 
            // Pozo
            // 
            this.Pozo.Image = global::UNO_Game.Properties.Resources.Red1;
            this.Pozo.Location = new System.Drawing.Point(540, 318);
            this.Pozo.Name = "Pozo";
            this.Pozo.Size = new System.Drawing.Size(54, 93);
            this.Pozo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Pozo.TabIndex = 5;
            this.Pozo.TabStop = false;
            // 
            // FormPartida
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1165, 756);
            this.Controls.Add(this.Mazo_P1);
            this.Controls.Add(this.Mazo_P2);
            this.Controls.Add(this.Pozo);
            this.Controls.Add(this.btnVolverMenu);
            this.Name = "FormPartida";
            this.Text = "UNO";
            this.Load += new System.EventHandler(this.FormPartida_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Mazo_P1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mazo_P2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pozo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnVolverMenu;
        private System.Windows.Forms.PictureBox Mazo_P1;
        private System.Windows.Forms.PictureBox Mazo_P2;
        private System.Windows.Forms.PictureBox Pozo;
    }
}