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
    //Aquí se carga el juego, barajean cartas, etc. Se puede acceder al menú principal desde aquí.
    public partial class FormPartida : Form
    {
        private Form1 menuPrincipal; //Guarda una referencia al menú
        public FormPartida(Form1 menu)
        {
            InitializeComponent();
            this.menuPrincipal = menu; //Guarda la referencia al menú principal
            this.FormClosed += FormPartida_VentanaCerrada; 
        }

        public FormPartida()
        {
            InitializeComponent();
        }

        private void btnVolverMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormPartida_VentanaCerrada(object sender, FormClosedEventArgs e)
        {
            if (menuPrincipal != null)
                menuPrincipal.Show(); //Vuelve a mostrar el menú
               
        }

        private void FormPartida_Load(object sender, EventArgs e)
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
    }
}
