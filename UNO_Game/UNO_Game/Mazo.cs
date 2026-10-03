using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO_Game
{
    public class Mazo
    {
        private List<Carta> Cartas; 
        public Mazo()
        {
            Cartas = new List<Carta>();
            InicializarMazo();
        }
        private void InicializarMazo()
        {
            string[] colores = { "Rojo", "Verde", "Azul", "Amarillo" };

            foreach (string color in colores)
            {
                Cartas.Add(new Carta(color, "0", $"{color}0")); //carta 0 de cada color

                for (int i = 1; i <= 9; i++) // cartas del 1 al 9 de cada color (*2)
                {
                    Cartas.Add(new Carta(color, i.ToString(), $"{color}{i}"));
                    Cartas.Add(new Carta(color, i.ToString(), $"{color}{i}"));
                }

                for (int i = 0; i < 2; i++) //cartas accion de cada color
                {
                    Cartas.Add(new Carta(color, "MasDos", $"{color}_mas2"));
                    Cartas.Add(new Carta(color, "Salto", $"{color}_salto"));
                    Cartas.Add(new Carta(color, "Reversa", $"{color}_reversa"));
                }

                for (int i = 0; i < 4; i++)  //comodines
                {
                    Cartas.Add(new Carta("Comodin", "CambioColor", "Comodin_colores"));
                    Cartas.Add(new Carta("Comodin", "MasCuatro", "Comodin_mas4"));
                }
            }
        }

        public void Barajar()
        {
            Random rand = new Random();
            int n = Cartas.Count;
            while(n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                Carta value = Cartas[k];
                Cartas[k] = Cartas[n];
                Cartas[n] = value;
            }
        }

        public Carta RobarCarta()
        {
            if (Cartas.Count == 0) 
                return null;

            Carta cartaTomada = Cartas[0];
            Cartas.RemoveAt(0);
            return cartaTomada;
        }

        public List<Carta> RepartirMazo(int cantidad)
        {
            List<Carta> mazo = new List<Carta>();
            for (int i = 0; i < cantidad; i++)
            {
                Carta cartaTomada = RobarCarta();
                if (cartaTomada != null)
                    mazo.Add(cartaTomada);
            }
            return mazo;
        }

    }
}
