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

            // Antes: Load += async (s, e) => await CargarPartidas();
            VisibleChanged += async (s, e) =>
            {
                if (Visible) await CargarPartidas();
            };
            dataGridViewPartida.SelectionChanged += async (s, e) => await CargarMovimientos();
        }
        public FormHistorial()
        {
            InitializeComponent();
            Load += async (s, e) => await CargarPartidas();
            dataGridViewPartida.SelectionChanged += async (s, e) => await CargarMovimientos();

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
            if (partida == null) return;

            string json = await http.GetStringAsync($"partidas/{partida.IdPartida}/movimientos");
            var movs = JsonConvert.DeserializeObject<List<MovimientoDto>>(json);
            dataGridViewMovimiento.DataSource = movs;
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

    }
}
