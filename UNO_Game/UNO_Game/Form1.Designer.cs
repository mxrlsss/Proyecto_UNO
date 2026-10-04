namespace UNO_Game
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
            this.Mazo_P1 = new System.Windows.Forms.PictureBox();
            this.Mazo_P2 = new System.Windows.Forms.PictureBox();
            this.Pozo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Mazo_P1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mazo_P2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pozo)).BeginInit();
            this.SuspendLayout();
            // 
            // Mazo_P1
            // 
            this.Mazo_P1.Image = global::UNO_Game.Properties.Resources.Red1;
            this.Mazo_P1.Location = new System.Drawing.Point(468, 433);
            this.Mazo_P1.Name = "Mazo_P1";
            this.Mazo_P1.Size = new System.Drawing.Size(96, 148);
            this.Mazo_P1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Mazo_P1.TabIndex = 0;
            this.Mazo_P1.TabStop = false;
            this.Mazo_P1.Click += new System.EventHandler(this.Mazo_P1_Click);
            // 
            // Mazo_P2
            // 
            this.Mazo_P2.Image = global::UNO_Game.Properties.Resources.Red1;
            this.Mazo_P2.Location = new System.Drawing.Point(468, 23);
            this.Mazo_P2.Name = "Mazo_P2";
            this.Mazo_P2.Size = new System.Drawing.Size(96, 148);
            this.Mazo_P2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Mazo_P2.TabIndex = 1;
            this.Mazo_P2.TabStop = false;
            this.Mazo_P2.Click += new System.EventHandler(this.Mazo_P2_Click);
            // 
            // Pozo
            // 
            this.Pozo.Image = global::UNO_Game.Properties.Resources.Red1;
            this.Pozo.Location = new System.Drawing.Point(542, 253);
            this.Pozo.Name = "Pozo";
            this.Pozo.Size = new System.Drawing.Size(54, 93);
            this.Pozo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Pozo.TabIndex = 2;
            this.Pozo.TabStop = false;
            this.Pozo.Click += new System.EventHandler(this.Pozo_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1244, 654);
            this.Controls.Add(this.Mazo_P1);
            this.Controls.Add(this.Mazo_P2);
            this.Controls.Add(this.Pozo);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Mazo_P1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mazo_P2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Pozo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox Mazo_P1;
        private System.Windows.Forms.PictureBox Mazo_P2;
        private System.Windows.Forms.PictureBox Pozo;
    }
}

