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
        public string ColorElegidoTranslate;

        public FormEligeColor()
        {
            InitializeComponent();
        }

        private void btnRojo_Click(object sender, EventArgs e)
        {
            ColorElegido = "Rojo";
            ColorElegidoTranslate = "Red";
            this.Close();
        }

        private void btnAzul_Click(object sender, EventArgs e)
        {
            ColorElegido = "Azul";
            ColorElegidoTranslate = "DodgerBlue";
            this.Close();
        }

        private void btnVerde_Click(object sender, EventArgs e)
        {
            ColorElegido = "Verde";
            ColorElegidoTranslate = "Green";
            this.Close();
        }

        private void btnAmarillo_Click(object sender, EventArgs e)
        {
            ColorElegido = "Amarillo";
            ColorElegidoTranslate = "Yellow";
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
