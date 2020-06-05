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

    
    public partial class EnterNumberDialog : Form
    {

        public int n;
        private List<RegressionFunction> additionalFunctions;
        private Action redraw;

        public int Number
        {
            get { return n; }
        }

        public void SetCoefficientStrings(string[] strings)
        {
            comboBox2.BeginUpdate();

            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(strings);

            comboBox2.EndUpdate();
        }

        public void SetCoefficientStrings2(string[] strings)
        {
            comboBox3.BeginUpdate();

            comboBox3.Items.Clear();
            comboBox3.Items.AddRange(strings);

            comboBox3.EndUpdate();
        }

        public EnterNumberDialog(List<RegressionFunction> additionalFunctions, Action redraw)
        {
            this.additionalFunctions = additionalFunctions;
            this.redraw = redraw;

            InitializeComponent();
            comboBox1.BeginUpdate();

            comboBox1.Items
                .AddRange(additionalFunctions.Select((f) => f.FunctionString())
                .ToArray());

            comboBox1.EndUpdate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                n = Int32.Parse(textBox1.Text) - 1;
                if (n < 0 || n > additionalFunctions.Count)
                {
                    MessageBox.Show("Нет функции с таким номером", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    additionalFunctions.RemoveAt(n);

                    comboBox1.SelectedIndex = -1;
                    
                    comboBox1.BeginUpdate();
                    //comboBox2.Items.Remove(comboBox1.SelectedItem);
                    
                    comboBox1.Items.Clear();
                    comboBox1.Items
                        .AddRange(additionalFunctions.Select((f) => f.FunctionString())
                        .ToArray());

                    comboBox1.EndUpdate();
                    comboBox2.SelectedIndex = -1;
                    comboBox2.BeginUpdate();
                    comboBox2.Items.Clear();
                    comboBox2.EndUpdate();
                    comboBox3.SelectedIndex = -1;
                    comboBox3.BeginUpdate();
                    comboBox3.Items.Clear();
                    comboBox3.EndUpdate();
                }
                redraw.Invoke();
                this.Close();
            }
            catch(System.FormatException)
            {
                MessageBox.Show("Не выбран график!", 
                    "Данные не выбраны", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            try
            {
                
                comboBox2.SelectedIndex = comboBox1.SelectedIndex;
                comboBox3.SelectedIndex = comboBox1.SelectedIndex;
                if (comboBox1.SelectedIndex != -1)
                {
                    textBox1.Text = (comboBox1.SelectedIndex + 1).ToString();
                }
                else
                {
                    textBox1.Text = "";
                }
            }catch(System.ArgumentOutOfRangeException)
            {
                MessageBox.Show("Введены не правильные параметры. Должно быть выбрано одно напряжение!",
                                "Выберите напряжение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            additionalFunctions.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            redraw.Invoke();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            comboBox1.SelectedIndex = comboBox2.SelectedIndex;
            comboBox3.SelectedIndex = comboBox2.SelectedIndex;
            
            if (comboBox2.SelectedIndex != -1)
            {
                textBox1.Text = (comboBox2.SelectedIndex + 1).ToString();
            }
            else
            {
                textBox1.Text = "";
            }
        }


        private void comboBox3_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            
            comboBox2.SelectedIndex = comboBox3.SelectedIndex;
            comboBox1.SelectedIndex = comboBox3.SelectedIndex;
            
            if (comboBox3.SelectedIndex != -1)
            {
                textBox1.Text = (comboBox3.SelectedIndex + 1).ToString();
            }
            else
            {
                textBox1.Text = "";
            }

        }
    }
}
