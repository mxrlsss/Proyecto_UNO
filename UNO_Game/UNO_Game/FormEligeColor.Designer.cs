namespace UNO_Game
{
    partial class FormEligeColor
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
            this.btnRojo = new System.Windows.Forms.Button();
            this.btnAzul = new System.Windows.Forms.Button();
            this.btnVerde = new System.Windows.Forms.Button();
            this.btnAmarillo = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnRojo
            // 
            this.btnRojo.BackColor = System.Drawing.Color.Red;
            this.btnRojo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRojo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRojo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnRojo.Location = new System.Drawing.Point(170, 202);
            this.btnRojo.Name = "btnRojo";
            this.btnRojo.Size = new System.Drawing.Size(209, 100);
            this.btnRojo.TabIndex = 0;
            this.btnRojo.UseVisualStyleBackColor = false;
            this.btnRojo.Click += new System.EventHandler(this.btnRojo_Click);
            // 
            // btnAzul
            // 
            this.btnAzul.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnAzul.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAzul.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAzul.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnAzul.Location = new System.Drawing.Point(403, 202);
            this.btnAzul.Name = "btnAzul";
            this.btnAzul.Size = new System.Drawing.Size(209, 100);
            this.btnAzul.TabIndex = 1;
            this.btnAzul.UseVisualStyleBackColor = false;
            this.btnAzul.Click += new System.EventHandler(this.btnAzul_Click);
            // 
            // btnVerde
            // 
            this.btnVerde.BackColor = System.Drawing.Color.Green;
            this.btnVerde.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVerde.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnVerde.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnVerde.Location = new System.Drawing.Point(170, 320);
            this.btnVerde.Name = "btnVerde";
            this.btnVerde.Size = new System.Drawing.Size(209, 100);
            this.btnVerde.TabIndex = 2;
            this.btnVerde.UseVisualStyleBackColor = false;
            this.btnVerde.Click += new System.EventHandler(this.btnVerde_Click);
            // 
            // btnAmarillo
            // 
            this.btnAmarillo.BackColor = System.Drawing.Color.Yellow;
            this.btnAmarillo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAmarillo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAmarillo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnAmarillo.Location = new System.Drawing.Point(403, 320);
            this.btnAmarillo.Name = "btnAmarillo";
            this.btnAmarillo.Size = new System.Drawing.Size(209, 100);
            this.btnAmarillo.TabIndex = 3;
            this.btnAmarillo.UseVisualStyleBackColor = false;
            this.btnAmarillo.Click += new System.EventHandler(this.btnAmarillo_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Bubblegum Sans", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(242, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(288, 56);
            this.label1.TabIndex = 4;
            this.label1.Text = "¡Elige un color!";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // FormEligeColor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UNO_Game.Properties.Resources.ElegirColorBck;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(848, 576);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAmarillo);
            this.Controls.Add(this.btnVerde);
            this.Controls.Add(this.btnAzul);
            this.Controls.Add(this.btnRojo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormEligeColor";
            this.Opacity = 0.98D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormEligeColor";
            this.Load += new System.EventHandler(this.FormEligeColor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRojo;
        private System.Windows.Forms.Button btnAzul;
        private System.Windows.Forms.Button btnVerde;
        private System.Windows.Forms.Button btnAmarillo;
        private System.Windows.Forms.Label label1;
    }
}