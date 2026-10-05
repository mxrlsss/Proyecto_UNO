using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient; //Para manejar a conecion a SQL

namespace UNO_Game
{
    internal class ConexionBD
    {
        private static string cadenaConexion = "Server=127.0.0.1; Database=bebesote; Uid=root; Pwd=M4ng0ph*nk; Port=3306;";

        public static MySqlConnection ObtenerConexion()
        {
            return new MySqlConnection(cadenaConexion);
        }
    }
}
