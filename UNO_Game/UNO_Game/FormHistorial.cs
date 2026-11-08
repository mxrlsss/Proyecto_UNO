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

        // Aquí se guarda cuál es la partida que se pidió al último, para ignorar las viejas.
        private int idPartidaSolicitada = -1;

        public FormHistorial(Form1 menu)
        {
            InitializeComponent();
            this.menuPrincipal = menu; //Guarda la referencia al menú principal
            this.FormClosed += FormPartida_VentanaCerrada;

            ConectarEventosGrids();

            // Se recarga cada vez que el formulario se muestra de nuevo desde el menú
            VisibleChanged += async (s, e) =>
            {
                if (Visible) await CargarPartidas();
            };
        }

        public FormHistorial()
        {
            InitializeComponent();
            ConectarEventosGrids();
            Load += async (s, e) => await CargarPartidas();
        }

        private void ConectarEventosGrids()
        {
            dataGridViewPartida.CellFormatting += dataGridViewPartida_CellFormatting;
            dataGridViewJugadorPartida.CellFormatting += dataGridViewJugadorPartida_CellFormatting;
            dataGridViewPartida.SelectionChanged += async (s, e) => await CargarDetalle();
        }

        private async Task CargarPartidas()
        {
            try
            {
                dataGridViewPartida.DataSource = await ApiHistorial.ObtenerPartidasAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo cargar el historial desde " + ApiHistorial.ServidorIp + ".\n" +
                    "¿Está encendida la API en esa PC?\n\n" + ex.Message);
            }
        }

        // Carga los jugadores y los movimientos de la partida seleccionada, las dos a la vez
        private async Task CargarDetalle()
        {
            var partida = dataGridViewPartida.CurrentRow?.DataBoundItem as PartidaDto;
            if (partida == null)
            {
                idPartidaSolicitada = -1;
                dataGridViewMovimiento.DataSource = null;
                dataGridViewJugadorPartida.DataSource = null;
                return;
            }

            int id = partida.IdPartida;
            idPartidaSolicitada = id;

            try
            {
                var tareaJugadores = ApiHistorial.ObtenerJugadoresAsync(id);
                var tareaMovimientos = ApiHistorial.ObtenerMovimientosAsync(id);
                await Task.WhenAll(tareaJugadores, tareaMovimientos);

                if (idPartidaSolicitada != id) return;   // ya eligió otra partida, esta respuesta sobra

                dataGridViewJugadorPartida.DataSource = tareaJugadores.Result;
                dataGridViewMovimiento.DataSource = tareaMovimientos.Result;
            }
            catch (Exception ex)
            {
                if (idPartidaSolicitada == id)
                    MessageBox.Show("Error al cargar el detalle de la partida: " + ex.Message);
            }
        }

        private void dataGridViewPartida_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dataGridViewPartida.Columns[e.ColumnIndex].DataPropertyName;

            if (col == "Ganador" && string.IsNullOrEmpty(e.Value as string))
            {
                e.Value = "Sin ganador";
                e.FormattingApplied = true;
            }
            else if (col == "FechaFin" && e.Value == null)
            {
                e.Value = "Partida interrumpida";
                e.FormattingApplied = true;
            }
            else if (col == "Jugadores" && string.IsNullOrEmpty(e.Value as string))
            {
                e.Value = "Sin jugadores";
                e.FormattingApplied = true;
            }
        }

        private void dataGridViewJugadorPartida_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string col = dataGridViewJugadorPartida.Columns[e.ColumnIndex].DataPropertyName;

            if (col == "Resultado")
            {
                string resultado = e.Value as string;

                if (resultado == "ganador")
                    e.Value = "Ganador";
                else if (resultado == "perdedor")
                    e.Value = "Perdedor";
                else
                    e.Value = "Cancelada";   // en_curso, vacío o null

                e.FormattingApplied = true;
            }
        }

        private void FormHistorial_Load(object sender, EventArgs e)
        {

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

        private void dataGridViewMovimiento_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridViewPartida_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}