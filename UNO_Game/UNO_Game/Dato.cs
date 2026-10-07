using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace UNO_Game
{
    public class PartidaDto
    {
        [JsonProperty("idPartida")] public int IdPartida { get; set; }
        [JsonProperty("fecha_inicio")] public DateTime? FechaInicio { get; set; }
        [JsonProperty("fecha_fin")] public DateTime? FechaFin { get; set; }
        [JsonProperty("ganador")] public string Ganador { get; set; }
    }

    public class MovimientoDto
    {
        [JsonProperty("numero_turno")] public int NumTurno { get; set; }
        [JsonProperty("jugador")] public string Jugador { get; set; }
        [JsonProperty("accion")] public string Accion { get; set; }
        [JsonProperty("descripcion")] public string Descripcion { get; set; }
        [JsonProperty("tiempo_jugada")] public DateTime TiempoJugada { get; set; }
    }
}
