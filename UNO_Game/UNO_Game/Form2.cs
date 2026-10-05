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
        private Mazo mazoJuego;
        private List<Carta> mazoP1;
        private List<Carta> mazoP2;
        private Carta CartaEnMesa;

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



        private void ActualizarInterfazVisual()
        { 
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
            mazoJuego = new Mazo();
            mazoP1 = mazoJuego.RepartirMazo(7); 
            mazoP2 = mazoJuego.RepartirMazo(7);

            CartaEnMesa = mazoJuego.RobarCarta();

            string imgPozo = $"{CartaEnMesa.Color}{CartaEnMesa.Valor.ToString()}";
            Pozo.Image = (Image)Properties.Resources.ResourceManager.GetObject(imgPozo);

            MostrarMazoJugador(mazoP1, PanelMazoP1);
            MostrarMazoJugador(mazoP2, PanelMazoP2);
            ActualizarInterfazVisual();
        }

        private void MostrarMazoJugador(List<Carta> mazo, FlowLayoutPanel panel)
        {
            panel.Controls.Clear();

            foreach(var carta in mazo)
            {
                PictureBox pic = new PictureBox();
              
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Width = 70;
                pic.Height = 100;
                pic.Margin = new Padding(5);

                string nombreImg = $"{carta.Color}{carta.Valor.ToString()}";
                pic.Image = (Image)Properties.Resources.ResourceManager.GetObject(nombreImg);
                pic.Click += (sender, e) => { MessageBox.Show("Esto es la logica de click a la carta Clickeaste la carta"); };
                panel.Controls.Add(pic);
            }
        }
        


        private void Pozo_Click(object sender, EventArgs e)
        {

        }

        private void PanelMazoP2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void PanelMazoP1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
