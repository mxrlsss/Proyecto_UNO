using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNO_Game
{
    public partial class FormPartida
    {
        //Porque son dos jugadores, necesitazn coincidir con lo que ya está establecido en la API
        private const int MIN_JUGADORES = 2;
        private const int MAX_JUGADORES = 4;


        private int idPartida = 0;
        private int numeroTurno = 0;
        private bool apiAvisada = false;

        private readonly List<int> idsJugadores = new List<int>();

        private int IdJugador(int jugador) => idsJugadores[jugador - 1];

        private StringContent Json(object datos)
        {
            return new StringContent(
                JsonConvert.SerializeObject(datos),
                Encoding.UTF8,
                "application/json");
        }

        private void AvisarErrorApi(Exception ex)
        {
            if (apiAvisada) return;   // un solo aviso, para no molestar en cada jugada
            apiAvisada = true;
            MessageBox.Show("No se pudo guardar en la base de datos: " + ex.Message);
        }

        private async Task<int> CrearJugadorApi(string nombre)
        {
            var resp = await http.PostAsync("jugadores", Json(new { nombre = nombre }));
            resp.EnsureSuccessStatusCode();

            string json = await resp.Content.ReadAsStringAsync();
            return JObject.Parse(json)["idJugador"].Value<int>();
        }

        private async Task CrearPartidaApi(List<string> nombres)
        {
            // Reinicia el estado por si se juega otra partida con el mismo formulario
            idPartida = 0;
            numeroTurno = 0;
            apiAvisada = false;
            idsJugadores.Clear();

            nombres = nombres.Select(n => (n ?? "").Trim()).ToList();

            if (nombres.Count < MIN_JUGADORES || nombres.Count > MAX_JUGADORES)
            {
                MessageBox.Show($"Una partida necesita entre {MIN_JUGADORES} y {MAX_JUGADORES} jugadores.");
                return;
            }
            if (nombres.Any(string.IsNullOrEmpty))
            {
                MessageBox.Show("Todos los jugadores deben tener nombre.");
                return;
            }
            if (nombres.Distinct(StringComparer.OrdinalIgnoreCase).Count() != nombres.Count)
            {
                MessageBox.Show("Hay jugadores con el mismo nombre. Cada uno debe llamarse distinto.");
                return;
            }

            try
            {
                // 1) Crear (u obtener) cada jugador y guardar su ID
                foreach (string nombre in nombres)
                    idsJugadores.Add(await (CrearJugadorApi(nombre)));
                var resp = await http.PostAsync("partidas",
                    Json(new { ids_jugadores = idsJugadores }));
                resp.EnsureSuccessStatusCode();

                string json = await resp.Content.ReadAsStringAsync();
                idPartida = JObject.Parse(json)["idPartida"].Value<int>();
            }
            catch (Exception ex)
            {
                idsJugadores.Clear();   // si algo falló, no dejamos una lista a medias
                AvisarErrorApi(ex);
            }
        }


        private async Task RegistrarMovimientoApi(int jugador, string accion, string descripcion)
        {
            if (idPartida == 0) return;   // la partida no se pudo crear en la API
            numeroTurno++;
            try
            {
                var resp = await http.PostAsync($"partidas/{idPartida}/movimientos",
                    Json(new
                    { //Arma el json con las especificaciones
                        id_jugador = IdJugador(jugador),
                        numero_turno = numeroTurno,
                        accion = accion,
                        descripcion = descripcion
                    }));
                //SI existe errores los cacha y explota
                resp.EnsureSuccessStatusCode();
            }
            catch (Exception ex) { AvisarErrorApi(ex); }
        }

        private async Task FinalizarPartidaApi(int jugadorGanador)
        {
            if (idPartida == 0) return;
            try
            {
                var resp = await http.PutAsync($"partidas/{idPartida}/finalizar",
                    Json(new { id_ganador = IdJugador(jugadorGanador) }));
                resp.EnsureSuccessStatusCode();
            }
            catch (Exception ex) { AvisarErrorApi(ex); }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FormPartida
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Name = "FormPartida";
            this.Load += new System.EventHandler(this.FormPartida_Load);
            this.ResumeLayout(false);

        }
        /*
        private void FormPartida_Load(object sender, EventArgs e)
        {

        }*/
    }
}