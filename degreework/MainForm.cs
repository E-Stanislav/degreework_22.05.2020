using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Configuration;
using System.Reflection;
using Microsoft.Office.Interop.Word;
using OpenGL;
using System.Diagnostics;

namespace degreework
{
    public partial class MainForm : Form
    {
        public static string LastFilePath;
        public static string FilePath;
        string LastOpenProject;
        public static MainForm formm;
        public static string PrScFilepath="";
        public static int count=0;
        //public bool check = true; 

        public MainForm()
        {
            InitializeComponent();
            formm = this;
        }

        private void OpenProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenProject();
        }


        private void OpenProject()
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Выбрать папку с проектом";

            if (LastOpenProject != "")
            {
                dlg.SelectedPath = LastOpenProject;
            }

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FilePath = dlg.SelectedPath;
                //Data.file_path = dlg.SelectedPath;

                if (!(File.Exists(FilePath + @"\RESULT1.BIN") && File.Exists(FilePath + @"\RESULT2.BIN")))// && File.Exists(Data.file_path + @"\RESULT3.BIN")))
                {
                    LastOpenProject = FilePath;
                    NoFiles nofiles = new NoFiles();
                    nofiles.Show();
                }
                else
                {
                    LastOpenProject = FilePath;
                    toolStripStatusLabelName.Text = FilePath;
                    ResultToolStripMenuItem.Enabled = true;
                    toolStripButton3.Enabled = true;

                    write_config();
                }

            }
        }


        private void ResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ++count;
            write_config();
            System.Diagnostics.Process.Start(System.Windows.Forms.Application.StartupPath + "\\Remake_Model_Form.exe");
            //Form1 newModel = new Form1(this);
            //newModel.Show();
            //
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LastFilePath = AppConfigSettings.ReadSetting("Посдледний открытый проект для графиков");
            LastOpenProject = AppConfigSettings.ReadSetting("Посдледний открытый проект");
           
            if (LastOpenProject != "")
            {
                OpenLastProjectToolStripMenuItem.Enabled = true;
                toolStripButton2.Enabled = true;
            }



                int sss;
            sss = Convert.ToInt16(AppConfigSettings.ReadSetting("Панель инструментов"));
            Settings.check = Convert.ToBoolean(sss);
            if (Settings.check)
            {
                toolStrip1.Visible = true;
            }
            else
            {
                toolStrip1.Visible = false;
            }
           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppConfigSettings.WriteSetting("Посдледний открытый проект для графиков", LastFilePath);
            AppConfigSettings.WriteSetting("Посдледний открытый проект", LastOpenProject);
            int sss = Convert.ToInt16(Settings.check);
            AppConfigSettings.WriteSetting("Панель инструментов", sss.ToString());
            AppConfigSettings.WriteSetting("Папка для скриншотов", PrScFilepath);
        }

        private void OpenLastProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilePath = LastOpenProject;
            toolStripStatusLabelName.Text = LastOpenProject;
            ResultToolStripMenuItem.Enabled = true;
            toolStripButton3.Enabled = true;
            write_config();
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            /*Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();

            Microsoft.Office.Interop.Word.Document doc;
            object fileName = @"C:\Users\Imp\Documents\Visual Studio 2010\Projects\degreework\degreework\bin\Debug\Help.docx";
            object falseValue = false;
            object trueValue = true;
            object missing = Type.Missing;

            doc = app.Documents.Open(ref fileName, ref missing, ref trueValue,
            ref missing, ref missing, ref missing, ref missing, ref missing,        
            ref missing, ref missing, ref missing, ref missing, ref missing,
            ref missing, ref missing, ref missing);
            app.Visible = true;*/
            //string pathhelp =System.Windows.Forms.Application.StartupPath+@"/Help/"+"\\Help.chm";
            string pathhelp = System.Windows.Forms.Application.StartupPath  + "\\Help.chm";
            System.Windows.Forms.Help.ShowHelp(this, pathhelp);
        }

        private void GrafToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graphics graph = new Graphics();
            graph.Show();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenProject();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            FilePath = LastOpenProject;
            toolStripStatusLabelName.Text = LastOpenProject;
            ResultToolStripMenuItem.Enabled = true;
            toolStripButton3.Enabled = true;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ++count;
            write_config();
            System.Diagnostics.Process.Start(System.Windows.Forms.Application.StartupPath + "\\Remake_Model_Form.exe");
            //Form1 newModel = new Form1(this);
            //newModel.Show();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Graphics graph = new Graphics();
            graph.Show();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings newformS = new Settings();
            настройкиToolStripMenuItem.Enabled = false;
            newformS.Show();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 box_about = new AboutBox1();
            box_about.Show();
        }

        private void write_config()
        {
            FileStream fs = new FileStream(System.Windows.Forms.Application.StartupPath + "\\config.txt", FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(FilePath);
            sw.WriteLine(count.ToString());
            sw.Close();
            fs.Close();
        }

        private void ReportToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void техПоддержкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.mai6.ru/contents/Kafedra609/Discipliny/CADCAEcistemy");
        }

        private void сообщитьОПроблемеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://forms.gle/29unAtG27tbpEeo27");
        }

        private void отправитьОтзывToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://forms.gle/29unAtG27tbpEeo27");
        }

        private void ProjecttoolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
