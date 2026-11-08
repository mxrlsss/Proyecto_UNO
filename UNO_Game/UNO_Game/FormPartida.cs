using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace UNO_Game
{
    // Aquí se carga el juego, barajean cartas, etc. Se puede acceder al menú principal desde aquí.
    public partial class FormPartida : Form
    {
        // Crea la conexión entre la URL y la base de datos
        private static readonly HttpClient http = new HttpClient { BaseAddress = new Uri("http://localhost:8000/") };

        private Form1 menuPrincipal; // Guarda una referencia al menú
        private Mazo mazoJuego;
        private List<Carta> mazoP1;
        private List<Carta> mazoP2;
        private List<Carta> mazoP3;
        private List<Carta> pozoDescarte = new List<Carta>();
        private Carta CartaEnMesa;

        // IP de conexión adaptada del código del compañero
        private const string IP_ESTATICA_HOST = "192.168.100.8";

        private bool MusicaSilencio = false;
        string P1 = "Jugador 1";
        string P2 = "Jugador 2";
        string P3 = "Jugador 3";
        public int turnoActual;
        private System.Media.SoundPlayer SoundFondo;
        System.Media.SoundPlayer efecto;
        bool HayGanador = false;
        private bool esHost;
        public int IdLocal;

        // Variables de Red (Sockets Puros)
        private TcpListener servidorListener;
        private TcpClient clienteSocket;
        private NetworkStream streamRed;
        private NetworkStream streamP2;
        private NetworkStream streamP3;

        private bool sentidoHorario = true;
        private bool dijoUNO = false;
        private bool JuegoIniciado = false; // Lógica de Sala de Espera

        public FormPartida()
        {
            InitializeComponent();
        }

        public FormPartida(Form1 menu, bool host)
        {
            InitializeComponent();
            this.menuPrincipal = menu;
            this.esHost = host;
            this.FormClosed += FormPartida_VentanaCerrada;
            this.FormClosing += FormPartida_FormClosing;
        }

        private void ActualizarInterfazVisual() { }

        private void btnVolverMenu_Click(object sender, EventArgs e)
        {
            if (btnVolver.Visible == false) btnVolver.Visible = true;
            else btnVolver.Visible = false;
        }

        private void FormPartida_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (streamRed != null) streamRed.Close();
                if (streamP2 != null) streamP2.Close();
                if (streamP3 != null) streamP3.Close();
                if (clienteSocket != null) clienteSocket.Close();
                if (servidorListener != null) servidorListener.Stop();
            }
            catch { }

            if (!HayGanador)
            {
                var respuesta = MessageBox.Show(
               "¿Seguro que quieres salir de la partida?",
               "Volver al menu de inicio",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Question);

                if (respuesta == DialogResult.No) e.Cancel = true;
            }
        }

        private void FormPartida_VentanaCerrada(object sender, FormClosedEventArgs e)
        {
            if (menuPrincipal != null) menuPrincipal.Show();
        }

        // CARGA INICIAL: Muestra la Sala de Espera bloqueando controles
        private async void FormPartida_Load(object sender, EventArgs e)
        {
            PanelMazoP2.Enabled = false; // Único panel visual del jugador local
            MazoRobar.Enabled = false;
            Pozo.Enabled = false;
            this.Text = "UNO - Sala de Espera...";

            if (esHost) await IniciarServidor3Jugadores();
            else await ConectarAlHost();
        }

        // LÓGICA DE CONEXIÓN HOST
        private async Task IniciarServidor3Jugadores()
        {
            try
            {
                IdLocal = 1;
                servidorListener = new TcpListener(IPAddress.Any, 11000);
                servidorListener.Start();

                this.Text = "UNO - Esperando al Jugador 2...";
                TcpClient invitado1 = await servidorListener.AcceptTcpClientAsync();
                streamP2 = invitado1.GetStream();

                this.Text = "UNO - Esperando al Jugador 3...";
                TcpClient invitado2 = await servidorListener.AcceptTcpClientAsync();
                streamP3 = invitado2.GetStream();

                this.Text = "UNO - ¡Todos conectados! Iniciando...";
                await Task.Delay(1000);

                IniciarJuegoReal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear la sala: " + ex.Message);
            }
        }

        // LÓGICA DE CONEXIÓN CLIENTE
        private async Task ConectarAlHost()
        {
            try
            {
                clienteSocket = new TcpClient();
                await clienteSocket.ConnectAsync(IP_ESTATICA_HOST, 11000);
                streamRed = clienteSocket.GetStream();

                this.Text = "UNO - ¡Conectado! Esperando a que el Host inicie...";
                _ = EscucharMensajesDeRed(streamRed);
            }
            catch
            {
                MessageBox.Show("No se pudo conectar. Asegúrate de que el Host esté esperando.");
                this.Close();
            }
        }

        // ARRANQUE REAL DE LA PARTIDA (Sólo el Host genera las cartas)
        private async void IniciarJuegoReal()
        {
            JuegoIniciado = true;
            this.Text = "UNO - En Partida";
            Pozo.Enabled = true;
            PanelMazoP2.Enabled = true;
            MazoRobar.Enabled = true;

            if (esHost)
            {
                Random rand_turno = new Random();
                turnoActual = rand_turno.Next(1, 4); // 1, 2 o 3

                mazoJuego = new Mazo();
                mazoJuego.Barajar();
                mazoP1 = mazoJuego.RepartirMazo(7);
                mazoP2 = mazoJuego.RepartirMazo(7);
                mazoP3 = mazoJuego.RepartirMazo(7);

                CartaEnMesa = mazoJuego.RobarCarta();
                if (CartaEnMesa.Color == "Comodin" || CartaEnMesa.Valor.ToString() == "Salto" || CartaEnMesa.Valor.ToString() == "Reversa" || CartaEnMesa.Valor.ToString() == "MasDos")
                {
                    do { CartaEnMesa = mazoJuego.RobarCarta(); }
                    while (CartaEnMesa.Color == "Comodin" || CartaEnMesa.Valor.ToString() == "Salto" || CartaEnMesa.Valor.ToString() == "Reversa" || CartaEnMesa.Valor.ToString() == "MasDos");
                }

                string imgPozo_correcta = $"{CartaEnMesa.Color}{CartaEnMesa.Valor.ToString()}";
                Pozo.Image = (Image)Properties.Resources.ResourceManager.GetObject(imgPozo_correcta);

                // Enviar manos iniciales
                string mensajeCartasP2 = "MANO:2|" + string.Join(",", mazoP2.Select(c => $"{c.Color},{c.Valor},{c.NombreRecurso}"));
                byte[] datosP2 = Encoding.UTF8.GetBytes(mensajeCartasP2);
                await streamP2.WriteAsync(datosP2, 0, datosP2.Length);

                string mensajeCartasP3 = "MANO:3|" + string.Join(",", mazoP3.Select(c => $"{c.Color},{c.Valor},{c.NombreRecurso}"));
                byte[] datosP3 = Encoding.UTF8.GetBytes(mensajeCartasP3);
                await streamP3.WriteAsync(datosP3, 0, datosP3.Length);

                // Escuchar a los invitados
                _ = EscucharMensajesDeRed(streamP2);
                _ = EscucharMensajesDeRed(streamP3);

                MostrarMazoJugador(mazoP1, PanelMazoP2); // Se dibuja la mano del Host en su panel local

                List<string> jugadoresPartida = new List<string> { P1, P2, P3 };
                // await CrearPartidaApi(jugadoresPartida); // Descomentar cuando la API esté enlazada
            }
        }

        private void AvanzarTurno()
        {
            if (sentidoHorario)
            {
                turnoActual++;
                if (turnoActual > 3) turnoActual = 1;
            }
            else
            {
                turnoActual--;
                if (turnoActual < 1) turnoActual = 3;
            }
        }

        private int CalcularSiguienteTurno()
        {
            int siguiente = turnoActual;
            if (sentidoHorario)
            {
                siguiente++;
                if (siguiente > 3) siguiente = 1;
            }
            else
            {
                siguiente--;
                if (siguiente < 1) siguiente = 3;
            }
            return siguiente;
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

            if (turnoActual != IdLocal)
            {
                sonido("CartaNoValida");
                MessageBox.Show("¡Esperad Brochaho! Aún no es tu turno.");
                return;
            }

            Carta cartaElegida = PicClickeado.Tag as Carta;

            if (cartaElegida != null)
            {
                if (cartaElegida.Color == CartaEnMesa.Color || cartaElegida.Valor == CartaEnMesa.Valor || cartaElegida.Color == "Comodin")
                {
                    pozoDescarte.Add(CartaEnMesa);
                    CartaEnMesa = cartaElegida;

                    int jugadorQueTiro = turnoActual;
                    string desc = $"{cartaElegida.Color} {cartaElegida.Valor}";

                    string imgPozo = cartaElegida.NombreRecurso;
                    Pozo.Image = (Image)Properties.Resources.ResourceManager.GetObject(imgPozo);

                    if (IdLocal == 1) mazoP1.Remove(cartaElegida);
                    else if (IdLocal == 2) mazoP2.Remove(cartaElegida);
                    else if (IdLocal == 3) mazoP3.Remove(cartaElegida);

                    PicClickeado.Dispose();

                    bool pierdeTurno = false;
                    int oponente = CalcularSiguienteTurno();
                    string valorSTR = cartaElegida.Valor.ToString();

                    if (valorSTR == "MasDos")
                    {
                        _ = AplicarCastigo(oponente, 2);
                        pierdeTurno = true;
                        if (oponente == 1) MessageBox.Show($"Toma 2 {P1}");
                        else if (oponente == 2) MessageBox.Show($"Toma 2 {P2}");
                        else if (oponente == 3) MessageBox.Show($"Toma 2 {P3}");
                    }

                    if (valorSTR == "Reversa")
                    {
                        pierdeTurno = true;
                        sentidoHorario = !sentidoHorario;
                        AvanzarTurno();
                        MessageBox.Show($"¡Salto! Turno de P{turnoActual}");
                    }
                    else if (valorSTR == "Salto")
                    {
                        pierdeTurno = true;
                        if (oponente == 1) MessageBox.Show($"Vuelves a tirar {P2}");
                        else MessageBox.Show($"Vuelves a tirar {P1}");
                    }

                    if (cartaElegida.Color == "Comodin")
                    {
                        if (valorSTR == "MasCuatro")
                        {
                            _ = AplicarCastigo(oponente, 4);
                            pierdeTurno = true;
                            if (oponente == 1) MessageBox.Show($"Toma 4, {P1}");
                            else if (oponente == 2) MessageBox.Show($"Toma 4, {P2}");
                            else if (oponente == 3) MessageBox.Show($"Toma 4, {P3}");
                        }

                        FormEligeColor selector = new FormEligeColor();
                        selector.ShowDialog();
                        CambioColor = true;
                        color = selector.ColorElegidoTranslate;

                        CartaEnMesa.Color = selector.ColorElegido;
                        MessageBox.Show($"Cambio a {CartaEnMesa.Color}");
                    }

                    if (!pierdeTurno) AvanzarTurno();

                    try
                    {
                        string mensajeAccion = $"JUGAR:{cartaElegida.Color},{cartaElegida.Valor},TURNO,{turnoActual}";
                        if (CambioColor) mensajeAccion += $",COLOR,{color}";
                        byte[] datosAccion = Encoding.UTF8.GetBytes(mensajeAccion);

                        if (esHost)
                        {
                            if (streamP2 != null) await streamP2.WriteAsync(datosAccion, 0, datosAccion.Length);
                            if (streamP3 != null) await streamP3.WriteAsync(datosAccion, 0, datosAccion.Length);
                        }
                        else
                        {
                            await streamRed.WriteAsync(datosAccion, 0, datosAccion.Length);
                        }
                    }
                    catch { }

                    // if (esHost) await RegistrarMovimientoApi(jugadorQueTiro, "jugar_carta", desc); // Descomentar cuando la API esté lista

                    List<Carta> manoDelQueTiro = mazoP1;
                    if (IdLocal == 2) manoDelQueTiro = mazoP2;
                    else if (IdLocal == 3) manoDelQueTiro = mazoP3;

                    if (manoDelQueTiro.Count == 1)
                    {
                        if (dijoUNO) dijoUNO = false;
                        else
                        {
                            MessageBox.Show("¡Se te olvidó gritar UNO! Tomas 2 cartas de castigo brochacho.");
                            _ = AplicarCastigo(IdLocal, 2);
                        }
                    }

                    if (manoDelQueTiro.Count == 0)
                    {
                        // if (esHost) await FinalizarPartidaApi(jugadorQueTiro);
                        PanelMazoP2.Enabled = false;
                        MazoRobar.Enabled = false;
                        sonido("Ganar2");
                        MessageBox.Show($"¡Felicidades, Jugador {IdLocal} ha ganado brochacho!");
                        HayGanador = true;
                        if (menuPrincipal != null)
                        {
                            this.Close();
                            efecto?.Stop();
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

        private async void MazoRobar_Click(object sender, EventArgs e)
        {
            if (!JuegoIniciado) return;

            if (esHost)
            {
                Carta nuevaCarta = mazoJuego.RobarCarta(pozoDescarte);
                if (nuevaCarta != null)
                {
                    mazoP1.Add(nuevaCarta);
                    MostrarMazoJugador(mazoP1, PanelMazoP2);
                    // await RegistrarMovimientoApi(turnoActual, "robar_carta", $"{nuevaCarta.Color} {nuevaCarta.Valor}");
                }
                else MessageBox.Show("No hay más cartas en el mazo para robar.");
            }
            else
            {
                try
                {
                    string msjPeticion = "PETICION:ROBAR";
                    byte[] datos = Encoding.UTF8.GetBytes(msjPeticion);
                    await streamRed.WriteAsync(datos, 0, datos.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al pedir carta al host: " + ex.Message);
                }
            }
        }

        private async Task AplicarCastigo(int victima, int cantidad)
        {
            if (!esHost)
            {
                string msj = $"CASTIGAR:{victima},{cantidad}";
                byte[] datos = Encoding.UTF8.GetBytes(msj);
                await streamRed.WriteAsync(datos, 0, datos.Length);
                return;
            }

            for (int i = 0; i < cantidad; i++)
            {
                Carta castigo = mazoJuego.RobarCarta(pozoDescarte);
                if (castigo != null)
                {
                    if (victima == 1)
                    {
                        this.Invoke((MethodInvoker)delegate { mazoP1.Add(castigo); });
                    }
                    else
                    {
                        string msj = $"TOMA:{castigo.Color},{castigo.Valor},{castigo.NombreRecurso}";
                        byte[] datos = Encoding.UTF8.GetBytes(msj);
                        if (victima == 2 && streamP2 != null) await streamP2.WriteAsync(datos, 0, datos.Length);
                        else if (victima == 3 && streamP3 != null) await streamP3.WriteAsync(datos, 0, datos.Length);
                    }
                }
            }
            if (victima == 1)
            {
                this.Invoke((MethodInvoker)delegate { MostrarMazoJugador(mazoP1, PanelMazoP2); });
            }
        }

        public void sonido(string sonido)
        {
            try
            {
                var streamAudio = Properties.Resources.ResourceManager.GetStream(sonido);
                efecto = new System.Media.SoundPlayer(streamAudio);
                efecto.Play();
            }
            catch (Exception ex) { }
        }

        public void sonidoFondo(string sonido)
        {
            try
            {
                var streamAudio = Properties.Resources.ResourceManager.GetStream(sonido);
                SoundFondo = new System.Media.SoundPlayer(streamAudio);
                SoundFondo.PlayLooping();
            }
            catch (Exception ex) { }
        }

        private void btnVolver_Click(object sender, EventArgs e) { this.Close(); }
        private void label1_Click(object sender, EventArgs e) { }

        // MEGA-BUCLE CENTRAL DE RED (Para 3 Jugadores)
        private async Task EscucharMensajesDeRed(NetworkStream streamActivo)
        {
            byte[] buffer = new byte[4096];
            try
            {
                while (streamActivo != null)
                {
                    int bytesLeidos = await streamActivo.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesLeidos == 0) break;

                    string mensajeRecibido = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);

                    if (mensajeRecibido.StartsWith("MANO:"))
                    {
                        string contenido = mensajeRecibido.Substring(5);
                        string[] partesSeparadas = contenido.Split('|');

                        IdLocal = int.Parse(partesSeparadas[0]);
                        string cartasStr = partesSeparadas[1];

                        List<Carta> miManoRecibida = new List<Carta>();
                        string[] items = cartasStr.Split(',');

                        for (int i = 0; i < items.Length; i += 3)
                        {
                            if (i + 2 < items.Length) miManoRecibida.Add(new Carta(items[i], items[i + 1], items[i + 2]));
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            JuegoIniciado = true;
                            this.Text = "UNO - En Partida";
                            Pozo.Enabled = true;
                            PanelMazoP2.Enabled = true;
                            MazoRobar.Enabled = true;

                            if (IdLocal == 2)
                            {
                                mazoP2 = miManoRecibida;
                                MostrarMazoJugador(mazoP2, PanelMazoP2);
                            }
                            else if (IdLocal == 3)
                            {
                                mazoP3 = miManoRecibida;
                                MostrarMazoJugador(mazoP3, PanelMazoP2);
                            }
                        });
                    }
                    else if (mensajeRecibido.StartsWith("JUGAR:"))
                    {
                        // BROADCAST: Si el Host recibe un movimiento, le pasa el chisme a la otra computadora
                        if (esHost)
                        {
                            byte[] reenviar = Encoding.UTF8.GetBytes(mensajeRecibido);
                            if (streamActivo == streamP2 && streamP3 != null) await streamP3.WriteAsync(reenviar, 0, reenviar.Length);
                            else if (streamActivo == streamP3 && streamP2 != null) await streamP2.WriteAsync(reenviar, 0, reenviar.Length);
                        }

                        string contenido = mensajeRecibido.Substring(6);
                        string[] partes = contenido.Split(',');

                        string colorRival = partes[0];
                        string valorRival = partes[1];
                        int turnoRecibido = turnoActual;
                        if (partes.Length >= 4 && partes[2] == "TURNO") int.TryParse(partes[3], out turnoRecibido);

                        this.Invoke((MethodInvoker)delegate
                        {
                            string imgPozoRival = $"{colorRival}{valorRival}";
                            Pozo.Image = (Image)Properties.Resources.ResourceManager.GetObject(imgPozoRival);
                            CartaEnMesa = new Carta(colorRival, valorRival, imgPozoRival);

                            if (partes.Length >= 6 && partes[4] == "COLOR")
                            {
                                CartaEnMesa.Color = partes[5];
                            }
                            turnoActual = turnoRecibido;
                        });
                    }
                    else if (mensajeRecibido.StartsWith("PETICION:ROBAR"))
                    {
                        if (esHost)
                        {
                            Carta cartaRobada = mazoJuego.RobarCarta(pozoDescarte);
                            if (cartaRobada != null)
                            {
                                string msjToma = $"TOMA:{cartaRobada.Color},{cartaRobada.Valor},{cartaRobada.NombreRecurso}";
                                byte[] datosToma = Encoding.UTF8.GetBytes(msjToma);
                                await streamActivo.WriteAsync(datosToma, 0, datosToma.Length);
                                // await RegistrarMovimientoApi(turnoActual, "robar_carta", $"{cartaRobada.Color} {cartaRobada.Valor}");
                            }
                        }
                    }
                    else if (mensajeRecibido.StartsWith("TOMA:"))
                    {
                        string[] datosCarta = mensajeRecibido.Substring(5).Split(',');
                        Carta nuevaCarta = new Carta(datosCarta[0], datosCarta[1], datosCarta[2]);

                        this.Invoke((MethodInvoker)delegate
                        {
                            if (IdLocal == 2) { mazoP2.Add(nuevaCarta); MostrarMazoJugador(mazoP2, PanelMazoP2); }
                            else if (IdLocal == 3) { mazoP3.Add(nuevaCarta); MostrarMazoJugador(mazoP3, PanelMazoP2); }
                        });
                    }
                    else if (mensajeRecibido.StartsWith("CASTIGAR:"))
                    {
                        if (esHost)
                        {
                            string[] partes = mensajeRecibido.Substring(9).Split(',');
                            _ = AplicarCastigo(int.Parse(partes[0]), int.Parse(partes[1]));
                        }
                    }
                    else if (mensajeRecibido.StartsWith("GRITO_UNO:"))
                    {
                        if (esHost)
                        {
                            byte[] reenviar = Encoding.UTF8.GetBytes(mensajeRecibido);
                            if (streamActivo == streamP2 && streamP3 != null) await streamP3.WriteAsync(reenviar, 0, reenviar.Length);
                            else if (streamActivo == streamP3 && streamP2 != null) await streamP2.WriteAsync(reenviar, 0, reenviar.Length);
                        }

                        string idGriton = mensajeRecibido.Substring(10);
                        this.Invoke((MethodInvoker)delegate
                        {
                            string nombreGriton = (idGriton == "1") ? P1 : (idGriton == "2") ? P2 : P3;
                            MessageBox.Show($"¡{nombreGriton} gritó UNO!");
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Conexión de red finalizada: " + ex.Message);
            }
        }

        private async void btnUNO_Click(object sender, EventArgs e)
        {
            if (turnoActual == IdLocal)
            {
                List<Carta> miMano = mazoP1;
                if (IdLocal == 2) miMano = mazoP2;
                else if (IdLocal == 3) miMano = mazoP3;

                if (miMano.Count == 2)
                {
                    dijoUNO = true;
                    MessageBox.Show("¡Gritaste UNO! Estás a salvo.");
                    try
                    {
                        string msj = $"GRITO_UNO:{IdLocal}";
                        byte[] datos = Encoding.UTF8.GetBytes(msj);
                        if (esHost)
                        {
                            if (streamP2 != null) await streamP2.WriteAsync(datos, 0, datos.Length);
                            if (streamP3 != null) await streamP3.WriteAsync(datos, 0, datos.Length);
                        }
                        else await streamRed.WriteAsync(datos, 0, datos.Length);
                    }
                    catch { }
                }
                else MessageBox.Show("Solo puedes gritar UNO cuando te queden 2 cartas antes de tirar.");
            }
        }
    }
}