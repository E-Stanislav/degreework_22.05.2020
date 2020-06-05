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
    public partial class Settings : Form
    {
        public static bool check;
        //int sss;
        public Settings()
        {
            InitializeComponent();
            //MdiParent = parent;
            
            checkBox1.Checked = check;
            //Панель инструментов
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                MainForm.formm.toolStrip1.Visible = true;
                check = true;
            }
            else
            {
                MainForm.formm.toolStrip1.Visible = false;
                check = false;
                //checkBox1.Checked = false;
            }
        }

        private void Settings_FormClosed(object sender, FormClosedEventArgs e)
        {
            check = checkBox1.Checked;
            MainForm.formm.настройкиToolStripMenuItem.Enabled = true;
            //formm.настройкиToolStripMenuItem.Enabled = true;
            
        }
    }
}
