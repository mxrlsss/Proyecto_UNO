namespace UNO_Game
{
    partial class FormSala
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSala));
            this.btnHistorial = new System.Windows.Forms.Button();
            this.CreatePartida = new System.Windows.Forms.Button();
            this.Join_Partida = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnHistorial
            // 
            this.btnHistorial.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnHistorial.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnHistorial.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnHistorial.Font = new System.Drawing.Font("Bubblegum Sans", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHistorial.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnHistorial.Location = new System.Drawing.Point(755, 863);
            this.btnHistorial.Name = "btnHistorial";
            this.btnHistorial.Size = new System.Drawing.Size(269, 61);
            this.btnHistorial.TabIndex = 2;
            this.btnHistorial.Text = "Ver Historial";
            this.btnHistorial.UseVisualStyleBackColor = false;
            this.btnHistorial.Click += new System.EventHandler(this.btnHistorial_Click);
            // 
            // CreatePartida
            // 
            this.CreatePartida.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.CreatePartida.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.CreatePartida.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.CreatePartida.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.CreatePartida.Font = new System.Drawing.Font("Bubblegum Sans", 12F, System.Drawing.FontStyle.Bold);
            this.CreatePartida.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.CreatePartida.Location = new System.Drawing.Point(755, 668);
            this.CreatePartida.Name = "CreatePartida";
            this.CreatePartida.Size = new System.Drawing.Size(269, 67);
            this.CreatePartida.TabIndex = 3;
            this.CreatePartida.Text = "Crear partida";
            this.CreatePartida.UseVisualStyleBackColor = false;
            this.CreatePartida.Click += new System.EventHandler(this.CreatePartida_Click);
            // 
            // Join_Partida
            // 
            this.Join_Partida.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Join_Partida.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Join_Partida.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Join_Partida.Font = new System.Drawing.Font("Bubblegum Sans", 12F, System.Drawing.FontStyle.Bold);
            this.Join_Partida.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Join_Partida.Location = new System.Drawing.Point(755, 764);
            this.Join_Partida.Name = "Join_Partida";
            this.Join_Partida.Size = new System.Drawing.Size(269, 67);
            this.Join_Partida.TabIndex = 4;
            this.Join_Partida.Text = "Unirse a una partida";
            this.Join_Partida.UseVisualStyleBackColor = false;
            // 
            // FormSala
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UNO_Game.Properties.Resources.Background_partida;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1616, 1108);
            this.Controls.Add(this.Join_Partida);
            this.Controls.Add(this.CreatePartida);
            this.Controls.Add(this.btnHistorial);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSala";
            this.Text = "Sala ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnHistorial;
        private System.Windows.Forms.Button CreatePartida;
        private System.Windows.Forms.Button Join_Partida;
    }
}