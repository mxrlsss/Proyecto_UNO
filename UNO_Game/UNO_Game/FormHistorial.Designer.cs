namespace UNO_Game
{
    partial class FormHistorial
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
            this.dataGridViewPartida = new System.Windows.Forms.DataGridView();
            this.dataGridViewMovimiento = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPartida)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMovimiento)).BeginInit();
            this.SuspendLayout();
            // 
            // btnVolverMenu
            // 
            this.btnVolverMenu.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnVolverMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolverMenu.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnVolverMenu.Location = new System.Drawing.Point(12, 384);
            this.btnVolverMenu.Name = "btnVolverMenu";
            this.btnVolverMenu.Size = new System.Drawing.Size(89, 54);
            this.btnVolverMenu.TabIndex = 1;
            this.btnVolverMenu.Text = "Volver";
            this.btnVolverMenu.UseVisualStyleBackColor = false;
            this.btnVolverMenu.Click += new System.EventHandler(this.btnVolverMenu_Click);
            // 
            // dataGridViewPartida
            // 
            this.dataGridViewPartida.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPartida.Location = new System.Drawing.Point(114, 27);
            this.dataGridViewPartida.Name = "dataGridViewPartida";
            this.dataGridViewPartida.RowHeadersWidth = 62;
            this.dataGridViewPartida.RowTemplate.Height = 28;
            this.dataGridViewPartida.Size = new System.Drawing.Size(240, 150);
            this.dataGridViewPartida.TabIndex = 2;
            this.dataGridViewPartida.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPartida_CellContentClick);
            // 
            // dataGridViewMovimiento
            // 
            this.dataGridViewMovimiento.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMovimiento.Location = new System.Drawing.Point(417, 27);
            this.dataGridViewMovimiento.Name = "dataGridViewMovimiento";
            this.dataGridViewMovimiento.RowHeadersWidth = 62;
            this.dataGridViewMovimiento.RowTemplate.Height = 28;
            this.dataGridViewMovimiento.Size = new System.Drawing.Size(240, 150);
            this.dataGridViewMovimiento.TabIndex = 3;
            // 
            // FormHistorial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dataGridViewMovimiento);
            this.Controls.Add(this.dataGridViewPartida);
            this.Controls.Add(this.btnVolverMenu);
            this.Name = "FormHistorial";
            this.Text = "Historial";
            this.Load += new System.EventHandler(this.FormHistorial_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPartida)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMovimiento)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnVolverMenu;
        private System.Windows.Forms.DataGridView dataGridViewPartida;
        private System.Windows.Forms.DataGridView dataGridViewMovimiento;
    }
}