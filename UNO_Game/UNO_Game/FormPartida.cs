using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

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
        private List<Carta> pozoDescarte = new List<Carta>();
        private Carta CartaEnMesa;
        private bool MusicaSilencio = false;
        string P1 = "Alexis";
        string P2 = "Iván";
        public int turnoActual; //1 para P1 y 2 para P2
        private System.Media.SoundPlayer SoundFondo;
        System.Media.SoundPlayer efecto;
        bool HayGanador = false;

        private bool esHost;
        private System.Net.Sockets.TcpListener servidor;
        private System.Net.Sockets.TcpClient cliente;
        private System.Net.Sockets.NetworkStream stream;
        private bool JuegoIniciado = false;

        public FormPartida(Form1 menu)
        {
            InitializeComponent();
            this.menuPrincipal = menu;
            this.FormClosed += FormPartida_VentanaCerrada;
            this.FormClosing += FormPartida_FormClosing;
        }
        public FormPartida()
        {
            InitializeComponent();
        }

        public FormPartida(Form1 menu, bool esHost)
        {
            InitializeComponent();
            this.menuPrincipal = menu;
            this.esHost = esHost;
            this.FormClosed += FormPartida_VentanaCerrada;
            this.FormClosing += FormPartida_FormClosing;
        }

        private void ActualizarInterfazVisual()
        {
        }

        private void btnVolverMenu_Click(object sender, EventArgs e)
        {
            if (btnVolver.Visible == false)
            {
                btnVolver.Visible = true;
                //btnSonido.Visible = true;
            }
            else
            {
                btnVolver.Visible = false;
                //btnSonido.Visible = false;
            }

        }

        private void FormPartida_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (stream != null) stream.Close();
                if (cliente != null) cliente.Close();
                if (servidor != null) servidor.Stop();
            }
            catch { }

            if (!HayGanador)
            {
                var respuesta = MessageBox.Show(
               "¿Seguro que quieres salir de la partida?",
               "Volver al menu de inicio",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Question);

                if (respuesta == DialogResult.No)
                    e.Cancel = true;
            }

        }

        private void FormPartida_VentanaCerrada(object sender, FormClosedEventArgs e)
        {
            if (menuPrincipal != null)
                menuPrincipal.Show(); //Vuelve a mostrar el menú

        }

        private async void FormPartida_Load(object sender, EventArgs e)
        {
            PanelMazoP1.Enabled = false;
            PanelMazoP2.Enabled = false;
            MazoRobar.Enabled = false;
            Pozo.Enabled = false;
            this.Text = "UNO - Sala de Espera";

            if (esHost) IniciarServidor();
            else ConectarAlHost();
        }

        private async void IniciarJuegoReal()
        {
            JuegoIniciado = true;
            this.Text = "UNO - En Partida";
            Pozo.Enabled = true;

            Random rand_turno = new Random();
            turnoActual = rand_turno.Next(1, 3);
            indicadorTurno();
            mazoJuego = new Mazo();
            mazoJuego.Barajar();
            mazoP1 = mazoJuego.RepartirMazo(7);
            mazoP2 = mazoJuego.RepartirMazo(7);
            ColorCambioP1.Visible = false;
            ColorCambioP2.Visible = false;

            CartaEnMesa = mazoJuego.RobarCarta();
            string imgPozo = $"{CartaEnMesa.Color}{CartaEnMesa.Valor.ToString()}";
            Pozo.Image = (Image)Properties.Resources.ResourceManager.GetObject(imgPozo);

            if (CartaEnMesa.Color == "Comodin" || CartaEnMesa.Valor.ToString() == "Salto" || CartaEnMesa.Valor.ToString() == "Reversa" || CartaEnMesa.Valor.ToString() == "MasDos")
            {
                do { CartaEnMesa = mazoJuego.RobarCarta(); }
                while (CartaEnMesa.Color == "Comodin" || CartaEnMesa.Valor.ToString() == "Salto" || CartaEnMesa.Valor.ToString() == "Reversa" || CartaEnMesa.Valor.ToString() == "MasDos");
            }
            string imgPozo_correcta = $"{CartaEnMesa.Color}{CartaEnMesa.Valor.ToString()}";
            Pozo.Image = (Image)Properties.Resources.ResourceManager.GetObject(imgPozo_correcta);

            MostrarMazoJugador(mazoP1, PanelMazoP1);
            MostrarMazoJugador(mazoP2, PanelMazoP2);
            ActualizarInterfazVisual();

            PanelMazoP1.Enabled = true;
            PanelMazoP2.Enabled = true;
            MazoRobar.Enabled = true;

            if (esHost) await CrearPartidaApi(); // Solo el host crea la partida
        }

        private async void IniciarServidor()
        {
            try
            {
                servidor = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, 2410);
                servidor.Start();
                this.Text = "UNO - Esperando a Iván...";

                cliente = await servidor.AcceptTcpClientAsync();
                stream = cliente.GetStream();

                this.Text = "UNO - ¡Iván conectado! Iniciando...";
                await Task.Delay(1000);

                IniciarJuegoReal();
                _ = RecibirMensajesAsync();
            }
            catch (Exception ex) { MessageBox.Show("Error al crear la sala: " + ex.Message); }
        }

        private async void ConectarAlHost()
        {
            try
            {
                cliente = new System.Net.Sockets.TcpClient();
                await cliente.ConnectAsync("127.0.0.1", 2410);
                stream = cliente.GetStream();

                this.Text = "UNO - ¡Conectado! Iniciando...";
                await Task.Delay(1000);

                IniciarJuegoReal();
                _ = RecibirMensajesAsync();
            }
            catch
            {
                MessageBox.Show("No se pudo conectar. Asegúrate de que el Host esté esperando.", "Error");
                this.Close();
            }
        }

        private async Task RecibirMensajesAsync()
        {
            byte[] buffer = new byte[4096];
            try
            {
                while (cliente.Connected)
                {
                    int bytesLeidos = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesLeidos == 0) break;
                    string json = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);
                    ProcesarMensajeRed(json);
                }
            }
            catch { }
        }

        private void ProcesarMensajeRed(string json)
        {
            try
            {
                dynamic msg = JsonConvert.DeserializeObject(json);
                string accion = msg.Accion;

                if (accion == "JUGAR")
                {
                    string nombreCarta = msg.Carta;
                    int jugadorQueTiro = msg.Jugador;
                    List<Carta> mazoRival = (jugadorQueTiro == 1) ? mazoP1 : mazoP2;
                    Carta carta = mazoRival.FirstOrDefault(c => c.NombreRecurso == nombreCarta);
                    if (carta != null)
                    {
                        mazoRival.Remove(carta);
                        pozoDescarte.Add(CartaEnMesa);
                        CartaEnMesa = carta;
                        Pozo.Image = (Image)Properties.Resources.ResourceManager.GetObject(carta.NombreRecurso);

                        if (jugadorQueTiro == 1) MostrarMazoJugador(mazoP1, PanelMazoP1);
                        else MostrarMazoJugador(mazoP2, PanelMazoP2);
                    }
                }
                else if (accion == "ROBAR")
                {
                    int jugadorQueRobo = msg.Jugador;
                    Carta nuevaCarta = mazoJuego.RobarCarta(pozoDescarte);
                    if (jugadorQueRobo == 1) { mazoP1.Insert(0, nuevaCarta); MostrarMazoJugador(mazoP1, PanelMazoP1); }
                    else { mazoP2.Insert(0, nuevaCarta); MostrarMazoJugador(mazoP2, PanelMazoP2); }
                }
                else if (accion == "CAMBIO_TURNO")
                {
                    turnoActual = msg.Turno;
                    indicadorTurno();
                }
            }
            catch { }
        }

        private async void EnviarMensajeRed(object datos)
        {
            if (stream != null && cliente != null && cliente.Connected)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(datos);
                    byte[] buffer = Encoding.UTF8.GetBytes(json);
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
                catch { }
            }
        }


        private void MostrarMazoJugador(List<Carta> mazo, FlowLayoutPanel panel)
        {
            panel.Controls.Clear();
            foreach (var carta in mazo)
            {
                PictureBox pic = new PictureBox();
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Width = 100;
                pic.Height = 130;
                pic.Margin = new Padding(5);
                string nombreImg = $"{carta.Color}{carta.Valor.ToString()}";
                pic.Image = (Image)Properties.Resources.ResourceManager.GetObject(nombreImg);
                pic.Tag = carta;
                pic.Click += CartaJugador_Click;
                panel.Controls.Add(pic);
            }
        }

        private async void CartaJugador_Click(object sender, EventArgs e)
        {
            if (!JuegoIniciado) return;

            bool CambioColor = false;
            var color = "";
            PictureBox PicClickeado = sender as PictureBox;
            FlowLayoutPanel PanelPadre = PicClickeado.Parent as FlowLayoutPanel;

            if (PanelPadre == PanelMazoP1 && turnoActual != 1)
            {
                sonido("CartaNoValida");
                MessageBox.Show($"Esperad Brochaho!, aun no es tu turno, es turno de {P2}");
                return;
            }
            if (PanelPadre == PanelMazoP2 && turnoActual != 2)
            {
                sonido("CartaNoValida");
                MessageBox.Show($"Esperad Brochaho!, aun no es tu turno, es turno de {P1}");
                return;
            }

            Carta cartaElegida = PicClickeado.Tag as Carta;

            if (cartaElegida != null)
            {
                if (cartaElegida.Color == CartaEnMesa.Color || cartaElegida.Valor == CartaEnMesa.Valor || cartaElegida.Color == "Comodin")
                {
                    ColorCambioP1.Visible = false;
                    ColorCambioP2.Visible = false;
                    pozoDescarte.Add(CartaEnMesa);
                    CartaEnMesa = cartaElegida;

                    int jugadorQueTiro = turnoActual;
                    string desc = $"{cartaElegida.Color} {cartaElegida.Valor}";

                    EnviarMensajeRed(new { Accion = "JUGAR", Carta = cartaElegida.NombreRecurso, Jugador = jugadorQueTiro });

                    string imgPozo = cartaElegida.NombreRecurso;
                    Pozo.Image = (Image)Properties.Resources.ResourceManager.GetObject(imgPozo);

                    if (PanelPadre == PanelMazoP1) mazoP1.Remove(cartaElegida);
                    else if (PanelPadre == PanelMazoP2) mazoP2.Remove(cartaElegida);

                    PicClickeado.Dispose();

                    bool pierdeTurno = false;
                    int oponente = (turnoActual == 1) ? 2 : 1;
                    string valorSTR = cartaElegida.Valor.ToString();

                    if (valorSTR == "MasDos")
                    {
                        CastigarJugador(oponente, 2);
                        pierdeTurno = true;
                        if (oponente == 1) MessageBox.Show($"Toma 2 {P1}");
                        else MessageBox.Show($"Toma 2 {P2}");
                    }
                    if (valorSTR == "Salto" || valorSTR == "Reversa")
                    {
                        pierdeTurno = true;
                        if (oponente == 1) MessageBox.Show($"Vuelves a tirar {P2}");
                        else MessageBox.Show($"Vuelves a tirar {P1}");
                    }
                    if (cartaElegida.Color == "Comodin")
                    {
                        if (valorSTR == "MasCuatro")
                        {
                            CastigarJugador(oponente, 4);
                            pierdeTurno = true;
                            if (oponente == 1) MessageBox.Show($"Toma 4, {P1}");
                            else MessageBox.Show($"Toma 4, {P2}");
                        }
                        FormEligeColor selector = new FormEligeColor();
                        selector.ShowDialog();
                        CambioColor = true;
                        color = selector.ColorElegidoTranslate;
                        CartaEnMesa.Color = selector.ColorElegido;
                        MessageBox.Show($"Cambio a {CartaEnMesa.Color}");
                    }

                    if (!pierdeTurno)
                    {
                        turnoActual = (turnoActual == 1) ? 2 : 1;
                        indicadorTurno();
                    }

                    EnviarMensajeRed(new { Accion = "CAMBIO_TURNO", Turno = turnoActual });

                    if (turnoActual == 1 && CambioColor == true)
                    {
                        ColorCambioP1.Visible = true;
                        ColorCambioP1.BackColor = Color.FromName(color);
                    }
                    else if (turnoActual == 2 && CambioColor == true)
                    {
                        ColorCambioP2.Visible = true;
                        ColorCambioP2.BackColor = Color.FromName(color);
                    }

                    if (esHost) await RegistrarMovimientoApi(jugadorQueTiro, "jugar_carta", desc); // Solo Host guarda en BD

                    List<Carta> manoDelQueTiro = (jugadorQueTiro == 1) ? mazoP1 : mazoP2;
                    if (manoDelQueTiro.Count == 0)
                    {
                        if (esHost) await FinalizarPartidaApi(jugadorQueTiro); // Solo Host guarda en BD

                        PanelMazoP1.Enabled = false;
                        PanelMazoP2.Enabled = false;
                        MazoRobar.Enabled = false;
                        sonido("Ganar2");
                        if (jugadorQueTiro == 1) MessageBox.Show($"¡Felixidades {P1}, has ganado brochacho!");
                        else MessageBox.Show($"¡Felixidades {P2}, has ganado brochacho!");
                        HayGanador = true;
                        if (menuPrincipal != null)
                        {
                            this.Close();
                            efecto.Stop();
                        }
                    }
                }
                else
                {
                    sonido("CartaNoValida");
                    MessageBox.Show("No puedes jugar esta carta brochaho, debe coincidir el color o el valor!");
                }
            }
        }

        private void Pozo_Click(object sender, EventArgs e) { }
        private void PanelMazoP2_Paint(object sender, PaintEventArgs e) { }
        private void PanelMazoP1_Paint(object sender, PaintEventArgs e) { }

        private async void MazoRobar_Click(object sender, EventArgs e)
        {
            if (!JuegoIniciado) return;

            Carta nuevaCarta = mazoJuego.RobarCarta(pozoDescarte);
            if (nuevaCarta != null)
            {
                int jugador = turnoActual;
                if (turnoActual == 1) { mazoP1.Insert(0, nuevaCarta); MostrarMazoJugador(mazoP1, PanelMazoP1); }
                else { mazoP2.Insert(0, nuevaCarta); MostrarMazoJugador(mazoP2, PanelMazoP2); }

                EnviarMensajeRed(new { Accion = "ROBAR", Jugador = jugador });

                if (esHost) await RegistrarMovimientoApi(jugador, "robar_carta", $"{nuevaCarta.Color} {nuevaCarta.Valor}"); // Solo Host guarda en BD
            }
            else MessageBox.Show("No hay más cartas en el mazo para robar.");
        }

        private void CastigarJugador(int Jugador, int cantidad)
        {
            for (int i = 0; i < cantidad; i++)
            {
                sonido($"TomaCartasMas{cantidad}");
                Carta castigo = mazoJuego.RobarCarta(pozoDescarte);
                if (castigo != null)
                {
                    if (Jugador == 1) mazoP1.Add(castigo);
                    else mazoP2.Add(castigo);
                }
            }
            if (Jugador == 1) MostrarMazoJugador(mazoP1, PanelMazoP1);
            else MostrarMazoJugador(mazoP2, PanelMazoP2);
        }

        public void sonido(string sonido)
        {
            try
            {
                var stream = Properties.Resources.ResourceManager.GetStream(sonido);
                efecto = new System.Media.SoundPlayer(stream);
                efecto.Play();
            }
            catch (Exception ex) { MessageBox.Show("No se pudo reproducir el sonido de castigo: " + ex.Message); }
        }

        public void sonidoFondo(string sonido)
        {
            try
            {
                var stream = Properties.Resources.ResourceManager.GetStream(sonido);
                SoundFondo = new System.Media.SoundPlayer(stream);
                SoundFondo.PlayLooping();
            }
            catch (Exception ex) { MessageBox.Show("No se pudo reproducir el sonido de fondo: " + ex.Message); }
        }

        private void indicadorTurno()
        {
            if (turnoActual == 1)
            {
                P1_Nick.Image = Properties.Resources.Alexis_ON;
                P1_avatar.Image = Properties.Resources.Alexis_Profile;
                P2_Nick.Image = Properties.Resources.Iván_OFF;
                P2_avatar.Image = Properties.Resources.Ivan_Profile_OFF;
            }
            else
            {
                P2_Nick.Image = Properties.Resources.Iván_ON;
                P2_avatar.Image = Properties.Resources.Ivan_Profile;
                P1_Nick.Image = Properties.Resources.Alexis_OFF;
                P1_avatar.Image = Properties.Resources.Alexis_Profile_OFF;
            }
        }

        private void btnVolver_Click(object sender, EventArgs e) { this.Close(); }
        private void label1_Click(object sender, EventArgs e) { }
    }
}