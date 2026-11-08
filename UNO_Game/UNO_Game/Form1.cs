using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UNO_Game;

namespace UNO_Game
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Start_Button_Click(object sender, EventArgs e)
        {
            FormPartida formPartida = new FormPartida(this, true); // El 'true' indica que es Host
            formPartida.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormHistorial formhistorial = new FormHistorial();
            formhistorial.Show();
            this.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Mazo_P1_Click(object sender, EventArgs e)
        {

        }

        private void Mazo_P2_Click(object sender, EventArgs e)
        {

        }

        private void Pozo_Click(object sender, EventArgs e)
        {

        }

        private void btnUnirseSala_Click(object sender, EventArgs e)
        {
            
            Form prompt = new Form()
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Unirse a Partida",
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = "Ingresa la contraseña:" };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 240 };
            Button confirmation = new Button() { Text = "Aceptar", Left = 160, Width = 100, Top = 80, DialogResult = DialogResult.OK };

            confirmation.Click += (s, ev) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                string pass = textBox.Text;

                if (pass != "2410")
                {
                    MessageBox.Show("Contraseña incorrecta. Acceso denegado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                FormPartida formPartida = new FormPartida(this, false); // El 'false' indica que es Invitado
                formPartida.Show();
                this.Hide();
            }
        }
    }
}