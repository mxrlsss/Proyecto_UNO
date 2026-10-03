using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO_Game
{
    public class Carta
    {
        public string Color { get; set; } //color de la carta
        public string Valor { get; set; } //valor de la carta (+2, +4, reversa, salto, etc.)
        public string NombreRecurso { get; set; } //nombre del recurso de la carta
        public Carta(string color, string valor, string nombreRecurso)
        {
            Color = color;
            Valor = valor;
            NombreRecurso = nombreRecurso;
        }
    }
}
