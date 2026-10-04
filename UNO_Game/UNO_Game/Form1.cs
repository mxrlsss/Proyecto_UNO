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
            FormPartida formPartida = new FormPartida(this);
            formPartida.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormHistorial formHistorial = new FormHistorial(this);
            formHistorial.Show();
            this.Hide();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }


       

    }
}
