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
        private List<Carta> mazoP3;
        private List<Carta> pozoDescarte = new List<Carta>();
        private Carta CartaEnMesa;
        private const string IP_ESTATICA_HOST = "192.168.1.19"; //cambiar por la IP del host en la red local
        private bool MusicaSilencio = false;
        string P1 = "";
        string P2 = "";
        string P3 = "";
        public int turnoActual; //1 para P1 y 2 para P2 y 3 para P3
        private System.Media.SoundPlayer SoundFondo;
        System.Media.SoundPlayer efecto;
        bool HayGanador = false;
        private bool esHost;

        private NetworkStream streamRed; 
        private TcpClient clienteSocket;
        private TcpListener servidorListener;
        public int IdLocal;
        private NetworkStream streamP2;
        private NetworkStream streamP3;

        private bool sentidoHorario = true; // Variable para controlar el sentido del juego

        private bool dijoUNO = false;



        public FormPartida(Form1 menu, bool host)
        {
            InitializeComponent(); 
            this.menuPrincipal = menu; //Guarda la referencia al menú principal
            this.FormClosed += FormPartida_VentanaCerrada;
            this.FormClosing += FormPartida_FormClosing;
            esHost = host;
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
            if(!HayGanador)
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

            if (esHost)
            {
                IdLocal = 1;
                Random rand_turno = new Random();
                turnoActual = rand_turno.Next(1, 4); //1 para P1 y 2 para P2 y 3 para P3 
                //indicadorTurno();

                mazoJuego = new Mazo();
                mazoJuego.Barajar();
                mazoP1 = mazoJuego.RepartirMazo(7);
                mazoP2 = mazoJuego.RepartirMazo(7);
                mazoP3 = mazoJuego.RepartirMazo(7);
                //ColorCambioP1.Visible = false;
                //ColorCambioP2.Visible = false;



                //sonidoFondo("FondoPartida");      inhablilitados porm problemas con los canalaes de audio de windows
                //btnSonido.Image = Properties.Resources.Sonido_Playing;

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

                servidorListener = new TcpListener(IPAddress.Any, 11000);
                servidorListener.Start();

                //jugador 2
                TcpClient invitado1 = await servidorListener.AcceptTcpClientAsync();
                streamP2 = invitado1.GetStream();

                string mensajeCartasP2 = "MANO:2|" + string.Join(",", mazoP2.Select(c => $"{c.Color},{c.Valor},{c.NombreRecurso}"));
                byte[] datosP2 = Encoding.UTF8.GetBytes(mensajeCartasP2);
                await streamP2.WriteAsync(datosP2, 0, datosP2.Length);
                _ = EscucharMensajesDeRed(streamP2);


                //jugador 3
                TcpClient invitado2 = await servidorListener.AcceptTcpClientAsync();
                streamP3 = invitado2.GetStream();

                string mensajeCartasP3 = "MANO:3|" + string.Join(",", mazoP3.Select(c => $"{c.Color},{c.Valor},{c.NombreRecurso}"));
                byte[] datosP3 = Encoding.UTF8.GetBytes(mensajeCartasP3);
                await streamP3.WriteAsync(datosP3, 0, datosP3.Length);
                _ = EscucharMensajesDeRed(streamP3);


                MostrarMazoJugador(mazoP1, PanelMazoP2); //se muestran en el dos porque es el de abajo 

            }

            if (!esHost)
            {
                //MostrarMazoJugador(mazoP1, PanelMazoP2);
                clienteSocket = new TcpClient();
                await clienteSocket.ConnectAsync(IP_ESTATICA_HOST, 11000);
                streamRed = clienteSocket.GetStream();
                _ = EscucharMensajesDeRed(streamRed);

            }

            List<string> jugadoresPartida = new List<string> {P1, P2, P3 };
            await CrearPartidaApi(jugadoresPartida);
        }

        private void AvanzarTurno()
        {
            if (sentidoHorario)
            {
                turnoActual++;
                if (turnoActual > 3) turnoActual = 1; // 1 -> 2 -> 3 -> 1
            }
            else
            {
                turnoActual--;
                if (turnoActual < 1) turnoActual = 3; // 1 -> 3 -> 2 -> 1
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
            bool CambioColor = false;
            var color= "";
            PictureBox PicClickeado = sender as PictureBox;
            FlowLayoutPanel PanelPadre = PicClickeado.Parent as FlowLayoutPanel;

            if (turnoActual != IdLocal)
            {
                sonido("CartaNoValida");
                MessageBox.Show("¡Esperad Brochaho! Aún no es tu turno.");
                return;
            }


            Carta cartaElegida = PicClickeado.Tag as Carta;

            if(cartaElegida != null)
            {
                //MessageBox.Show($"Carta elegida: {cartaElegida.Color} {cartaElegida.Valor}");
                if (cartaElegida.Color == CartaEnMesa.Color || cartaElegida.Valor == CartaEnMesa.Valor || cartaElegida.Color == "Comodin")
                {
                    //ColorCambioP1.Visible = false;
                    ColorCambioP2.Visible = false;
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

                    //cartas especiales 

                    bool pierdeTurno = false;
                    int oponente = CalcularSiguienteTurno();
                    string valorSTR = cartaElegida.Valor.ToString();

                    //+2
                    if (valorSTR == "MasDos")
                    {
                        _ = AplicarCastigo(oponente, 2);
                        pierdeTurno = true;
                        if (oponente == 1)
                            MessageBox.Show($"Toma 2 {P1}");
                        if(oponente == 2)
                            MessageBox.Show($"Toma 2 {P2}");
                        if(oponente == 3)
                            MessageBox.Show($"Toma 2 {P3}");
                    }

                    /*Saltos (para 1 a 1 funciona, si es de mas jugadores debe cambiarse (proximosmparciales)
                    if (valorSTR == "Salto" || valorSTR == "Reversa")
                    {
                        pierdeTurno = true;
                        if (oponente == 1)
                            MessageBox.Show($"Vuelves a tirar {P2}");
                        else
                            MessageBox.Show($"Vuelves a tirar {P1}");
                    }
                    */

                    if(valorSTR == "Reversa")
                    {
                        pierdeTurno = true;
                        sentidoHorario = !sentidoHorario; // Cambia el sentido del juego
                        AvanzarTurno();
                        MessageBox.Show($"¡Salto! Turno de P{turnoActual}");
                    }


                    //mas 4 y cambio de clor
                    if (cartaElegida.Color == "Comodin")
                    {
                        if (valorSTR == "MasCuatro")
                        {
                            _ = AplicarCastigo(oponente, 4);
                            pierdeTurno = true;
                            if (oponente == 1)
                                MessageBox.Show($"Toma 4, {P1}");
                            if (oponente == 2)
                                MessageBox.Show($"Toma 4, {P2}");
                            if(oponente == 3)
                                MessageBox.Show($"Toma 4, {P3}");
                        }

                        FormEligeColor selector = new FormEligeColor();
                        selector.ShowDialog();
                        CambioColor = true;
                        color = selector.ColorElegidoTranslate;

                        CartaEnMesa.Color = selector.ColorElegido;
                        MessageBox.Show($"Cambio a {CartaEnMesa.Color}");
                    }

                    //cierran cartas especiales

                    if (!pierdeTurno)
                    {
                        AvanzarTurno();
                    }

                    try
                    {
                        string mensajeAccion = $"JUGAR:{cartaElegida.Color},{cartaElegida.Valor},TURNO,{turnoActual}";

                        // Si fue un comodín, le pegamos el nuevo color al final del mensaje
                        if (CambioColor)
                        {
                            mensajeAccion += $",COLOR,{color}";
                        }

                        byte[] datosAccion = Encoding.UTF8.GetBytes(mensajeAccion);
                        await streamRed.WriteAsync(datosAccion, 0, datosAccion.Length);
                    }
                    catch (Exception ex)
                    {
                        // Manejo opcional de error de red
                    }

                    await RegistrarMovimientoApi(jugadorQueTiro, "jugar_carta", desc);

                    List<Carta> manoDelQueTiro = mazoP1;
                    if (IdLocal == 2) manoDelQueTiro = mazoP2;
                    else if (IdLocal == 3) manoDelQueTiro = mazoP3;

                    if (manoDelQueTiro.Count == 1)
                    {
                        if (dijoUNO)
                        {
                            dijoUNO = false; // Se salvó. Reseteamos la variable para el futuro.
                        }
                        else
                        {
                            // ¡No gritó UNO! Castigo automático de 2 cartas.
                            MessageBox.Show("¡Se te olvidó gritar UNO! Tomas 2 cartas de castigo brochacho.");
                            _ = AplicarCastigo(IdLocal, 2);
                        }
                    }

                    if (manoDelQueTiro.Count == 0)
                    {
                        await FinalizarPartidaApi(jugadorQueTiro);
                        //PanelMazoP1.Enabled = false;
                        PanelMazoP2.Enabled = false;
                        MazoRobar.Enabled = false;
                        sonido("Ganar2");
                        if (jugadorQueTiro == 1)
                            MessageBox.Show($"¡Felixidades {P1}, has ganado brochacho!");
                        else
                            MessageBox.Show($"¡Felixidades {P2}, has ganado brochacho!");
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
            if (esHost)
            {


                Carta nuevaCarta = mazoJuego.RobarCarta(pozoDescarte);

                if (nuevaCarta != null)
                {

                    mazoP1.Add(nuevaCarta);
                    MostrarMazoJugador(mazoP1, PanelMazoP2);
                    await RegistrarMovimientoApi(turnoActual, "robar_carta", $"{nuevaCarta.Color} {nuevaCarta.Valor}");

                }
                else
                {
                    MessageBox.Show("No hay más cartas en el mazo para robar.");
                }
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
                // El invitado no tiene la baraja, así que le "exige" al Host por red que castigue al oponente
                string msj = $"CASTIGAR:{victima},{cantidad}";
                byte[] datos = Encoding.UTF8.GetBytes(msj);
                await streamRed.WriteAsync(datos, 0, datos.Length);
                return;
            }

            // SI ERES EL HOST: Tú repartes la justicia y sacas las cartas
            for (int i = 0; i < cantidad; i++)
            {
                Carta castigo = mazoJuego.RobarCarta(pozoDescarte);
                if (castigo != null)
                {
                    if (victima == 1) // El Host se castiga a sí mismo localmente
                    {
                        this.Invoke((MethodInvoker)delegate { mazoP1.Add(castigo); });
                    }
                    else // Le avienta la carta por red a la víctima usando el "TOMA:"
                    {
                        string msj = $"TOMA:{castigo.Color},{castigo.Valor},{castigo.NombreRecurso}";
                        byte[] datos = Encoding.UTF8.GetBytes(msj);

                        if (victima == 2 && streamP2 != null)
                            await streamP2.WriteAsync(datos, 0, datos.Length);
                        else if (victima == 3 && streamP3 != null)
                            await streamP3.WriteAsync(datos, 0, datos.Length);
                    }
                }
            }

            // Si la víctima fue el Host, actualizamos su pantalla al terminar de recibir todas
            if (victima == 1)
            {
                this.Invoke((MethodInvoker)delegate { MostrarMazoJugador(mazoP1, PanelMazoP2); });
            }
        }


        public void sonido(string sonido) //se pasan versatilemnte 
        {
            try
            {
                var stream = Properties.Resources.ResourceManager.GetStream(sonido);

                efecto = new System.Media.SoundPlayer(stream);
                efecto.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo reproducir el sonido de castigo: " + ex.Message);
            }
        }

        //No utilzado por problemas con los canales de audio de windows
        public void sonidoFondo(string sonido) //se pasan versatilemnte 
        {

            try
            {
                var stream = Properties.Resources.ResourceManager.GetStream(sonido);

                SoundFondo = new System.Media.SoundPlayer(stream);
                SoundFondo.PlayLooping();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo reproducir el sonido de fondo: " + ex.Message);
            }
        }

        /*private void indicadorTurno()
        {
            if(turnoActual == 1)
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
        */ 
        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /*
        private void btnSonido_Click(object sender, EventArgs e)
        {
            if(!MusicaSilencio)
            {
                if(SoundFondo != null)
                {
                    SoundFondo.Stop();
                }
                MusicaSilencio = true;

                btnSonido.Image = Properties.Resources.Sonido;
            }
            else
            {
                if (SoundFondo != null)
                {
                    SoundFondo.PlayLooping();
                }
                MusicaSilencio = false;
                btnSonido.Image = Properties.Resources.Sonido_Playing;
            }
        }
        */
        private void label1_Click(object sender, EventArgs e)
        {

        }



        //bucle de escucha de mensajes de red (sockets)
        private async Task EscucharMensajesDeRed(NetworkStream streamActivo)
        {
            byte[] buffer = new byte[4096];

            try
            {
                // El bucle se queda escuchando continuamente ese "tubo" específico
                while (streamActivo != null)
                {
                    int bytesLeidos = await streamActivo.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesLeidos == 0) break; // Se cortó la conexión

                    string mensajeRecibido = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);

                    // 1. CASO: RECIBIR LA MANO INICIAL (Solo para el Invitado al arrancar)
                    if (mensajeRecibido.StartsWith("MANO:"))
                    {
                        string contenido = mensajeRecibido.Substring(5);
                        string[] partesSeparadas = contenido.Split('|');

                        IdLocal = int.Parse(partesSeparadas[0]); // ¡Atrapas tu ID de jugador (2 o 3)!
                        string cartasStr = partesSeparadas[1];     // Te quedas solo con la cadena de cartas

                        List<Carta> miManoRecibida = new List<Carta>();
                        string[] items = cartasStr.Split(',');

                        // Avanzamos de 3 en 3 por las propiedades: Color, Valor, NombreRecurso
                        for (int i = 0; i < items.Length; i += 3)
                        {
                            if (i + 2 < items.Length)
                            {
                                string color = items[i];
                                string valor = items[i + 1];
                                string recurso = items[i + 2];

                                miManoRecibida.Add(new Carta(color, valor, recurso));
                            }
                        }

                        // Actualizamos la UI de forma segura con Invoke
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (IdLocal == 2)
                            {
                                mazoP2 = miManoRecibida;
                                MostrarMazoJugador(mazoP2, PanelMazoP2); // O el panel visual que le corresponda en tu diseño
                            }
                            else if (IdLocal == 3)
                            {
                                mazoP3 = miManoRecibida;
                                MostrarMazoJugador(mazoP3, PanelMazoP2); // Asegúrate de tener un panel para el jugador 3
                            }
                        });
                    }
                    // 2. CASO: ALGUIEN TIRA UNA CARTA (Actualiza el Pozo y Turno para todos)
                    else if (mensajeRecibido.StartsWith("JUGAR:"))
                    {
                        // Formato esperado enviado por red: JUGAR:Color,Valor,TURNO:NumeroDeTurno
                        string contenido = mensajeRecibido.Substring(6);
                        string[] partes = contenido.Split(',');

                        string colorRival = partes[0];
                        string valorRival = partes[1];

                        int turnoRecibido = turnoActual;
                        if (partes.Length >= 4 && partes[2] == "TURNO")
                        {
                            int.TryParse(partes[3], out turnoRecibido);
                        }

                        // Actualizamos la pantalla de todos en la LAN de forma segura
                        this.Invoke((MethodInvoker)delegate
                        {
                            string imgPozoRival = $"{colorRival}{valorRival}";
                            Pozo.Image = (Image)Properties.Resources.ResourceManager.GetObject(imgPozoRival);

                            // 2. CRÍTICO: Actualizamos la variable local de la carta en mesa para que las reglas funcionen
                            CartaEnMesa = new Carta(colorRival, valorRival, imgPozoRival);

                            // 3. Revisamos si el mensaje traía el pasajero extra "COLOR"
                            if (partes.Length >= 6 && partes[4] == "COLOR")
                            {
                                string nuevoColorElegido = partes[5];

                                // ¡Magia! Le inyectamos el nuevo color a la carta en mesa
                                CartaEnMesa.Color = nuevoColorElegido;

                                // Opcional: Aquí puedes prender tu cuadrito de color para que el jugador lo vea
                                ColorCambioP2.Visible = true;
                                ColorCambioP2.BackColor = Color.FromName(nuevoColorElegido);
                            }

                            // 4. Actualizar el turno en curso y mover los avatares visuales
                            turnoActual = turnoRecibido;
                            //indicadorTurno();
                        });
                    }
                    else if (mensajeRecibido.StartsWith("PETICION:ROBAR"))
                    {
                        if (esHost)
                        {
                            // El Host saca la carta física de su baraja
                            Carta cartaRobada = mazoJuego.RobarCarta(pozoDescarte);

                            if (cartaRobada != null)
                            {
                                // Se la avienta de regreso por el MISMO "tubo" al invitado que la pidió
                                string msjToma = $"TOMA:{cartaRobada.Color},{cartaRobada.Valor},{cartaRobada.NombreRecurso}";
                                byte[] datosToma = Encoding.UTF8.GetBytes(msjToma);
                                await streamActivo.WriteAsync(datosToma, 0, datosToma.Length);

                                // Registramos en la BD que alguien robó (puedes pasarle el turnoActual)
                                await RegistrarMovimientoApi(turnoActual, "robar_carta", $"{cartaRobada.Color} {cartaRobada.Valor}");
                            }
                        }
                    }

                    // 4. CASO: EL HOST NOS MANDA LA CARTA QUE ROBAMOS (Solo lo ejecuta el Invitado)
                    else if (mensajeRecibido.StartsWith("TOMA:"))
                    {
                        // Parseamos la carta que nos acaba de llegar
                        string[] datosCarta = mensajeRecibido.Substring(5).Split(',');
                        string color = datosCarta[0];
                        string valor = datosCarta[1];
                        string recurso = datosCarta[2];

                        Carta nuevaCarta = new Carta(color, valor, recurso);

                        // La dibujamos de forma segura en nuestra pantalla
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (IdLocal == 2)
                            {
                                mazoP2.Add(nuevaCarta); // Agregamos al final de nuestra mano
                                MostrarMazoJugador(mazoP2, PanelMazoP2);
                            }
                            else if (IdLocal == 3)
                            {
                                mazoP3.Add(nuevaCarta); // Agregamos al final de nuestra mano
                                MostrarMazoJugador(mazoP3, PanelMazoP2);
                            }
                        });
                    }
                    else if (mensajeRecibido.StartsWith("CASTIGAR:"))
                    {
                        if (esHost)
                        {
                            string[] partes = mensajeRecibido.Substring(9).Split(',');
                            int idVictima = int.Parse(partes[0]);
                            int cartasCastigo = int.Parse(partes[1]);

                            // Disparamos el método que reparte la justicia por red
                            _ = AplicarCastigo(idVictima, cartasCastigo);
                        }
                    }
                    else if (mensajeRecibido.StartsWith("GRITO_UNO:"))
                    {
                        string idGriton = mensajeRecibido.Substring(10);

                        this.Invoke((MethodInvoker)delegate
                        {
                            // Opcional: sonido("GritoUNO"); 
                            string nombreGriton = (idGriton == "1") ? P1 : (idGriton == "2") ? P2 : P3;
                            MessageBox.Show($"¡{nombreGriton} gritó UNO!");
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Si la conexión se cierra de golpe
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

                // Solo es válido si tienes exactamente 2 cartas (a punto de tirar y quedarte con 1)
                if (miMano.Count == 2)
                {
                    dijoUNO = true;
                    MessageBox.Show("¡Gritaste UNO! Estás a salvo.");

                    // Le avisamos a los demás por red para meter presión psicológica
                    try
                    {
                        string msj = $"GRITO_UNO:{IdLocal}";
                        byte[] datos = Encoding.UTF8.GetBytes(msj);
                        await streamRed.WriteAsync(datos, 0, datos.Length);
                    }
                    catch { }
                }
                else
                {
                    MessageBox.Show("Solo puedes gritar UNO cuando te queden 2 cartas antes de tirar.");
                }
            }
        }
    }

}
