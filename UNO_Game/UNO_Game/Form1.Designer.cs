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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Start_Button = new System.Windows.Forms.Button();
            this.btnHistorial = new System.Windows.Forms.Button();
            this.btnUnirseSala = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Start_Button
            // 
            this.Start_Button.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Start_Button.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Start_Button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Start_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.Start_Button.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Start_Button.Location = new System.Drawing.Point(364, 453);
            this.Start_Button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Start_Button.Name = "Start_Button";
            this.Start_Button.Size = new System.Drawing.Size(239, 54);
            this.Start_Button.TabIndex = 0;
            this.Start_Button.Text = "Iniciar Partida";
            this.Start_Button.UseVisualStyleBackColor = false;
            this.Start_Button.Click += new System.EventHandler(this.Start_Button_Click);
            // 
            // btnHistorial
            // 
            this.btnHistorial.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnHistorial.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnHistorial.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnHistorial.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHistorial.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnHistorial.Location = new System.Drawing.Point(12, 458);
            this.btnHistorial.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnHistorial.Name = "btnHistorial";
            this.btnHistorial.Size = new System.Drawing.Size(239, 49);
            this.btnHistorial.TabIndex = 1;
            this.btnHistorial.Text = "Ver Historial";
            this.btnHistorial.UseVisualStyleBackColor = false;
            this.btnHistorial.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnUnirseSala
            // 
            this.btnUnirseSala.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnUnirseSala.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnUnirseSala.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnUnirseSala.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnUnirseSala.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUnirseSala.Location = new System.Drawing.Point(709, 453);
            this.btnUnirseSala.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnUnirseSala.Name = "btnUnirseSala";
            this.btnUnirseSala.Size = new System.Drawing.Size(239, 54);
            this.btnUnirseSala.TabIndex = 2;
            this.btnUnirseSala.Text = "Unirse a Partida";
            this.btnUnirseSala.UseVisualStyleBackColor = false;
            this.btnUnirseSala.Click += new System.EventHandler(this.btnUnirseSala_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UNO_Game.Properties.Resources.Background_Main;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(970, 518);
            this.Controls.Add(this.btnUnirseSala);
            this.Controls.Add(this.btnHistorial);
            this.Controls.Add(this.Start_Button);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "UNO GAME";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Start_Button;
        private System.Windows.Forms.Button btnHistorial;
        private System.Windows.Forms.Button btnUnirseSala;
    }
}