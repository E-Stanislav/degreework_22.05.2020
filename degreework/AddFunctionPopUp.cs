using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace degreework
{
    public partial class AddFunctionPopUp : Form
    {

        private double y0;
        private double a;
        private double b;
        private double c;
        private double d;

        public double Y0
        {
            get { return y0; }
        }
        public double A
        {
            get { return a; }
        }
        public double B
        {
            get { return b; }
        }
        public double C
        {
            get { return c; }
        }
        public double D
        {
            get { return d; }
        }


        public AddFunctionPopUp()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                y0 = Double.Parse(textBox1.Text);
                a = Double.Parse(textBox2.Text);
                b = Double.Parse(textBox3.Text);
                c = Double.Parse(textBox4.Text);
                d = Double.Parse(textBox5.Text);
                Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Неверный формат числа", "Неверный формат числа", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
            }
        }

    }
}
