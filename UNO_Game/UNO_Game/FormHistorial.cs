using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UNO_Game
{
    public partial class FormHistorial : Form
    {
        private static readonly HttpClient http =
            new HttpClient { BaseAddress = new Uri("http://localhost:8000/") };
        private Form1 menuPrincipal; //Guarda una referencia al menú
        public FormHistorial(Form1 menu)
        {
            InitializeComponent();
            this.menuPrincipal = menu; //Guarda la referencia al menú principal
            this.FormClosed += FormPartida_VentanaCerrada;

            dataGridViewPartida.CellFormatting += dataGridViewPartida_CellFormatting;
            dataGridViewJugadorPartida.CellFormatting += dataGridViewJugadorPartida_CellFormatting;

            // Antes: Load += async (s, e) => await CargarPartidas();
            VisibleChanged += async (s, e) =>
            {
                if (Visible) await CargarPartidas();
            };
            dataGridViewPartida.SelectionChanged += async (s, e) =>
            {
                await CargarMovimientos();
                await CargarJugadoresPartida();
            };
        }
        public FormHistorial()
        {
            InitializeComponent();
            Load += async (s, e) => await CargarPartidas();
            dataGridViewPartida.SelectionChanged += async (s, e) =>
            {
                await CargarMovimientos();
                await CargarJugadoresPartida();
            };

        }

        private async Task CargarPartidas()
        {
            string json = await http.GetStringAsync("partidas");
            var partidas = JsonConvert.DeserializeObject<List<PartidaDto>>(json);
            dataGridViewPartida.DataSource = partidas;
        }

        private async Task CargarMovimientos()
        {
            var partida = dataGridViewPartida.CurrentRow?.DataBoundItem as PartidaDto;
            if (partida == null)
            {
                dataGridViewMovimiento.DataSource = null; //ojo agregué
                return;
            }
             

            string json = await http.GetStringAsync($"partidas/{partida.IdPartida}/movimientos");
            var movs = JsonConvert.DeserializeObject<List<MovimientoDto>>(json);
            dataGridViewMovimiento.DataSource = movs;
        }

        private async Task CargarJugadoresPartida()
        {
            var partida = dataGridViewPartida.CurrentRow?.DataBoundItem as PartidaDto;
            if (partida == null)
            {
                dataGridViewJugadorPartida.DataSource = null;
                return;
            }

            string json = await http.GetStringAsync($"partidas/{partida.IdPartida}/jugadores");
            var jugadores = JsonConvert.DeserializeObject<List<JugadorPartidaDto>>(json);
            dataGridViewJugadorPartida.DataSource = jugadores;
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
