using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace UNO_Game
{
    //Aquí se carga el juego, barajean cartas, etc. Se puede acceder al menú principal desde aquí.
    
    public partial class FormPartida : Form
    {
        //Crea la conexión entre la URL y la base de datos
        private static readonly HttpClient http =
         new HttpClient { BaseAddress = new Uri("http://localhost:8000/") };

        private Form1 menuPrincipal; //Guarda una referencia al menú
        private Mazo mazoJuego;
        private List<Carta> mazoP1;
        private List<Carta> mazoP2;
        private Carta CartaEnMesa;
        string P1 = "Alexis";
        string P2 = "Ivan";

        private int turnoActual = 1; //1 para P1 y 2 para P2

        public FormPartida(Form1 menu)
        {
            InitializeComponent();
            this.menuPrincipal = menu; //Guarda la referencia al menú principal
            this.FormClosed += FormPartida_VentanaCerrada;
            this.FormClosing += FormPartida_FormClosing;
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
            if(btnVolver.Visible == false)
            {
                btnVolver.Visible = true;
                btnSonido.Visible = true;
            }
            else
            {
                btnVolver.Visible = false;
                btnSonido.Visible = false;
            }

        }

        private void FormPartida_FormClosing(object sender, FormClosingEventArgs e)
        {
            var respuesta = MessageBox.Show(
                "¿Seguro que quieres salir de la partida?",
                "Salir",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta == DialogResult.No)
                e.Cancel = true;
        }

        private void FormPartida_VentanaCerrada(object sender, FormClosedEventArgs e)
        {
            if (menuPrincipal != null)
                menuPrincipal.Show(); //Vuelve a mostrar el menú
               
        }

        private async void FormPartida_Load(object sender, EventArgs e)
        {
            mazoJuego = new Mazo();
            mazoJuego.Barajar();

            mazoP1 = mazoJuego.RepartirMazo(7); 
            mazoP2 = mazoJuego.RepartirMazo(7);

            CartaEnMesa = mazoJuego.RobarCarta();
            string imgPozo = $"{CartaEnMesa.Color}{CartaEnMesa.Valor.ToString()}";
            //MessageBox.Show($"Carta en mesa: {CartaEnMesa.Color} {CartaEnMesa.Valor}");
            Pozo.Image = (Image)Properties.Resources.ResourceManager.GetObject(imgPozo);

            if (CartaEnMesa.Color == "Comodin" || CartaEnMesa.Valor.ToString() == "Salto" || CartaEnMesa.Valor.ToString() == "Reversa" || CartaEnMesa.Valor.ToString() == "MasDos")
            {
                do
                {
                    CartaEnMesa = mazoJuego.RobarCarta();
                }
                while (CartaEnMesa.Color == "Comodin" || CartaEnMesa.Valor.ToString() == "Salto" || CartaEnMesa.Valor.ToString() == "Reversa" || CartaEnMesa.Valor.ToString() == "MasDos");
            }
            string imgPozo_correcta = $"{CartaEnMesa.Color}{CartaEnMesa.Valor.ToString()}"; 
            //MessageBox.Show($"Carta en mesa: {CartaEnMesa.Color} {CartaEnMesa.Valor}");
            Pozo.Image = (Image)Properties.Resources.ResourceManager.GetObject(imgPozo_correcta);

            MostrarMazoJugador(mazoP1, PanelMazoP1);
            MostrarMazoJugador(mazoP2, PanelMazoP2);
            ActualizarInterfazVisual();

            await CrearPartidaApi();
        }

        private void MostrarMazoJugador(List<Carta> mazo, FlowLayoutPanel panel)
        {
            panel.Controls.Clear();

            foreach(var carta in mazo)
            {
                PictureBox pic = new PictureBox();
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Width = 100;
                pic.Height = 130;
                pic.Margin = new Padding(5);


                string nombreImg = $"{carta.Color}{carta.Valor.ToString()}";
              

                pic.Image = (Image)Properties.Resources.ResourceManager.GetObject(nombreImg);

                pic.Tag = carta; // Almacena la carta en la propiedad Tag del PictureBoBox
                
                pic.Click += CartaJugador_Click; // Asigna el evento Click al PictureBox 

                panel.Controls.Add(pic);
            }
        }

        private async void CartaJugador_Click(object sender, EventArgs e) //Agregar base de datos para guardar la partida, y que se pueda continuar desde donde se dejó.
        {
            PictureBox PicClickeado = sender as PictureBox;
            FlowLayoutPanel PanelPadre = PicClickeado.Parent as FlowLayoutPanel;

            if(PanelPadre == PanelMazoP1 && turnoActual != 1)
            {
                sonido("CartaNoValida");
                MessageBox.Show($"Esperad Brochaho!, aun no es tu turno, es turno de {P2}");
                return;
            }
            if(PanelPadre == PanelMazoP2 && turnoActual != 2)
            {
                sonido("CartaNoValida");
                MessageBox.Show($"Esperad Brochaho!, aun no es tu turno, es turno de {P1}");
                return;
            }

            Carta cartaElegida = PicClickeado.Tag as Carta;

            if(cartaElegida != null)
            {
                //MessageBox.Show($"Carta elegida: {cartaElegida.Color} {cartaElegida.Valor}");
                if (cartaElegida.Color == CartaEnMesa.Color || cartaElegida.Valor == CartaEnMesa.Valor || cartaElegida.Color == "Comodin")
                {
                    CartaEnMesa = cartaElegida;

                    int jugadorQueTiro = turnoActual;                                  
                    string desc = $"{cartaElegida.Color} {cartaElegida.Valor}";

                    string imgPozo = cartaElegida.NombreRecurso;
                    Pozo.Image = (Image)Properties.Resources.ResourceManager.GetObject(imgPozo);

                    if(PanelPadre == PanelMazoP1)
                    {
                        mazoP1.Remove(cartaElegida);
                    }
                    else if (PanelPadre == PanelMazoP2)
                    {
                        mazoP2.Remove(cartaElegida);
                    }


                    PicClickeado.Dispose();

                    //cartas especiales 

                    bool pierdeTurno = false;
                    int oponente = (turnoActual == 1) ? 2 : 1; 
                    string valorSTR = cartaElegida.Valor.ToString();

                    //+2
                    if(valorSTR == "MasDos")
                    {
                        CastigarJugador(oponente, 2);
                        pierdeTurno = true;
                        if(oponente == 1)
                            MessageBox.Show($"Toma 2 {P1}");
                        else
                            MessageBox.Show($"Toma 2 {P2}");
                    }
                    //Saltos (para 1 a 1 funciona, si es de mas jugadores debe cambiarse (proximosmparciales)
                    if (valorSTR == "Salto" || valorSTR == "Reversa")
                    {
                        pierdeTurno = true;
                        if(oponente == 1)
                            MessageBox.Show($"Vuelves a tirar {P2}");
                        else
                            MessageBox.Show($"Vuelves a tirar {P1}");
                    }
                    //mas 4 y cambio de clor
                    if (cartaElegida.Color == "Comodin")
                    {
                        if(valorSTR == "MasCuatro")
                        {
                            CastigarJugador(oponente, 4);
                            pierdeTurno = true;
                            if (oponente == 1)
                                MessageBox.Show($"Toma 4, {P1}");
                            else
                                MessageBox.Show($"Toma 4, {P2}");
                        }

                        FormEligeColor selector = new FormEligeColor();
                        selector.ShowDialog();
                        CartaEnMesa.Color = selector.ColorElegido; 
                        MessageBox.Show($"Cambio a {CartaEnMesa.Color}");
                    }

                    //cierran cartas especiales

                    if(!pierdeTurno)
                        turnoActual = (turnoActual == 1) ? 2 : 1; // Cambia el turno al otro jugador

                    await RegistrarMovimientoApi(jugadorQueTiro, "jugar_carta", desc);

                    List<Carta> manoDelQueTiro = (jugadorQueTiro == 1) ? mazoP1 : mazoP2;
                    if (manoDelQueTiro.Count == 0)
                    {
                        await FinalizarPartidaApi(jugadorQueTiro);
                        PanelMazoP1.Enabled = false;
                        PanelMazoP2.Enabled = false;
                        MazoRobar.Enabled = false;
                        sonido("Ganar");
                        if(jugadorQueTiro == 1)
                            MessageBox.Show($"¡Felixidades {P1}, has ganado brochacho!");
                        else
                            MessageBox.Show($"¡Felixidades {P2}, has ganado brochacho!");
                    }
                }
                else
                {
                    sonido("CartaNoValida"); 
                    MessageBox.Show("No puedes jugar esta carta brochaho, debe coincidir el color o el valor!");
                }
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


        private async void MazoRobar_Click(object sender, EventArgs e)
        {
            Carta nuevaCarta = mazoJuego.RobarCarta();

            if(nuevaCarta != null)
            {
                int jugador = turnoActual;
                if (turnoActual == 1)
                {
                    mazoP1.Add(nuevaCarta);
                    MostrarMazoJugador(mazoP1, PanelMazoP1);
                }
                else
                {
                    mazoP2.Add(nuevaCarta);
                    MostrarMazoJugador(mazoP2, PanelMazoP2);
                }

                await RegistrarMovimientoApi(jugador, "robar_carta",
                    $"{nuevaCarta.Color} {nuevaCarta.Valor}");
            }
            else
            {
                MessageBox.Show("No hay más cartas en el mazo para robar.");
            }
        }

        private void CastigarJugador(int Jugador, int cantidad)
        {

            for (int i = 0; i < cantidad; i++)
            {
                sonido($"TomaCartasMas{cantidad}");
                Carta castigo = mazoJuego.RobarCarta();
                if (castigo != null)
                {
                    if (Jugador == 1)
                        mazoP1.Add(castigo);
                    else mazoP2.Add(castigo);

                }
                /*else
                {
                    MessageBox.Show("No hay más cartas en el mazo para robar.");
                    break;
                }*/
            }
            if (Jugador == 1)
                MostrarMazoJugador(mazoP1, PanelMazoP1);
            else
                MostrarMazoJugador(mazoP2, PanelMazoP2);
        }

        public void sonido(string sonido) //se pasan versatilemnte 
        {
            try
            {
                var stream = Properties.Resources.ResourceManager.GetStream(sonido); 

                using (System.Media.SoundPlayer reproducir = new System.Media.SoundPlayer(stream))
                {
                    reproducir.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo reproducir el sonido de castigo: " + ex.Message);
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSonido_Click(object sender, EventArgs e)
        {

        }
    }

}
