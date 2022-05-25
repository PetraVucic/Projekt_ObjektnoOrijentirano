using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OTTER
{
    public partial class ODABIR : Form
    {
        private string putanja;

        public string Putanja
        {
            get { return putanja; }
            set { putanja = value; }
        }
        public ODABIR()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                putanja = "sprites//arya.png";
            }
            else if (radioButton2.Checked)
            {
                putanja = "sprites//daenerys.png";
            }
            else if (radioButton3.Checked)
            {
                putanja = "sprites//johnsnow.png";
            }
            else
            {
                MessageBox.Show("Odaberite jednog heroja!");
                return;
            }
            this.Close();
        }


        private void ODABIR_Load(object sender, EventArgs e)
        {

        }

        private void ODABIR_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.BackgroundImage = null;
            label1.Visible = false;
            pictureBox1.Visible = true;
            pictureBox2.Visible = true;
            pictureBox3.Visible = true;
            radioButton1.Visible = true;
            radioButton2.Visible = true;
            radioButton3.Visible = true;
            button1.Visible = true;
        }
    }
}
