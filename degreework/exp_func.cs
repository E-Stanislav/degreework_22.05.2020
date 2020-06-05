using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace degreework
{
    public partial class exp_func : Form
    {
        private double y0;
        private double a1;
        private double b1;

        public double Y0
        {
            get { return y0; }
        }
        public double A1
        {
            get { return a1; }
        }
        public double B1
        {
            get { return b1; }
        }


        bool drag = false;
        Point Start_Point = new Point(0, 0); 
        public exp_func()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                y0 = Convert.ToDouble(textBox1.Text);
                a1 = Convert.ToDouble(textBox2.Text);
                b1 = Convert.ToDouble(textBox3.Text);
                this.Close();
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Не введено число", "Ошибка", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void exp_func_MouseDown(object sender, MouseEventArgs e)
        {
            drag = true;
            Start_Point = new Point(e.X, e.Y);
        }

        private void exp_func_MouseMove(object sender, MouseEventArgs e)
        {
            if(drag)
            {
                Point p = PointToScreen(e.Location);
                this.Location = new Point(p.X - Start_Point.X, p.Y - Start_Point.Y);
            }
        }

        private void exp_func_MouseUp(object sender, MouseEventArgs e)
        {
            drag = false;
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
