using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UNO_Game;




namespace UNO_Game
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void Start_Button_Click(object sender, EventArgs e)
        {
            FormSala formSala = new FormSala();
            formSala.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //YA NO SE USA AQUÍ
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Mazo_P1_Click(object sender, EventArgs e)
        {

        }

        private void Mazo_P2_Click(object sender, EventArgs e)
        {

        }

        private void Pozo_Click(object sender, EventArgs e)
        {

        }
    }
}
