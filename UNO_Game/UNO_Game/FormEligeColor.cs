using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNO_Game
{
    public partial class FormEligeColor : Form
    {
        public string ColorElegido { get; private set; }

        public FormEligeColor()
        {
            InitializeComponent();
        }

        private void btnRojo_Click(object sender, EventArgs e)
        {
            ColorElegido = "Rojo";
            FormPartida formPartida = new FormPartida();
            formPartida.sonido("CambioColor");
            this.Close();
        }

        private void btnAzul_Click(object sender, EventArgs e)
        {
            ColorElegido = "Azul";
            FormPartida formPartida = new FormPartida();
            formPartida.sonido("CambioColor");
            this.Close();
        }

        private void btnVerde_Click(object sender, EventArgs e)
        {
            ColorElegido = "Verde";
            FormPartida formPartida = new FormPartida();
            formPartida.sonido("CambioColor");
            this.Close();
        }

        private void btnAmarillo_Click(object sender, EventArgs e)
        {
            FormPartida formPartida = new FormPartida();
            formPartida.sonido("CambioColor");
            ColorElegido = "Amarillo";
            this.Close();
        }


        private void FormEligeColor_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
    
}
