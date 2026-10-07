using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UNO_Game
{
    public partial class FormPartida
    {
        //Porque son dos jugadores, necesitazn coincidir con lo que ya está establecido en la API
        private const int ID_JUGADOR1 = 1;
        private const int ID_JUGADOR2 = 2;

        private int idPartida = 0;
        private int numeroTurno = 0;
        private bool apiAvisada = false;

        private int IdJugador(int jugador) => jugador == 1 ? ID_JUGADOR1 : ID_JUGADOR2;

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

        private async Task CrearPartidaApi()
        {
            try
            {
                //Espera la respuesta de la API
                var resp = await http.PostAsync("partidas",
                    Json(new { id_jugador1 = ID_JUGADOR1, id_jugador2 = ID_JUGADOR2 }));
                //Lee el error en caso de haberlo y lo manda a catch
                resp.EnsureSuccessStatusCode();

                string json = await resp.Content.ReadAsStringAsync();
                //Busca en el diccionario al usuario con idPartida; .Value lo convierte a entero
                idPartida = JObject.Parse(json)["idPartida"].Value<int>();
            }
            catch (Exception ex) { AvisarErrorApi(ex); }
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
    }
}