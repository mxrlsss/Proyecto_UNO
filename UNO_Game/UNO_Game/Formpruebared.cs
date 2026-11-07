using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using UNO_Game.Red;

namespace UNO_Game
{
    // Formulario SOLO para probar la conexión. No necesita diseñador: crea sus controles por código.
    // En tu PC: "Iniciar servidor" y luego "Conectar" (con 127.0.0.1).
    // En las PCs de tus compañeros: escriben tu IP y "Conectar".
    public class FormPruebaRed : Form
    {
        private const int PUERTO = 5000;

        private ServidorJuego servidor;
        private ClienteJuego cliente;

        private readonly TextBox txtIp = new TextBox { Text = "127.0.0.1", Width = 120 };
        private readonly TextBox txtNombre = new TextBox { Text = Environment.UserName, Width = 100 };
        private readonly Button btnServidor = new Button { Text = "Iniciar servidor", AutoSize = true };
        private readonly Button btnConectar = new Button { Text = "Conectar", AutoSize = true };
        private readonly TextBox txtMensaje = new TextBox { Dock = DockStyle.Fill };
        private readonly Button btnEnviar = new Button { Text = "Enviar", Dock = DockStyle.Right };
        private readonly TextBox txtLog = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Dock = DockStyle.Fill
        };

        public FormPruebaRed()
        {
            Text = "Prueba de red";
            ClientSize = new Size(640, 420);

            var arriba = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(5) };
            arriba.Controls.Add(new Label { Text = "IP del servidor:", AutoSize = true, Margin = new Padding(0, 6, 0, 0) });
            arriba.Controls.Add(txtIp);
            arriba.Controls.Add(new Label { Text = "Nombre:", AutoSize = true, Margin = new Padding(10, 6, 0, 0) });
            arriba.Controls.Add(txtNombre);
            arriba.Controls.Add(btnServidor);
            arriba.Controls.Add(btnConectar);

            var abajo = new Panel { Dock = DockStyle.Bottom, Height = 30 };
            abajo.Controls.Add(txtMensaje);
            abajo.Controls.Add(btnEnviar);

            Controls.Add(txtLog);   // Fill primero, luego los bordes
            Controls.Add(abajo);
            Controls.Add(arriba);

            btnServidor.Click += BtnServidor_Click;
            btnConectar.Click += BtnConectar_Click;
            btnEnviar.Click += BtnEnviar_Click;
            txtMensaje.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; BtnEnviar_Click(s, e); }
            };
            FormClosing += (s, e) =>
            {
                if (cliente != null) cliente.Desconectar();
                if (servidor != null) servidor.Detener();
            };
        }

        private void Log(string texto)
        {
            txtLog.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + texto + Environment.NewLine);
        }

        // ---------------------------------------------------------- SERVIDOR (solo tu PC)
        private void BtnServidor_Click(object sender, EventArgs e)
        {
            if (servidor != null) return;

            servidor = new ServidorJuego();   // se crea aquí, en el hilo de la interfaz
            servidor.ClienteConectado += id => Log("[Servidor] Cliente " + id + " conectado");
            servidor.ClienteDesconectado += id => Log("[Servidor] Cliente " + id + " desconectado");
            servidor.MensajeRecibido += async (id, m) =>
            {
                Log("[Servidor] Cliente " + id + " envió '" + m.Tipo + "'");
                if (m.Tipo == "chat")
                    await servidor.DifundirAsync(m);   // reenvía el chat a TODOS (incluido quien lo mandó)
            };

            try
            {
                servidor.Iniciar(PUERTO);
            }
            catch (Exception ex)
            {
                servidor = null;
                MessageBox.Show("No se pudo iniciar el servidor: " + ex.Message);
                return;
            }

            btnServidor.Enabled = false;
            Log("[Servidor] Escuchando en el puerto " + PUERTO);

            var ips = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                .Select(a => a.ToString());
            Log("[Servidor] Tus compañeros deben escribir una de estas IPs: " + string.Join(" / ", ips));
        }

        // ---------------------------------------------------------- CLIENTE (todas las PCs)
        private async void BtnConectar_Click(object sender, EventArgs e)
        {
            if (cliente != null && cliente.Conectado) return;

            cliente = new ClienteJuego();
            cliente.MensajeRecibido += m =>
            {
                if (m.Tipo == "chat")
                    Log((string)m.Datos["nombre"] + ": " + (string)m.Datos["texto"]);
                else if (m.Tipo == "error")
                    Log("[Error del servidor] " + (string)m.Datos["mensaje"]);
                else
                    Log("[Cliente] Mensaje '" + m.Tipo + "'");
            };
            cliente.Desconectado += () =>
            {
                Log("[Cliente] Se perdió la conexión con el servidor");
                btnConectar.Enabled = true;
            };

            btnConectar.Enabled = false;
            try
            {
                Log("[Cliente] Conectando a " + txtIp.Text.Trim() + ":" + PUERTO + "...");
                await cliente.ConectarAsync(txtIp.Text.Trim(), PUERTO);
                Log("[Cliente] ¡Conectado!");
            }
            catch (Exception ex)
            {
                Log("[Cliente] No se pudo conectar: " + ex.Message);
                btnConectar.Enabled = true;
            }
        }

        private async void BtnEnviar_Click(object sender, EventArgs e)
        {
            if (cliente == null || !cliente.Conectado)
            {
                Log("Primero conéctate al servidor.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMensaje.Text)) return;

            try
            {
                await cliente.EnviarAsync(new Mensaje("chat", new { nombre = txtNombre.Text, texto = txtMensaje.Text }));
                txtMensaje.Clear();
            }
            catch (Exception ex)
            {
                Log("[Cliente] No se pudo enviar: " + ex.Message);
            }
        }
    }
}