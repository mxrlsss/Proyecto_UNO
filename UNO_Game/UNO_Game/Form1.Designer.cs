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
<<<<<<< HEAD
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
=======
            this.Start_Button = new System.Windows.Forms.Button();
            this.btnHistorial = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Start_Button
            // 
            this.Start_Button.BackColor = System.Drawing.SystemColors.ControlDark;
            this.Start_Button.Location = new System.Drawing.Point(302, 161);
            this.Start_Button.Name = "Start_Button";
            this.Start_Button.Size = new System.Drawing.Size(198, 45);
            this.Start_Button.TabIndex = 0;
            this.Start_Button.Text = "Iniciar Partida";
            this.Start_Button.UseVisualStyleBackColor = false;
            this.Start_Button.Click += new System.EventHandler(this.Start_Button_Click);
            // 
            // btnHistorial
            // 
            this.btnHistorial.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.btnHistorial.Location = new System.Drawing.Point(302, 240);
            this.btnHistorial.Name = "btnHistorial";
            this.btnHistorial.Size = new System.Drawing.Size(198, 46);
            this.btnHistorial.TabIndex = 1;
            this.btnHistorial.Text = "Ver Historial";
            this.btnHistorial.UseVisualStyleBackColor = false;
            this.btnHistorial.Click += new System.EventHandler(this.button1_Click);
>>>>>>> 03f2644a26e5a20e2df5f579d01be9154c25b2dd
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
<<<<<<< HEAD
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
=======
            this.ClientSize = new System.Drawing.Size(1091, 647);
            this.Controls.Add(this.btnHistorial);
            this.Controls.Add(this.Start_Button);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
>>>>>>> 03f2644a26e5a20e2df5f579d01be9154c25b2dd
            this.ResumeLayout(false);

        }

        #endregion
<<<<<<< HEAD

        private System.Windows.Forms.PictureBox Mazo_P1;
        private System.Windows.Forms.PictureBox Mazo_P2;
        private System.Windows.Forms.PictureBox Pozo;
=======
        private System.Windows.Forms.Button Start_Button;
        private System.Windows.Forms.Button btnHistorial;

>>>>>>> 03f2644a26e5a20e2df5f579d01be9154c25b2dd
    }
}

