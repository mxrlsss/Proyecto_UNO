using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UNO_Game
{
    //Se modificó la clase Dato, ahora esta dentro de  Apihistorial
    public class PartidaDto
    {
        [JsonProperty("idPartida")] public int IdPartida { get; set; }
        [JsonProperty("fecha_inicio")] public DateTime FechaInicio { get; set; }
        [JsonProperty("fecha_fin")] public DateTime? FechaFin { get; set; }
        [JsonProperty("ganador")] public string Ganador { get; set; }
        [JsonProperty("jugadores")] public string Jugadores { get; set; }      
        [JsonProperty("num_jugadores")] public int NumJugadores { get; set; }
    }

    public class JugadorPartidaDto
    {
        [JsonProperty("idJugador")] public int IdJugador { get; set; }
        [JsonProperty("nombre")] public string Nombre { get; set; }
        [JsonProperty("resultado")] public string Resultado { get; set; }
    }

    public class MovimientoDto
    {
        [JsonProperty("numero_turno")] public int NumTurno { get; set; }
        [JsonProperty("jugador")] public string Jugador { get; set; }
        [JsonProperty("accion")] public string Accion { get; set; }
        [JsonProperty("descripcion")] public string Descripcion { get; set; }
        [JsonProperty("tiempo_jugada")] public DateTime TiempoJugada { get; set; }
    }

    // Respuesta al registrar una partida: te dice qué idJugador le tocó a cada nombre
    public class JugadorRegistradoDto
    {
        [JsonProperty("idJugador")] public int IdJugador { get; set; }
        [JsonProperty("nombre")] public string Nombre { get; set; }
    }

    public class PartidaCreadaDto
    {
        [JsonProperty("idPartida")] public int IdPartida { get; set; }
        [JsonProperty("jugadores")] public List<JugadorRegistradoDto> Jugadores { get; set; }

        // Para cuando tengas el nombre del que jugó y necesites su idJugador
        public int IdDe(string nombre)
        {
            var j = Jugadores.FirstOrDefault(x => string.Equals(x.Nombre, nombre, StringComparison.OrdinalIgnoreCase));
            if (j == null) throw new ArgumentException("El jugador '" + nombre + "' no está en la partida.");
            return j.IdJugador;
        }
    }

    //  ApiHistorial: todo lo que habla con la API de Python.
    public static class ApiHistorial
    {
        private const int PUERTO_API = 8000;

        private static string _ip = "192.168.100.8"; //modificar por la direccion 
        private static HttpClient _http = CrearCliente(_ip);

        public static string ServidorIp
        {
            get { return _ip; }
            set
            {
                string ip = string.IsNullOrWhiteSpace(value) ? "127.0.0.1" : value.Trim();
                if (ip == _ip) return;
                _ip = ip;
                _http = CrearCliente(ip);
            }
        }

        private static HttpClient CrearCliente(string ip)
        {
            return new HttpClient
            {
                BaseAddress = new Uri("http://" + ip + ":" + PUERTO_API + "/"),
                Timeout = TimeSpan.FromSeconds(5)    // si la API no responde, no se queda colgado
            };
        }

        //Lectura: para los clientes
        public static async Task<List<PartidaDto>> ObtenerPartidasAsync()
        {
            string json = await _http.GetStringAsync("partidas");
            return JsonConvert.DeserializeObject<List<PartidaDto>>(json);
        }

        public static async Task<List<JugadorPartidaDto>> ObtenerJugadoresAsync(int idPartida)
        {
            string json = await _http.GetStringAsync("partidas/" + idPartida + "/jugadores");
            return JsonConvert.DeserializeObject<List<JugadorPartidaDto>>(json);
        }

        public static async Task<List<MovimientoDto>> ObtenerMovimientosAsync(int idPartida)
        {
            string json = await _http.GetStringAsync("partidas/" + idPartida + "/movimientos");
            return JsonConvert.DeserializeObject<List<MovimientoDto>>(json);
        }

        //Escritura: solo puede hacerlo en servidor

        //Mando los nombres de 2 a 3 jugadores, y me devuelve el id de la partida y el id de cada jugador
        public static async Task<PartidaCreadaDto> CrearPartidaAsync(IEnumerable<string> nombres)
        {
            var resp = await _http.PostAsync("partidas", Json(new { jugadores = nombres }));
            await AsegurarExito(resp);
            return JsonConvert.DeserializeObject<PartidaCreadaDto>(await resp.Content.ReadAsStringAsync());
        }

        public static async Task RegistrarMovimientoAsync(int idPartida, int idJugador, int numeroTurno, string accion, string descripcion)
        {
            var resp = await _http.PostAsync("partidas/" + idPartida + "/movimientos", Json(new
            {
                id_jugador = idJugador,
                numero_turno = numeroTurno,
                accion = accion,
                descripcion = descripcion
            }));
            await AsegurarExito(resp);
        }

        public static async Task FinalizarPartidaAsync(int idPartida, int idGanador)
        {
            var resp = await _http.PutAsync("partidas/" + idPartida + "/finalizar", Json(new { id_ganador = idGanador }));
            await AsegurarExito(resp);
        }

        //utilidades
        private static StringContent Json(object datos)
        {
            return new StringContent(JsonConvert.SerializeObject(datos), Encoding.UTF8, "application/json");
        }

        // Si la API responde con error, deja el mensaje claro ("Hay jugadores con el mismo nombre")
        private static async Task AsegurarExito(HttpResponseMessage resp)
        {
            if (resp.IsSuccessStatusCode) return;

            string cuerpo = await resp.Content.ReadAsStringAsync();
            string detalle = cuerpo;
            try
            {
                var o = JObject.Parse(cuerpo);
                if (o["detail"] != null) detalle = o["detail"].ToString();
            }
            catch { /* no venía en el formato esperado, se deja el texto tal cual */ }

            throw new HttpRequestException("(" + (int)resp.StatusCode + ") " + detalle);
        }
    }
}