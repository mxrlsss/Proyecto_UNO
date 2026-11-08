using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNO_Game
{
    // Aquí se carga el juego, barajean cartas, etc. Se puede acceder al menú principal desde aquí.
    public partial class FormPartida : Form
    {
        // Marca que separa un mensaje de otro (TCP es un flujo de bytes, no de mensajes)
        private const string FIN = "<EOF>";

        private Form1 menuPrincipal;
        private Mazo mazoJuego;
        private List<Carta> mazoP1;
        private List<Carta> mazoP2;
        private List<Carta> mazoP3;
        private List<Carta> pozoDescarte = new List<Carta>();
        private Carta CartaEnMesa;
        private string ColorActivo = "";   // Color vigente en la mesa (cambia con los comodines sin tocar la carta)

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
        private readonly SemaphoreSlim _bloqueoEscritura = new SemaphoreSlim(1, 1); // evita que dos mensajes se mezclen
        private bool _cerrando = false;
        private bool _desconexionAvisada = false;

        private bool sentidoHorario = true;
        private bool JuegoIniciado = false;

        // ---- Conteo de cartas y regla del UNO (el host es quien manda; los clientes solo reciben el conteo) ----
        private int[] conteoCartas = new int[4];     // índices 1..3
        private bool[] unoPendiente = new bool[4];   // el jugador se quedó con 1 carta y aún no pasa la "ventana" de UNO
        private bool[] unoGritado = new bool[4];     // el jugador ya presionó UNO

        // ---- API (solo la usa el host) ----
        private PartidaCreadaDto partidaApi;                       // null si la API no está disponible
        private Task _tareaCrearPartida = Task.CompletedTask;
        private int numeroTurnoApi = 0;
        private bool apiAvisada = false;
        private readonly SemaphoreSlim _bloqueoApi = new SemaphoreSlim(1, 1);

        // ---- Controles creados por código (no hay que tocar el diseñador) ----
        private Label lblConteo;
        private Label lblAviso;
        private Label lblColor;
        private System.Windows.Forms.Timer timerAviso;

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
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
        }

        // ====================== UTILIDADES ======================

        private void ActualizarInterfazVisual()
        {
            if (!JuegoIniciado) return;
            this.Text = $"UNO - Eres Jugador {IdLocal} | Turno de: Jugador {turnoActual}";
            RefrescarEtiquetas();
            ActualizarIndicadorColor();
        }

        private void CrearControlesExtra()
        {
            lblConteo = new Label();
            lblConteo.AutoSize = false;
            lblConteo.TextAlign = ContentAlignment.MiddleCenter;
            lblConteo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblConteo.ForeColor = Color.White;
            lblConteo.BackColor = Color.FromArgb(150, 0, 0, 0);
            lblConteo.Height = 36;
            lblConteo.Visible = false;
            this.Controls.Add(lblConteo);
            lblConteo.BringToFront();

            lblAviso = new Label();
            lblAviso.AutoSize = false;
            lblAviso.TextAlign = ContentAlignment.MiddleCenter;
            lblAviso.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblAviso.ForeColor = Color.Gold;
            lblAviso.BackColor = Color.FromArgb(170, 0, 0, 0);
            lblAviso.Height = 46;
            lblAviso.Visible = false;
            this.Controls.Add(lblAviso);
            lblAviso.BringToFront();

            // Indicador del color elegido con un comodín (oculto hasta que se juegue uno)
            lblColor = new Label();
            lblColor.AutoSize = false;
            lblColor.TextAlign = ContentAlignment.MiddleCenter;
            lblColor.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblColor.ForeColor = Color.White;
            lblColor.Size = new Size(220, 40);
            lblColor.Visible = false;
            this.Controls.Add(lblColor);
            lblColor.BringToFront();

            timerAviso = new System.Windows.Forms.Timer();
            timerAviso.Interval = 3500;
            timerAviso.Tick += (s, e) => { lblAviso.Visible = false; timerAviso.Stop(); };
        }

        // Mensaje que se ve unos segundos y NO bloquea (a diferencia de un MessageBox)
        private void Aviso(string texto)
        {
            EnUI(() =>
            {
                if (lblAviso == null) return;
                lblAviso.Text = texto;
                lblAviso.Visible = true;
                lblAviso.BringToFront();
                timerAviso.Stop();
                timerAviso.Start();
            });
        }

        // Barra de arriba: cuántas cartas tiene cada jugador y de quién es el turno
        private void RefrescarEtiquetas()
        {
            if (lblConteo == null) return;

            if (!JuegoIniciado || conteoCartas[1] + conteoCartas[2] + conteoCartas[3] == 0)
            {
                lblConteo.Visible = false;
                return;
            }

            var partes = new List<string>();
            for (int i = 1; i <= 3; i++)
            {
                string nombre = (i == IdLocal) ? "Tú" : $"J{i}";
                string flecha = (i == turnoActual) ? "▶ " : "";
                partes.Add($"{flecha}{nombre}: {conteoCartas[i]}");
            }
            lblConteo.Text = string.Join("   |   ", partes);
            lblConteo.Visible = true;
            lblConteo.BringToFront();
        }

        // Muestra "Color a tirar" solo cuando la carta en mesa es un comodín
        private void ActualizarIndicadorColor()
        {
            if (lblColor == null) return;

            if (!JuegoIniciado || CartaEnMesa == null || CartaEnMesa.Color != "Comodin" || string.IsNullOrEmpty(ColorActivo))
            {
                lblColor.Visible = false;
                return;
            }

            Color fondo;
            switch (ColorActivo)
            {
                case "Rojo": fondo = Color.FromArgb(220, 30, 30); break;
                case "Verde": fondo = Color.FromArgb(40, 160, 60); break;
                case "Azul": fondo = Color.FromArgb(30, 110, 220); break;
                case "Amarillo": fondo = Color.FromArgb(240, 200, 0); break;
                default: fondo = Color.DimGray; break;
            }

            lblColor.BackColor = fondo;
            lblColor.ForeColor = (ColorActivo == "Amarillo") ? Color.Black : Color.White;
            lblColor.Text = "Color a tirar: " + ColorActivo;
            lblColor.Visible = true;
            lblColor.BringToFront();
        }

        // Mantiene todo visible: panel de cartas, botón UNO y barras de arriba, según el tamaño real de la ventana
        private void AcomodarPanelMano()
        {
            int margen = 10;
            int ancho = Math.Min(958, this.ClientSize.Width - 2 * margen);
            int alto = 162;

            PanelMazoP2.Width = ancho;
            PanelMazoP2.Height = alto;
            PanelMazoP2.Left = (this.ClientSize.Width - ancho) / 2;
            PanelMazoP2.Top = this.ClientSize.Height - alto - margen;
            PanelMazoP2.BringToFront();

            if (lblColor != null && Pozo != null)
            {
                lblColor.Left = Pozo.Left + (Pozo.Width - lblColor.Width) / 2;
                lblColor.Top = Pozo.Bottom + 8;
            }

            // Botón UNO: encima del panel, a la derecha
            if (btnUNO != null)
            {
                if (btnUNO.Width < 80 || btnUNO.Height < 40) btnUNO.Size = new Size(140, 60);
                btnUNO.Visible = true;
                btnUNO.Enabled = true;
                btnUNO.Left = Math.Min(PanelMazoP2.Right - btnUNO.Width, this.ClientSize.Width - btnUNO.Width - margen);
                btnUNO.Top = PanelMazoP2.Top - btnUNO.Height - margen;
                btnUNO.BringToFront();
            }

            if (lblConteo != null)
            {
                lblConteo.Width = Math.Max(300, Math.Min(700, this.ClientSize.Width - 200));
                lblConteo.Left = (this.ClientSize.Width - lblConteo.Width) / 2;
                lblConteo.Top = 10;
            }
            if (lblAviso != null)
            {
                lblAviso.Width = Math.Max(300, Math.Min(900, this.ClientSize.Width - 200));
                lblAviso.Left = (this.ClientSize.Width - lblAviso.Width) / 2;
                lblAviso.Top = 56;
            }
        }

        // Ejecuta algo en el hilo de la interfaz, sin importar desde dónde se llame
        private void EnUI(Action accion)
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired) this.Invoke(accion);
            else accion();
        }

        private string NombreJugador(int id)
        {
            return id == 1 ? P1 : id == 2 ? P2 : P3;
        }

        private List<Carta> ObtenerMiMano()
        {
            if (IdLocal == 1) return mazoP1;
            if (IdLocal == 2) return mazoP2;
            return mazoP3;
        }

        private static bool EsCartaEspecial(Carta c)
        {
            return c.Color == "Comodin" || c.Valor == "Salto" || c.Valor == "Reversa" || c.Valor == "MasDos";
        }

        private static string ManoAString(List<Carta> mano)
        {
            return string.Join(",", mano.Select(c => $"{c.Color},{c.Valor},{c.NombreRecurso}"));
        }

        // JUGAR:color,valor,recurso,turnoSiguiente,sentido(1=horario/0=antihorario),colorActivo
        private string ConstruirMensajeJugar(Carta c, int turno)
        {
            return $"JUGAR:{c.Color},{c.Valor},{c.NombreRecurso},{turno},{(sentidoHorario ? 1 : 0)},{ColorActivo}";
        }

        // ====================== API (SOLO EL HOST) ======================

        private void AvisarErrorApi(string que, string detalle)
        {
            if (apiAvisada) return;   // solo avisa una vez para no llenar de ventanas
            apiAvisada = true;
            MessageBox.Show("La API falló al " + que + ":\n" + detalle +
                            "\n\nEl juego continúa sin guardar. Revisa que la API esté corriendo (puerto 8000) y que acepte esos datos.",
                            "API");
        }

        private async Task CrearPartidaApi()
        {
            try
            {
                partidaApi = await ApiHistorial.CrearPartidaAsync(new List<string> { P1, P2, P3 });
            }
            catch (Exception ex)
            {
                partidaApi = null;
                AvisarErrorApi("crear la partida", ex.Message);
            }
        }

        private async Task RegistrarApi(int jugador, string accion, string descripcion)
        {
            if (!esHost) return;

            await _tareaCrearPartida;
            if (partidaApi == null) return;

            string error = null;
            await _bloqueoApi.WaitAsync();   // los movimientos se guardan en orden
            try
            {
                int idJugadorApi = partidaApi.IdDe(NombreJugador(jugador));
                numeroTurnoApi++;
                await ApiHistorial.RegistrarMovimientoAsync(partidaApi.IdPartida, idJugadorApi, numeroTurnoApi, accion, descripcion);
            }
            catch (Exception ex) { error = ex.Message; }
            finally { _bloqueoApi.Release(); }

            if (error != null) AvisarErrorApi("registrar un movimiento", error);
        }

        private async Task FinalizarPartidaApi(int ganador)
        {
            if (!esHost) return;

            await _tareaCrearPartida;
            if (partidaApi == null) return;

            string error = null;
            await _bloqueoApi.WaitAsync();   // espera a que se guarden los movimientos pendientes
            try
            {
                await ApiHistorial.FinalizarPartidaAsync(partidaApi.IdPartida, partidaApi.IdDe(NombreJugador(ganador)));
            }
            catch (Exception ex) { error = ex.Message; }
            finally { _bloqueoApi.Release(); }

            if (error != null) AvisarErrorApi("finalizar la partida", error);
        }

        // ====================== ENVÍO DE MENSAJES ======================

        private async Task Enviar(NetworkStream stream, string mensaje)
        {
            if (stream == null) return;
            byte[] datos = Encoding.UTF8.GetBytes(mensaje + FIN);

            await _bloqueoEscritura.WaitAsync();
            try { await stream.WriteAsync(datos, 0, datos.Length); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Error al enviar: " + ex.Message); }
            finally { _bloqueoEscritura.Release(); }
        }

        // Host: manda a los dos clientes. Cliente: manda al host.
        private async Task EnviarATodos(string mensaje)
        {
            if (esHost)
            {
                await Enviar(streamP2, mensaje);
                await Enviar(streamP3, mensaje);
            }
            else
            {
                await Enviar(streamRed, mensaje);
            }
        }

        // Host: reenvía lo que mandó un cliente al otro cliente
        private async Task Retransmitir(string mensaje, NetworkStream origen)
        {
            if (streamP2 != null && streamP2 != origen) await Enviar(streamP2, mensaje);
            if (streamP3 != null && streamP3 != origen) await Enviar(streamP3, mensaje);
        }

        // ====================== LÓGICA DEL HOST: CONTEO Y UNO ======================

        // Host: manda a todos cuántas cartas tiene cada jugador
        private async Task HostEnviarConteo()
        {
            await EnviarATodos($"CONTEO:{conteoCartas[1]},{conteoCartas[2]},{conteoCartas[3]}");
            EnUI(() => RefrescarEtiquetas());
        }

        // Host: cada vez que alguien TIRA o ROBA, revisa si otro jugador se quedó con 1 carta
        // y no gritó UNO. Si no lo gritó a tiempo, se le castiga con 2 cartas.
        private async Task HostRevisarUNO(int actor)
        {
            for (int v = 1; v <= 3; v++)
            {
                if (v == actor) continue;

                bool castigar = unoPendiente[v] && !unoGritado[v] && conteoCartas[v] == 1;
                unoPendiente[v] = false;   // la "ventana" de UNO se cierra en cuanto otro actúa

                if (castigar)
                {
                    await EnviarATodos($"UNO_FALLO:{v}");
                    Aviso($"¡{NombreJugador(v)} no gritó UNO a tiempo! Toma 2 cartas.");
                    await AplicarCastigo(v, 2);
                }
            }
        }

        // Host: un jugador (el host o un cliente) acaba de tirar una carta
        private async Task HostAlJugarCarta(int actor, Carta carta)
        {
            conteoCartas[actor]--;

            if (conteoCartas[actor] == 1)
            {
                unoPendiente[actor] = true;   // tiene hasta que otro juegue para gritar UNO
            }
            else
            {
                unoPendiente[actor] = false;
                unoGritado[actor] = false;
            }

            string desc = carta.Color == "Comodin"
                ? $"Comodin {carta.Valor} (color {ColorActivo})"
                : $"{carta.Color} {carta.Valor}";
            _ = RegistrarApi(actor, "jugar_carta", desc);

            if (conteoCartas[actor] > 0) await HostRevisarUNO(actor);
            await HostEnviarConteo();
        }

        // Host: un jugador acaba de robar una carta
        private async Task HostAlRobar(int jugador, Carta carta)
        {
            conteoCartas[jugador]++;
            unoGritado[jugador] = false;
            unoPendiente[jugador] = false;

            _ = RegistrarApi(jugador, "robar_carta", $"{carta.Color} {carta.Valor}");

            await HostRevisarUNO(jugador);
            await HostEnviarConteo();
        }

        // ====================== CIERRE ======================

        private void btnVolverMenu_Click(object sender, EventArgs e)
        {
            if (btnVolver.Visible == false) btnVolver.Visible = true;
            else btnVolver.Visible = false;
        }

        private void FormPartida_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Primero se pregunta; solo si confirma (o ya terminó la partida) se cierra la red
            if (!HayGanador && !_cerrando)
            {
                var respuesta = MessageBox.Show(
                    "¿Seguro que quieres salir de la partida?",
                    "Volver al menu de inicio",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            _cerrando = true;
            try
            {
                if (timerAviso != null) timerAviso.Stop();
                if (streamRed != null) streamRed.Close();
                if (streamP2 != null) streamP2.Close();
                if (streamP3 != null) streamP3.Close();
                if (clienteSocket != null) clienteSocket.Close();
                if (servidorListener != null) servidorListener.Stop();
            }
            catch { }
        }

        private void FormPartida_VentanaCerrada(object sender, FormClosedEventArgs e)
        {
            if (menuPrincipal != null) menuPrincipal.Show();
        }

        private void ManejarDesconexion(string motivo)
        {
            if (_cerrando || HayGanador || _desconexionAvisada || this.IsDisposed) return;
            _desconexionAvisada = true;

            MessageBox.Show(motivo);
            _cerrando = true;   // para que FormClosing no vuelva a preguntar
            this.Close();
        }

        // ====================== CARGA / CONEXIÓN ======================

        private async void FormPartida_Load(object sender, EventArgs e)
        {
            PanelMazoP2.Enabled = false;
            MazoRobar.Enabled = false;
            Pozo.Enabled = false;
            this.Text = "UNO - Sala de Espera...";

            PanelMazoP2.Anchor = AnchorStyles.Bottom;
            PanelMazoP2.AutoScroll = true;
            PanelMazoP2.WrapContents = false;

            CrearControlesExtra();
            AcomodarPanelMano();
            this.Resize += (s, ev) => AcomodarPanelMano();

            if (esHost) await IniciarServidor3Jugadores();
            else await ConectarAlHost();
        }

        private async Task IniciarServidor3Jugadores()
        {
            try
            {
                IdLocal = 1;
                servidorListener = new TcpListener(IPAddress.Any, 11000);
                servidorListener.Start();

                EnUI(() => this.Text = "UNO - Esperando al Jugador 2...");
                TcpClient invitado1 = await servidorListener.AcceptTcpClientAsync();
                invitado1.NoDelay = true;
                streamP2 = invitado1.GetStream();

                EnUI(() => this.Text = "UNO - Esperando al Jugador 3...");
                TcpClient invitado2 = await servidorListener.AcceptTcpClientAsync();
                invitado2.NoDelay = true;
                streamP3 = invitado2.GetStream();

                EnUI(() => this.Text = "UNO - ¡Todos conectados! Iniciando...");
                await Task.Delay(1000);

                await IniciarJuegoReal();
            }
            catch (Exception ex)
            {
                if (_cerrando) return;
                MessageBox.Show("Error al crear la sala: " + ex.Message);
            }
        }

        private async Task ConectarAlHost()
        {
            try
            {
                clienteSocket = new TcpClient();
                clienteSocket.NoDelay = true;
                await clienteSocket.ConnectAsync(IP_ESTATICA_HOST, 11000);
                streamRed = clienteSocket.GetStream();

                EnUI(() => this.Text = "UNO - ¡Conectado! Esperando a que el Host inicie...");
                _ = EscucharMensajesDeRed(streamRed);
            }
            catch
            {
                if (_cerrando) return;
                MessageBox.Show("No se pudo conectar. Asegúrate de que el Host esté esperando.");
                _cerrando = true;
                this.Close();
            }
        }

        // ARRANQUE REAL DE LA PARTIDA (Sólo el Host genera las cartas)
        private async Task IniciarJuegoReal()
        {
            // Registrar la partida en la API (en segundo plano, no retrasa el juego)
            _tareaCrearPartida = CrearPartidaApi();

            Random rand_turno = new Random();
            turnoActual = rand_turno.Next(1, 4);   // 1, 2 o 3
            sentidoHorario = true;

            mazoJuego = new Mazo();
            mazoJuego.Barajar();
            mazoP1 = mazoJuego.RepartirMazo(7);
            mazoP2 = mazoJuego.RepartirMazo(7);
            mazoP3 = mazoJuego.RepartirMazo(7);

            conteoCartas[1] = mazoP1.Count;
            conteoCartas[2] = mazoP2.Count;
            conteoCartas[3] = mazoP3.Count;

            // La primera carta de la mesa no puede ser comodín ni de acción
            CartaEnMesa = mazoJuego.RobarCarta();
            while (EsCartaEspecial(CartaEnMesa))
            {
                pozoDescarte.Add(CartaEnMesa);   // se guarda para que se recicle después
                CartaEnMesa = mazoJuego.RobarCarta();
            }
            ColorActivo = CartaEnMesa.Color;

            EnUI(() =>
            {
                JuegoIniciado = true;
                Pozo.Enabled = true;
                PanelMazoP2.Enabled = true;
                PanelMazoP2.Visible = true;
                MazoRobar.Enabled = true;

                object recursoImagen = Properties.Resources.ResourceManager.GetObject(CartaEnMesa.NombreRecurso);
                if (recursoImagen != null) Pozo.Image = (Image)recursoImagen;

                MostrarMazoJugador(mazoP1, PanelMazoP2);
                ActualizarInterfazVisual();
            });

            // Manos iniciales
            await Enviar(streamP2, "MANO:2|" + ManoAString(mazoP2));
            await Enviar(streamP3, "MANO:3|" + ManoAString(mazoP3));

            // Estado de la mesa: carta, de quién es el turno, sentido y color
            string estadoMesa = ConstruirMensajeJugar(CartaEnMesa, turnoActual);
            await Enviar(streamP2, estadoMesa);
            await Enviar(streamP3, estadoMesa);

            // Conteo inicial de cartas de cada jugador
            await HostEnviarConteo();

            // Escuchar a los invitados
            _ = EscucharMensajesDeRed(streamP2);
            _ = EscucharMensajesDeRed(streamP3);
        }

        // ====================== TURNOS ======================

        private void AvanzarTurno(int pasos = 1)
        {
            for (int i = 0; i < pasos; i++)
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

        // ====================== DIBUJO DE CARTAS ======================

        private void MostrarMazoJugador(List<Carta> mazo, FlowLayoutPanel panel)
        {
            foreach (Control c in panel.Controls.Cast<Control>().ToList()) c.Dispose();
            panel.Controls.Clear();

            foreach (var carta in mazo)
            {
                PictureBox pic = new PictureBox();
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Width = 100;
                pic.Height = 130;
                pic.Margin = new Padding(5);

                object recursoImagen = Properties.Resources.ResourceManager.GetObject(carta.NombreRecurso);

                if (recursoImagen != null)
                {
                    pic.Image = (Image)recursoImagen;
                    pic.BackColor = Color.Transparent;
                }
                else
                {
                    pic.BackColor = Color.DimGray;
                }

                pic.Tag = carta;
                pic.Click += CartaJugador_Click;
                panel.Controls.Add(pic);
            }

            panel.Visible = true;
            AcomodarPanelMano();   // también hace BringToFront del panel y del botón UNO
        }

        // ====================== JUGAR UNA CARTA ======================

        private async void CartaJugador_Click(object sender, EventArgs e)
        {
            if (!JuegoIniciado || CartaEnMesa == null) return;

            PictureBox pic = sender as PictureBox;
            if (pic == null) return;

            Carta carta = pic.Tag as Carta;
            if (carta == null) return;

            if (turnoActual != IdLocal)
            {
                sonido("CartaNoValida");
                MessageBox.Show("¡Esperad Brochaho! Aún no es tu turno.");
                return;
            }

            bool valida = carta.Color == "Comodin" || carta.Color == ColorActivo || carta.Valor == CartaEnMesa.Valor;
            if (!valida)
            {
                sonido("CartaNoValida");
                MessageBox.Show("No puedes jugar esta carta brochaho, debe coincidir el color o el valor!");
                return;
            }

            // Si es comodín, se elige el color ANTES de jugarla
            string colorElegido = carta.Color;
            if (carta.Color == "Comodin")
            {
                using (FormEligeColor selector = new FormEligeColor())
                {
                    selector.ShowDialog();
                    colorElegido = selector.ColorElegido;
                }
                if (string.IsNullOrEmpty(colorElegido)) colorElegido = "Rojo";
            }

            // Quitar la carta de mi mano y ponerla en la mesa
            List<Carta> miMano = ObtenerMiMano();
            miMano.Remove(carta);
            pic.Dispose();

            pozoDescarte.Add(CartaEnMesa);
            CartaEnMesa = carta;
            ColorActivo = colorElegido;

            object recImg = Properties.Resources.ResourceManager.GetObject(carta.NombreRecurso);
            if (recImg != null) Pozo.Image = (Image)recImg;

            // Efectos de la carta
            int victima = CalcularSiguienteTurno();
            int pasos = 1;
            string aviso = null;

            switch (carta.Valor)
            {
                case "MasDos":
                    await AplicarCastigo(victima, 2);
                    pasos = 2;   // el castigado pierde su turno
                    aviso = $"¡Toma 2, {NombreJugador(victima)}!";
                    break;
                case "MasCuatro":
                    await AplicarCastigo(victima, 4);
                    pasos = 2;
                    aviso = $"¡Toma 4, {NombreJugador(victima)}!";
                    break;
                case "Salto":
                    pasos = 2;
                    aviso = $"¡Salto! {NombreJugador(victima)} pierde su turno.";
                    break;
                case "Reversa":
                    sentidoHorario = !sentidoHorario;
                    aviso = "¡Reversa!";
                    break;
            }

            AvanzarTurno(pasos);
            ActualizarInterfazVisual();

            // Avisar a los demás
            await EnviarATodos(ConstruirMensajeJugar(carta, turnoActual));

            // Si soy el host, llevo la cuenta de cartas, reviso UNO y guardo en la API
            if (esHost) await HostAlJugarCarta(1, carta);

            // Aviso que NO bloquea (para que puedas presionar UNO a tiempo)
            string mensaje = "";
            if (carta.Color == "Comodin") mensaje += $"Cambio a {colorElegido}. ";
            if (aviso != null) mensaje += aviso + $" Turno de {NombreJugador(turnoActual)}. ";
            if (miMano.Count == 1) mensaje += "¡Te queda 1 carta, presiona UNO!";
            if (mensaje.Trim().Length > 0) Aviso(mensaje.Trim());

            // Victoria
            if (miMano.Count == 0)
            {
                await EnviarATodos("GANO:" + IdLocal);
                if (esHost) _ = FinalizarPartidaApi(IdLocal);

                PanelMazoP2.Enabled = false;
                MazoRobar.Enabled = false;
                JuegoIniciado = false;
                HayGanador = true;
                sonido("Ganar2");
                MessageBox.Show($"¡Felicidades, Jugador {IdLocal} ha ganado brochacho!");
                this.Close();
                efecto?.Stop();
            }
        }

        private void Pozo_Click(object sender, EventArgs e) { }
        private void PanelMazoP2_Paint(object sender, PaintEventArgs e) { }

        private async void MazoRobar_Click(object sender, EventArgs e)
        {
            if (!JuegoIniciado) return;

            if (turnoActual != IdLocal)
            {
                sonido("CartaNoValida");
                MessageBox.Show("¡Esperad Brochaho! Aún no es tu turno.");
                return;
            }

            if (esHost)
            {
                Carta nuevaCarta = mazoJuego.RobarCarta(pozoDescarte);
                if (nuevaCarta != null)
                {
                    mazoP1.Add(nuevaCarta);
                    MostrarMazoJugador(mazoP1, PanelMazoP2);
                    await HostAlRobar(1, nuevaCarta);
                }
                else MessageBox.Show("No hay más cartas en el mazo para robar.");
            }
            else
            {
                await Enviar(streamRed, "PETICION:ROBAR");
            }
        }

        // El host reparte el castigo. Si eres cliente, se lo pides al host.
        private async Task AplicarCastigo(int victima, int cantidad)
        {
            if (!esHost)
            {
                await Enviar(streamRed, $"CASTIGAR:{victima},{cantidad}");
                return;
            }

            int dadas = 0;
            for (int i = 0; i < cantidad; i++)
            {
                Carta castigo = mazoJuego.RobarCarta(pozoDescarte);
                if (castigo == null) continue;
                dadas++;

                if (victima == 1)
                {
                    mazoP1.Add(castigo);
                }
                else
                {
                    string msj = $"TOMA:{castigo.Color},{castigo.Valor},{castigo.NombreRecurso}";
                    if (victima == 2) await Enviar(streamP2, msj);
                    else if (victima == 3) await Enviar(streamP3, msj);
                }
            }

            conteoCartas[victima] += dadas;
            unoGritado[victima] = false;
            unoPendiente[victima] = false;

            if (victima == 1)
                EnUI(() => MostrarMazoJugador(mazoP1, PanelMazoP2));

            await HostEnviarConteo();
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

        // ====================== RED: ESCUCHAR ======================

        // Acumula lo que llega hasta encontrar <EOF>; así un mensaje cortado a la mitad
        // o varios pegados en una sola lectura se procesan bien.
        private async Task EscucharMensajesDeRed(NetworkStream streamActivo)
        {
            byte[] buffer = new byte[8192];
            Decoder decodificador = new UTF8Encoding(false).GetDecoder();
            char[] chars = new char[Encoding.UTF8.GetMaxCharCount(buffer.Length)];
            StringBuilder acumulado = new StringBuilder();

            try
            {
                while (!_cerrando)
                {
                    int leidos = await streamActivo.ReadAsync(buffer, 0, buffer.Length);
                    if (leidos == 0)
                    {
                        ManejarDesconexion("El otro jugador se desconectó.");
                        return;
                    }

                    int n = decodificador.GetChars(buffer, 0, leidos, chars, 0);
                    acumulado.Append(chars, 0, n);

                    string texto = acumulado.ToString();
                    int inicio = 0;
                    int fin;
                    while ((fin = texto.IndexOf(FIN, inicio, StringComparison.Ordinal)) >= 0)
                    {
                        string paquete = texto.Substring(inicio, fin - inicio);
                        inicio = fin + FIN.Length;

                        if (paquete.Length == 0) continue;

                        try
                        {
                            await ProcesarPaquete(paquete, streamActivo);
                        }
                        catch (Exception exPaquete)
                        {
                            if (_cerrando) return;
                            string vista = paquete.Length > 80 ? paquete.Substring(0, 80) + "..." : paquete;
                            MessageBox.Show("Error procesando mensaje de red:\n" + exPaquete.Message + "\n\nPaquete: " + vista);
                        }
                    }
                    acumulado.Remove(0, inicio);
                }
            }
            catch (Exception ex)
            {
                ManejarDesconexion("Conexión de red finalizada: " + ex.Message);
            }
        }

        private async Task ProcesarPaquete(string paquete, NetworkStream origen)
        {
            if (paquete.StartsWith("MANO:"))
            {
                // MANO:id|color,valor,recurso,color,valor,recurso,...
                string[] partes = paquete.Substring(5).Split('|');
                int id = int.Parse(partes[0]);

                List<Carta> mano = new List<Carta>();
                if (partes.Length > 1 && partes[1].Length > 0)
                {
                    string[] items = partes[1].Split(',');
                    for (int i = 0; i + 2 < items.Length; i += 3)
                        mano.Add(new Carta(items[i], items[i + 1], items[i + 2]));
                }

                IdLocal = id;
                if (id == 2) mazoP2 = mano;
                else if (id == 3) mazoP3 = mano;

                EnUI(() =>
                {
                    JuegoIniciado = true;
                    Pozo.Enabled = true;
                    PanelMazoP2.Enabled = true;
                    PanelMazoP2.Visible = true;
                    MazoRobar.Enabled = true;

                    MostrarMazoJugador(mano, PanelMazoP2);
                    ActualizarInterfazVisual();
                });
            }
            else if (paquete.StartsWith("JUGAR:"))
            {
                // JUGAR:color,valor,recurso,turnoSiguiente,sentido,colorActivo
                string[] p = paquete.Substring(6).Split(',');
                if (p.Length < 6) throw new FormatException("JUGAR incompleto");

                if (esHost) await Retransmitir(paquete, origen);

                // El host guarda la carta anterior en el pozo para poder reciclar el mazo
                if (esHost && CartaEnMesa != null) pozoDescarte.Add(CartaEnMesa);

                CartaEnMesa = new Carta(p[0], p[1], p[2]);
                turnoActual = int.Parse(p[3]);
                sentidoHorario = (p[4] == "1");
                ColorActivo = p[5];

                EnUI(() =>
                {
                    object recImg = Properties.Resources.ResourceManager.GetObject(CartaEnMesa.NombreRecurso);
                    if (recImg != null) Pozo.Image = (Image)recImg;
                    ActualizarInterfazVisual();
                });

                // Host: contar la carta, revisar UNO y guardar en la API
                if (esHost)
                {
                    int actor = (origen == streamP2) ? 2 : 3;
                    await HostAlJugarCarta(actor, CartaEnMesa);
                }
            }
            else if (paquete.StartsWith("PETICION:ROBAR"))
            {
                if (!esHost) return;

                int solicitante = (origen == streamP2) ? 2 : 3;
                if (turnoActual != solicitante) return;   // no es su turno

                Carta cartaRobada = mazoJuego.RobarCarta(pozoDescarte);
                if (cartaRobada != null)
                {
                    await Enviar(origen, $"TOMA:{cartaRobada.Color},{cartaRobada.Valor},{cartaRobada.NombreRecurso}");
                    await HostAlRobar(solicitante, cartaRobada);
                }
            }
            else if (paquete.StartsWith("TOMA:"))
            {
                string[] d = paquete.Substring(5).Split(',');
                Carta nueva = new Carta(d[0], d[1], d[2]);

                List<Carta> mano = ObtenerMiMano();
                if (mano == null) return;
                mano.Add(nueva);

                EnUI(() => MostrarMazoJugador(mano, PanelMazoP2));
            }
            else if (paquete.StartsWith("CASTIGAR:"))
            {
                if (!esHost) return;

                string[] partes = paquete.Substring(9).Split(',');
                await AplicarCastigo(int.Parse(partes[0]), int.Parse(partes[1]));
            }
            else if (paquete.StartsWith("CONTEO:"))
            {
                // CONTEO:cartasJ1,cartasJ2,cartasJ3 (lo manda el host)
                string[] c = paquete.Substring(7).Split(',');
                conteoCartas[1] = int.Parse(c[0]);
                conteoCartas[2] = int.Parse(c[1]);
                conteoCartas[3] = int.Parse(c[2]);
                EnUI(() => RefrescarEtiquetas());
            }
            else if (paquete.StartsWith("GRITO_UNO:"))
            {
                int id = int.Parse(paquete.Substring(10));

                if (esHost)
                {
                    if (conteoCartas[id] <= 2) unoGritado[id] = true;
                    await Retransmitir(paquete, origen);
                }

                Aviso($"¡{NombreJugador(id)} gritó UNO!");
            }
            else if (paquete.StartsWith("UNO_FALLO:"))
            {
                int id = int.Parse(paquete.Substring(10));
                Aviso($"¡{NombreJugador(id)} no gritó UNO a tiempo! Toma 2 cartas.");
            }
            else if (paquete.StartsWith("GANO:"))
            {
                int idGanador = int.Parse(paquete.Substring(5));

                if (esHost)
                {
                    await Retransmitir(paquete, origen);
                    _ = FinalizarPartidaApi(idGanador);
                }

                JuegoIniciado = false;
                HayGanador = true;
                sonido("Ganar2");
                MessageBox.Show($"¡{NombreJugador(idGanador)} ha ganado la partida!");
                this.Close();
            }
        }

        // ====================== BOTÓN UNO ======================

        // Puedes presionarlo cuando te queda 1 carta (hasta que otro jugador tire o robe),
        // o cuando tienes 2 cartas en tu turno, antes de tirar.
        private async void btnUNO_Click(object sender, EventArgs e)
        {
            if (!JuegoIniciado) return;

            List<Carta> miMano = ObtenerMiMano();
            int cuantas = miMano == null ? -1 : miMano.Count;

            bool puede = cuantas == 1 || (cuantas == 2 && turnoActual == IdLocal);
            if (!puede)
            {
                Aviso("Solo puedes gritar UNO cuando te queda 1 carta (o 2 en tu turno, antes de tirar).");
                return;
            }

            if (esHost) unoGritado[1] = true;   // el host anota su propio UNO
            await EnviarATodos($"GRITO_UNO:{IdLocal}");
            Aviso("¡Gritaste UNO! Estás a salvo.");
        }
    }
}