using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UNO_Game.Red
{
    public class Mensaje
    {
        public string Tipo { get; set; }
        public JToken Datos { get; set; } //Lega o mandas un mensaje en JSON

        // EscapeHtml convierte < y > en \u003C y \u003E dentro del JSON. Así el texto de un
        // jugador (por ejemplo un chat) jamás puede contener "<|EOM|>" y cortar el mensaje a la mitad.
        private static readonly JsonSerializerSettings Ajustes = new JsonSerializerSettings
        {
            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
            NullValueHandling = NullValueHandling.Ignore   // un mensaje sin datos no manda "Datos":null
        };

        public Mensaje() { }

        //JToken no es más que una representación JSON en memoria. Puede ser un objeto, un arreglo, un valor primitivo o null.

        public Mensaje(string tipo, object datos = null)
        {
            Tipo = tipo;
            //Si datos es Null pues que se quede con Null, si no, agarra el objeto y lo vuelve JToken
            Datos = datos == null ? null : JToken.FromObject(datos);
            /*Ejemplo:
             * var datos = new { nombre = "Ana", texto = "hola" };   //objeto anónimo de C#
                JToken t = JToken.FromObject(datos);   //hora es un árbol JSON

                t equivale a: {"nombre":"Ana","texto":"hola"}
            */
        }

        public string Serializar()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, Ajustes);
        }

        public static Mensaje Deserializar(string json)
        {
            var m = JsonConvert.DeserializeObject<Mensaje>(json);
            if (m != null && m.Datos != null && m.Datos.Type == JTokenType.Null)
                m.Datos = null;   // "Datos":null llega como un JValue nulo; lo volvemos null de verdad
            return m;
        }

        // Convierte Datos a una clase de nosotros:
        // Ejemplo: var d = m.Obtener<Carta>();
        public T Obtener<T>()
        {
            if (Datos == null || Datos.Type == JTokenType.Null) return default(T);
            return Datos.ToObject<T>(); //Si el dato no es Null en contenido ni en TJoken regresa la clase.
        }
    }

    /*
        Conexion: envuelve un Socket ya conectado. La usan el servidor y el cliente.
   
        TCP es un flujo de bytes sin "mensajes": una lectura puede traer medio mensaje,
        un mensaje completo o varios pegados. Por eso cada mensaje termina con <|EOM|>
        (End Of Message) y al recibir vamos juntando texto hasta encontrar esa marca.
    */
    internal class Conexion
    {
        private const string EOM = "<|EOM|>";
        private const int MAX_TEXTO = 1000000;   // 1 MB maximo, si la cadena sigue (Porque no ha llegado a EOM, para de buscar

        private readonly Socket _socket;
        private readonly byte[] _buffer = new byte[1024]; 
        private readonly char[] _chars = new char[Encoding.UTF8.GetMaxCharCount(1024)];

        private readonly Decoder _decodificador = new UTF8Encoding(false).GetDecoder();
        private readonly StringBuilder _texto = new StringBuilder();

        private readonly SemaphoreSlim _bloqueoEscritura = new SemaphoreSlim(1, 1); //Evita que dos hilos manden el mismo mensaje a la vez

        public int Id { get; set; }

        public Conexion(Socket socket)
        {
            _socket = socket;
            _socket.NoDelay = true;   // manda los mensajes al instante, sin esperar a juntar más
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        }

        public async Task EnviarAsync(Mensaje m)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(m.Serializar() + EOM);

            // El semáforo evita que dos mensajes se mezclen si se envían a la vez
            await _bloqueoEscritura.WaitAsync();
            try
            {
                int enviados = 0; //ENvía mensajes por partes 
                while (enviados < bytes.Length)   // SendAsync puede mandar solo una parte
                {
                    enviados += await _socket.SendAsync(
                        new ArraySegment<byte>(bytes, enviados, bytes.Length - enviados),
                        SocketFlags.None);
                }
            }
            finally { _bloqueoEscritura.Release(); }
        }

        // Regresa null si el otro lado cerró la conexión.
        // Lanza JsonException si llegó algo que no es un mensaje válido.
        public async Task<Mensaje> LeerAsync()
        {
            while (true)
            {
                // 1) ¿Ya hay un mensaje completo guardado de una lectura anterior?
                string json = ExtraerMensaje();
                if (json != null)
                {
                    var m = Mensaje.Deserializar(json);
                    if (m == null) throw new JsonException("Mensaje vacío");
                    return m;
                }

                // 2) Si no, leemos más bytes del socket
                int recibidos = await _socket.ReceiveAsync(new ArraySegment<byte>(_buffer), SocketFlags.None);
                if (recibidos == 0) return null;   // el otro lado cerró

                int n = _decodificador.GetChars(_buffer, 0, recibidos, _chars, 0);
                _texto.Append(_chars, 0, n);

                if (_texto.Length > MAX_TEXTO)
                    throw new InvalidDataException("Demasiados datos sin terminar un mensaje.");
            }
        }

        // Saca del texto acumulado todo lo que va antes del primer <|EOM|>
        private string ExtraerMensaje()
        {
            string todo = _texto.ToString();
            int i = todo.IndexOf(EOM, StringComparison.Ordinal);
            if (i < 0) return null;

            _texto.Remove(0, i + EOM.Length);
            return todo.Substring(0, i);
        }

        public void Cerrar()
        {
            try { _socket.Shutdown(SocketShutdown.Both); } catch { }
            try { _socket.Close(); } catch { }
        }
    }

    //  SERVIDOR: corre solo en la compu Mía (Iván).
    public class ServidorJuego
    {
        public const int MAX_CLIENTES = 3;

        private Socket _listener;
        private readonly Dictionary<int, Conexion> _clientes = new Dictionary<int, Conexion>();
        private readonly object _candado = new object();
        private readonly SynchronizationContext _ui = SynchronizationContext.Current;
        private bool _activo;

        public event Action<int> ClienteConectado;
        public event Action<int> ClienteDesconectado;
        public event Action<int, Mensaje> MensajeRecibido;

        public int NumClientes
        {
            get { lock (_candado) return _clientes.Count; }
        }

        public void Iniciar(int puerto)
        {
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(new IPEndPoint(IPAddress.Any, puerto));   // Any = acepta conexiones de otras PCs
                listener.Listen(10);
            }
            catch
            {
                listener.Close();
                throw;   // por ejemplo, el puerto ya está en uso
            }

            _listener = listener;
            _activo = true;
            AceptarClientesAsync();
        }

        public void Detener()
        {
            _activo = false;
            try { _listener.Close(); } catch { }

            List<Conexion> todos;
            lock (_candado)
            {
                todos = _clientes.Values.ToList();
                _clientes.Clear();
            }
            foreach (var c in todos) c.Cerrar();
        }

        public async Task EnviarAsync(int idCliente, Mensaje m)
        {
            Conexion con;
            lock (_candado) _clientes.TryGetValue(idCliente, out con);
            if (con == null) return;

            try { await con.EnviarAsync(m); }
            catch { Quitar(con); }
        }

        // A todos los clientes (opcionalmente menos a uno)
        public async Task DifundirAsync(Mensaje m, int? excepto = null)
        {
            List<Conexion> destino;
            lock (_candado) destino = _clientes.Values.Where(c => c.Id != excepto).ToList();

            foreach (var c in destino)
            {
                try { await c.EnviarAsync(m); }
                catch { Quitar(c); }
            }
        }
        private async void AceptarClientesAsync()
        {
            while (_activo)
            {
                Socket socket;
                try { socket = await _listener.AcceptAsync(); }
                catch { break; }   // el listener se cerró

                var con = new Conexion(socket);
                int id = SiguienteIdLibre();

                if (id == 0)   // sala llena
                {
                    try { await con.EnviarAsync(new Mensaje("error", new { mensaje = "La sala está llena (máximo 4 jugadores)." })); }
                    catch { }
                    con.Cerrar();
                    continue;
                }

                con.Id = id;
                lock (_candado) _clientes[id] = con;

                Publicar(() => ClienteConectado?.Invoke(id));
                AtenderClienteAsync(con);
            }
        }

        private int SiguienteIdLibre()
        {
            lock (_candado)
            {
                for (int i = 1; i <= MAX_CLIENTES; i++)
                    if (!_clientes.ContainsKey(i)) return i;
            }
            return 0;
        }

        private async void AtenderClienteAsync(Conexion con)
        {
            try
            {
                while (_activo)
                {
                    Mensaje m;
                    try { m = await con.LeerAsync(); }
                    catch (JsonException) { continue; }   // mensaje mal formado: se ignora

                    if (m == null) break;                 // el cliente se desconectó

                    int id = con.Id;
                    Publicar(() => MensajeRecibido?.Invoke(id, m));
                }
            }
            catch { /* conexión caída de golpe */
        }
            finally { Quitar(con); }
        }

        private void Quitar(Conexion con)
        {
            bool estabaRegistrado;
            lock (_candado)
            {
                Conexion actual;
                estabaRegistrado = _clientes.TryGetValue(con.Id, out actual) && actual == con;
                if (estabaRegistrado) _clientes.Remove(con.Id);
            }

            con.Cerrar();
            if (estabaRegistrado)
            {
                int id = con.Id;
                Publicar(() => ClienteDesconectado?.Invoke(id));
            }
        }

        private void Publicar(Action accion)
        {
            if (_ui != null) _ui.Post(_ => accion(), null);
            else accion();
        }
    }

    //  Clientes
    public class ClienteJuego
    {
        private Conexion _con;
        private readonly SynchronizationContext _ui = SynchronizationContext.Current;

        public event Action<Mensaje> MensajeRecibido;
        public event Action Desconectado;     // solo si el servidor se cae o te saca (no al llamar Desconectar())

        public bool Conectado
        {
            get { return _con != null; }
        }

        public async Task ConectarAsync(string ip, int puerto, int timeoutMs = 5000)
        {
            IPAddress direccion; //Es la IP del servidor, si no es una IP válida, intenta resolverla como nombre de host
            if (!IPAddress.TryParse(ip, out direccion))
            {
                var direcciones = await Dns.GetHostAddressesAsync(ip);
                direccion = direcciones.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
                if (direccion == null) throw new SocketException((int)SocketError.HostNotFound);
            }
            var ipEndPoint = new IPEndPoint(direccion, puerto);

            var socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Connect en otro hilo para poder ponerle un límite de tiempo
            var conectar = Task.Run(() => socket.Connect(ipEndPoint));

            if (await Task.WhenAny(conectar, Task.Delay(timeoutMs)) != conectar)
            {
                socket.Close();
                throw new TimeoutException("No se pudo conectar (tiempo agotado). Revisa la IP y el firewall del servidor.");
            }
            try { await conectar; }   // si la conexión fue rechazada, el error sale aquí
            catch { socket.Close(); throw; }

            _con = new Conexion(socket);
            EscucharAsync(_con);
        }

        public async Task EnviarAsync(Mensaje m)
        {
            var con = _con;
            if (con == null) throw new InvalidOperationException("No estás conectado al servidor.");
            await con.EnviarAsync(m);
        }

        public void Desconectar()
        {
            var con = _con;
            _con = null;
            if (con != null) con.Cerrar();
        }

        private async void EscucharAsync(Conexion con)
        {
            try
            {
                while (true)
                {
                    Mensaje m;
                    try { m = await con.LeerAsync(); }
                    catch (JsonException) { continue; }

                    if (m == null) break;   // el servidor cerró
                    Publicar(() => MensajeRecibido?.Invoke(m));
                }
            }
            catch { /* conexión caída */ }
            finally
            {
                con.Cerrar();
                if (_con == con)   // se cayó solita (no fue Desconectar())
                {
                    _con = null;
                    Publicar(() => Desconectado?.Invoke());
                }
            }
        }

        private void Publicar(Action accion)
        {
            if (_ui != null) _ui.Post(_ => accion(), null);
            else accion();
        }
    }
}