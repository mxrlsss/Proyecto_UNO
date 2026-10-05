using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNO_Game
{
    public partial class FormHistorial : Form
    {
        private Form1 menuPrincipal; //Guarda una referencia al menú
        public FormHistorial(Form1 menu)
        {
            InitializeComponent();
            this.menuPrincipal = menu; //Guarda la referencia al menú principal
            this.FormClosed += FormPartida_VentanaCerrada;
        }
        public FormHistorial()
        {
            InitializeComponent();

        }

        private void FormHistorial_Load(object sender, EventArgs e)
        {
            
        }

        private void CargarHistorialPartidas()
        {
            string query = @"Select 
                        p.idPartida as 'ID',
                        p.fecha_inicio as 'Fecha Inicio',
                        p.fecha_fin as 'Fecha Fin',
                        j.nombre as 'Ganador'
                        from Partida p
                        left join Jugador j on p.id_ganador = j.idJugador
                        order by p.idPartida DESC;";
            using(MySqlConnection conn = ConexionBD.ObtenerConexion())
            {
                try
                {
                    conn.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    //Le da los datos al DataGridView de partida
                    dataGridViewPartida.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar el historial de partidas: " + ex.Message);
                }
            }
        }

        private void CargarMovimientosPartida(int idPartida)
        {
            string query = @"Select 
                        m.num_turno as 'Turno',
                        j.nombre as 'Jugador',
                        m.accion as 'Acción',
                        m.descripcion as 'Descripción',
                        m.timestamp as  'Tiempo'
                        from log_movimientos m
                        inner join Jugador j on m.id_Jugador = j-idJugador
                        where m.id_Partida = @idPartida
                        order by m.num_turno ASC;";
            using (MySqlConnection conn = ConexionBD.ObtenerConexion())
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idPartida", idPartida);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    //Le da los datos al DataGridView de movimientos
                    dataGridViewMovimiento.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los movimientos de la partida: " + ex.Message);
                }
            }
        }

        private void dataGridViewPartida_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0) //Verifica que no sea un encabezado
            {
                int idPartida = Convert.ToInt32(dataGridViewPartida.Rows[e.RowIndex].Cells["ID"].Value);
                CargarMovimientosPartida(idPartida);
            }
        }
        private void btnVolverMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void FormPartida_VentanaCerrada(object sender, FormClosedEventArgs e)
        {
            if (menuPrincipal != null)
                menuPrincipal.Show(); //Vuelve a mostrar el menú

        }

    }
}
