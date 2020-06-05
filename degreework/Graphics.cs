using _2d_graphics_d;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.Statistics;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using ZedGraph;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace degreework
{
    public partial class Graphics : Form
    {

        GraphPane pane;
        GraphPane pane_d;
        Data data;
        string nrc;
        double x;
        double y;
        string tempx;
        string tempy;
        string res1;
        string res2;
        string res3;
        string res4;
        public static string FilePath;
        PointPairList list;
        double[] elements = new double[10];
        double[,] cm_m = new double[10, 7];
        double[,] cpc_m = new double[10, 7];
        double[,] ke_m = new double[10, 7];
        double[,] movx = new double[10, 7];
        double[,] movy = new double[10, 7];
        DirectoryInfo[] directories;
        string m_type_calc = "cpc";
        bool flag = false;
        List<String> ApproximationPointsType =
            new List<String>()
            {
                "Напряжение по X", "Напряжение по Y", "Касательное напряжение",
                "1-е главное напряжение", "2-е главное напряжение", "Эквивалентное",
            };
        List<Color> ApproximationColors =
            new List<Color>() {
                Color.Red, Color.Orange, Color.Green,
                Color.Cyan, Color.LightPink, Color.Violet,
            };
        GraphType CurrentGraphType = GraphType.Complex;
        enum GraphType
        {
            Complex, MoveX, MoveY, ForceECK, MoveECK
        }

        public List<RegressionFunction> additionalFunctions = new List<RegressionFunction>();
        private EnterNumberDialog enterNumberDialog;

        public Graphics()
        {
            InitializeComponent();



            ClearCheck();
            // Включим показ всплывающих подсказок при наведении курсора на график
            zedGraph.IsShowPointValues = true;

            // Будем обрабатывать событие PointValueEvent, чтобы изменить формат представления координат
            zedGraph.PointValueEvent +=
                new ZedGraphControl.PointValueHandler(zedGraph_PointValueEvent);

            zedGraph.GraphPane.XAxis.ScaleFormatEvent +=
                new ZedGraph.Axis.ScaleFormatHandler(
                    (pane, axis, val, index) =>
                    {
                        if (((val % 1 < 1e-15) || (val % 1 > 0.999999999999))
                                && (val - 3 >= 0) && (val - 3 < 10))
                        {
                            int i = (val % 1 > 0.999999999999)
                                    ? (((int)val) - 2)
                                    : (((int)val) - 3);
                            return elements[i] > 0
                                    ? val + ("\n" + elements[i])
                                    : val.ToString();
                        }
                        else
                        {
                            return val.ToString();
                        }
                    });
            /*val % 1 == 0
                    ? val + " (" + (2 * Math.Pow(val - 1, 2)) + ")"
                    : "");*/
            //MdiParent = parent;
            pane = zedGraph.GraphPane;

            // Fill the axis background with a color gradient
            pane.Chart.Fill = new Fill(Color.FromArgb(220, 220, 255), Color.White, 45F);

            int mainTitleFontSize = 20;
            int legendFontSize = 9;
            // Установим размеры шрифта для легенды
            pane.Legend.FontSpec.Size = legendFontSize;
            // Установим размеры шрифта для общего заголовка
            pane.Title.FontSpec.Size = mainTitleFontSize;
            pane.Title.FontSpec.IsUnderline = true;
            // Fill the pane background with a color gradient
            //pane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45F);
        }

        string zedGraph_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            // Получим точку, около которой находимся
            PointPair point = curve[iPt];

            // Сформируем строку
            string result = string.Format("X: {0:F3}\nY: {1:F3}", point.X, point.Y);

            return result;
        }

        private void OpenDataToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Выбрать папку с проектом";

            if (dlg.ShowDialog() == DialogResult.OK)
            {

                DirectoryInfo maindirectiry = new DirectoryInfo(dlg.SelectedPath);
                MainForm.LastFilePath = dlg.SelectedPath;
                toolStripStatusLabel2.Text = MainForm.LastFilePath;
                directories = maindirectiry.GetDirectories();
                flag = true;
                if (MainForm.LastFilePath != "")
                {
                    открытьПоследнийToolStripMenuItem1.Enabled = true;
                }

            }

        }


        public void movement(int ii, double x, double y)
        {
            Int64 num_of_el, i, j;
            element el;
            node[] node_n = new node[3];
            double[] dx = new double[3];
            double[] dy = new double[3];
            double[,] A = new double[3, 3];
            double[,] m = new double[3, 4];
            double[] u = new double[3];
            double[] v = new double[3];
            double uxy, vxy;
            //double temp;
            bool solved = true;

            num_of_el = data.find_el_by_point(x, y);
            //richTextBox1.Text += num_of_el.ToString();
            //richTextBox1.Text += "\n";
            el = data.find_el_by_number(num_of_el);
            node_n[0] = data.temp_nodes.getnode(el.node1 - 1);
            node_n[1] = data.temp_nodes.getnode(el.node2 - 1);
            node_n[2] = data.temp_nodes.getnode(el.node3 - 1);

            for (i = 0; i != 3; ++i)
            {
                A[i, 0] = 1.0;
                A[i, 1] = node_n[i].x;
                //richTextBox1.Text += A[i,2].ToString();
                //richTextBox1.Text += "\n";

                A[i, 2] = node_n[i].y;
                //richTextBox1.Text += A[i, 3].ToString();
                //richTextBox1.Text += "\n";
                dx[i] = node_n[i].movX;
                //richTextBox1.Text += dx[i].ToString();
                //richTextBox1.Text += "\n";
                dy[i] = node_n[i].movY;
                //richTextBox1.Text += dy[i].ToString();
                //richTextBox1.Text += "\n";
            }

            for (i = 0; i != 3; ++i)
            {
                for (j = 0; j != 3; ++j)
                {
                    m[i, j] = A[i, j];
                }
                m[i, 3] = dx[i];
            }
            solved = solveGJ(m, 3);

            //


            //

            if (!solved)
            {

            }


            for (i = 0; i != 3; ++i)
            {
                u[i] = m[i, 3];
            }



            for (i = 0; i != 3; ++i)
            {
                for (j = 0; j != 3; ++j)
                {
                    m[i, j] = A[i, j];
                }
                m[i, 3] = dy[i];
            }
            solved = solveGJ(m, 3);

            if (!solved)
            {

            }


            for (i = 0; i != 3; ++i)
            {
                v[i] = m[i, 3];
            }


            uxy = u[0] + u[1] * x + u[2] * y;
            vxy = v[0] + v[1] * x + v[2] * y;
            movx[ii, 0] = uxy;
            movy[ii, 0] = vxy;

        }


        public bool solveGJ(double[,] m, int nRow)
        {
            int i, j, k, maxRow;
            double c, temp;
            double eps;
            eps = 1E-10;
            for (i = 0; i != nRow; ++i)
            {
                maxRow = i;
                for (j = i + 1; j != nRow; ++j)
                {
                    if (Math.Abs(m[j, i]) > Math.Abs(m[maxRow, i]))
                        maxRow = j;
                }
                for (k = 0; k != nRow; ++k)
                {
                    temp = m[i, k];
                    m[i, k] = m[maxRow, k];
                    m[maxRow, k] = temp;
                }

                if (Math.Abs(m[i, i]) < eps)
                    return false;

                for (j = i + 1; j != nRow; ++j)
                {
                    c = m[j, i] / m[i, i];
                    for (k = i; k != nRow + 1; ++k)
                    {
                        m[j, k] = m[j, k] - m[i, k] * c;
                    }

                }
            }

            for (i = nRow - 1; i >= 0; --i)
            {
                c = m[i, i];
                for (j = 0; j != i; ++j)
                {
                    for (k = nRow; k >= 0; --k)
                    {
                        m[j, k] = m[j, k] - m[i, k] * m[j, i] / c;

                    }

                }
                m[i, i] = m[i, i] / c;
                m[i, nRow] = m[i, nRow] / c;
            }



            return true;

        }


        public void cm_m_stress(int ii, double x, double y)
        {
            Int64 elem;
            element el;
            byte[] count = new byte[3];
            double[] k = new double[3];
            node[] node_n = new node[3];
            double[,] strain = new double[3, 6];
            double rx, ry;
            byte i, j;
            double[] st = new double[6];


            elem = data.find_el_by_point(x, y);
            //MessageBox.Show(elem.ToString());
            el = data.find_el_by_number(elem);
            node_n[0] = data.temp_nodes.getnode(el.node1 - 1);
            node_n[1] = data.temp_nodes.getnode(el.node2 - 1);
            node_n[2] = data.temp_nodes.getnode(el.node3 - 1);

            for (i = 0; i != 3; ++i)
            {
                for (j = 0; j != 6; ++j)
                {
                    strain[i, j] = 0;
                    count[i] = 0;
                }

            }

            foreach (element ele in data.temp_elem.all_elements)
            {


                for (i = 0; i != 3; ++i)
                {
                    if (ele.node1 == node_n[i].number || ele.node2 == node_n[i].number || ele.node3 == node_n[i].number)
                    {
                        //textBox13.Text = ele.number.ToString();

                        for (j = 0; j != 6; ++j)
                        {
                            strain[i, j] = strain[i, j] + ele.stress[j];


                        }
                        ++count[i];
                        //textBox13.Text = count[0].ToString();
                    }
                }//if


            }//fr

            rx = (node_n[0].x + node_n[1].x + node_n[2].x) / 3;
            ry = (node_n[0].y + node_n[1].y + node_n[2].y) / 3;

            for (j = 0; j != 6; ++j)
            {
                for (i = 0; i != 3; ++i)
                {
                    strain[i, j] = mydiv(strain[i, j], count[i]);
                }

                for (i = 0; i != 3; ++i)
                {
                    k[i] = mydiv(Math.Sqrt((Math.Pow((rx - node_n[i].x), 2) + Math.Pow((ry - node_n[i].y), 2))), Math.Sqrt(Math.Pow((x - node_n[i].x), 2) + Math.Pow((y - node_n[i].y), 2)));
                }
                st[j] = mydiv(strain[0, j] * k[0] + strain[1, j] * k[1] + strain[2, j] * k[2], k[0] + k[1] + k[2]);
                cm_m[ii, j] = st[j];

            }

        }


        public void cps_m_stress(int ii, double x, double y)
        {
            Int64 elem;
            element el;
            byte[] count = new byte[3];
            double[] k = new double[3];
            node[] node_n = new node[3];
            double[,] strain = new double[3, 6];
            double rx, ry;
            byte i, j;
            double[] st = new double[6];


            elem = data.find_el_by_point(x, y);
            el = data.find_el_by_number(elem);
            node_n[0] = data.temp_nodes.getnode(el.node1 - 1);
            node_n[1] = data.temp_nodes.getnode(el.node2 - 1);
            node_n[2] = data.temp_nodes.getnode(el.node3 - 1);

            for (i = 0; i != 3; ++i)
            {
                for (j = 0; j != 6; ++j)
                {
                    strain[i, j] = 0;
                    count[i] = 0;
                }

            }

            foreach (element ele in data.temp_elem.all_elements)
            {


                for (i = 0; i != 3; ++i)
                {
                    if (ele.node1 == node_n[i].number || ele.node2 == node_n[i].number || ele.node3 == node_n[i].number)
                    {
                        //textBox13.Text = ele.number.ToString();

                        for (j = 0; j != 6; ++j)
                        {
                            strain[i, j] = strain[i, j] + ele.stress[j];


                        }
                        ++count[i];
                        //textBox13.Text = count[0].ToString();
                    }
                }//if


            }//fr



            for (j = 0; j != 6; ++j)
            {
                for (i = 0; i != 3; ++i)
                {
                    strain[i, j] = mydiv(strain[i, j], count[i]);
                }
                rx = mydiv(node_n[0].x * strain[0, j] + node_n[1].x * strain[1, j] + node_n[2].x * strain[2, j], strain[0, j] + strain[1, j] + strain[2, j]);
                ry = mydiv(node_n[0].y * strain[0, j] + node_n[1].y * strain[1, j] + node_n[2].y * strain[2, j], strain[0, j] + strain[1, j] + strain[2, j]);

                for (i = 0; i != 3; ++i)
                {
                    k[i] = mydiv(Math.Sqrt((Math.Pow((rx - node_n[i].x), 2) + Math.Pow((ry - node_n[i].y), 2))), Math.Sqrt(Math.Pow((x - node_n[i].x), 2) + Math.Pow((y - node_n[i].y), 2)));
                }
                st[j] = mydiv(strain[0, j] * k[0] + strain[1, j] * k[1] + strain[2, j] * k[2], k[0] + k[1] + k[2]);
                cpc_m[ii, j] = st[j];
            }



        }


        public void ke_m_stress(int ii, double x, double y)
        {
            double i = data.find_el_by_point(x, y);
            element el = data.find_el_by_number(i);

            for (int j = 0; j != 6; ++j)
            {
                ke_m[ii, j] = el.stress[j];
            }
        }


        public double mydiv(double x, double y)
        {
            if (Math.Abs(y) < 0.1E-4900)
                return 0.1E-4900;
            else return x / y;
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            m_type_calc = "cpc";
            RedrawAllIfNeeded();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            m_type_calc = "cp";
            RedrawAllIfNeeded();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            m_type_calc = "ke";
            RedrawAllIfNeeded();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            tempx = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            tempy = textBox2.Text;
        }

        private void DrawGraph(int j, string stresstype, string typemove)
        {
            // Получим панель для рисования
            //GraphPane pane = zedGraph.GraphPane;
            pane = zedGraph.GraphPane;
            //pane = new GraphPane();
            // !!!
            // Изменим тест надписи по оси X
            if (data == null)
            {
                MessageBox.Show("Необходимо выбрать данные для построения графиков",
                        "Данные не выбраны", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            pane.XAxis.Title.Text = "NRC\nКоличество элементов";

            // Изменим параметры шрифта для оси X
            //pane.XAxis.Title.FontSpec.IsUnderline = true;
            //pane.XAxis.Title.FontSpec.IsBold = false;
            //pane.XAxis.Title.FontSpec.FontColor = Color.Blue;

            // Изменим текст по оси Y
            pane.YAxis.Title.Text = "Напряжение";

            // Изменим текст заголовка графика
            pane.Title.Text = stresstype;

            // В параметрах шрифта сделаем заливку красным цветом
            //pane.Title.FontSpec.Fill.Brush = new SolidBrush(Color.Red);
            pane.Title.FontSpec.Fill.IsVisible = true;

            // Сделаем шрифт не полужирным
            pane.Title.FontSpec.IsBold = false;


            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            // pane.CurveList.Clear();

            // Создадим список точек
            list = new PointPairList();
            //MessageBox.Show(m_type_calc);

            if (typemove == "n")
            {

                switch (m_type_calc)
                {
                    case "cpc":
                        for (int a = 0; a < 10; ++a)
                        {
                            list.Add(a + 3, cpc_m[a, j]);
                        }

                        break;
                    case "cp":
                        for (int a = 0; a < 10; ++a)
                        {
                            list.Add(a + 3, cm_m[a, j]);
                        }
                        break;
                    case "ke":
                        for (int a = 0; a < 10; ++a)
                        {
                            list.Add(a + 3, ke_m[a, j]);
                        }
                        break;

                }
            }
            else
            {
                if (typemove == "х")
                {
                    for (int a = 0; a < 10; ++a)
                    {
                        list.Add(a + 3, movx[a, 0]);
                    }
                }
                else
                {
                    for (int a = 0; a < 10; ++a)
                    {
                        list.Add(a + 3, movy[a, 0]);
                    }
                }
            }


            // Заполняем список точек
            //for (double x = xmin; x <= xmax; x += 0.01)
            //{
            //list.Add(x, f(x));
            //}

            // Создадим кривую
            LineItem myline = pane.AddCurve("", list, Color.Blue, SymbolType.Circle);
            myline.Symbol.Fill.Type = FillType.Solid;
            myline.Line.IsVisible = интерполяцияПрямойToolStripMenuItem.Checked;
            //myline.Line.IsVisible = checkBox7.Checked;
            myline.Line.Width = 3.0f;
            // Устанавливаем интересующий нас интервал по оси X
            pane.XAxis.Scale.Min = 3;
            pane.XAxis.Scale.Max = 12;





            // Сетка
            // Включаем отображение сетки напротив крупных рисок по оси X
            pane.XAxis.MajorGrid.IsVisible = true;
            // Задаем вид пунктирной линии для крупных рисок по оси X:
            // Длина штрихов равна 10 пикселям, ... 
            pane.XAxis.MajorGrid.DashOn = 10;
            // затем 5 пикселей - пропуск
            pane.XAxis.MajorGrid.DashOff = 5;
            // Включаем отображение сетки напротив крупных рисок по оси Y
            pane.YAxis.MajorGrid.IsVisible = true;
            // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
            pane.YAxis.MajorGrid.DashOn = 10;
            pane.YAxis.MajorGrid.DashOff = 5;
            // Включаем отображение сетки напротив мелких рисок по оси X
            pane.YAxis.MinorGrid.IsVisible = true;
            // Задаем вид пунктирной линии для крупных рисок по оси Y: 
            // Длина штрихов равна одному пикселю, ... 
            pane.YAxis.MinorGrid.DashOn = 1;
            // затем 2 пикселя - пропуск
            pane.YAxis.MinorGrid.DashOff = 2;
            // Включаем отображение сетки напротив мелких рисок по оси Y
            pane.XAxis.MinorGrid.IsVisible = true;
            // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
            pane.XAxis.MinorGrid.DashOn = 1;
            pane.XAxis.MinorGrid.DashOff = 2;


            //pane.AxisChange();
            //pane.i
            //pane.
            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            zedGraph.AxisChange();

            // Обновляем график
            zedGraph.Invalidate();
            //pane_d = pane;
        }

        private void DrawApproximationGraph(int j, string stresstype, string typemove, string approximationtype)
        {
            // Получим панель для рисования
            //GraphPane pane = zedGraph.GraphPane;
            pane = zedGraph.GraphPane;
            //pane = new GraphPane();
            // !!!
            // Изменим тест надписи по оси X

            pane.GraphObjList.RemoveAll(text => text.GetType() == typeof(TextObj)
                    && ((string)text.Tag) == "2");

            if (data == null)
            {
                MessageBox.Show("Необходимо выбрать данные для построения графиков",
                        "Данные не выбраны", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            pane.XAxis.Title.Text = "NRC\nКоличество элементов";



            // Изменим параметры шрифта для оси X
            //pane.XAxis.Title.FontSpec.IsUnderline = true;
            //pane.XAxis.Title.FontSpec.IsBold = false;
            //pane.XAxis.Title.FontSpec.FontColor = Color.Blue;

            // Изменим текст по оси Y
            pane.YAxis.Title.Text = "Напряжение";

            // Изменим текст заголовка графика
            pane.Title.Text = stresstype;

            // В параметрах шрифта сделаем заливку красным цветом
            //pane.Title.FontSpec.Fill.Brush = new SolidBrush(Color.Red);
            pane.Title.FontSpec.Fill.IsVisible = true;

            // Сделаем шрифт не полужирным
            pane.Title.FontSpec.IsBold = false;


            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            // pane.CurveList.Clear();

            // Создадим список точек
            list = new PointPairList();
            //MessageBox.Show(m_type_calc);

            double[] x = new double[10];
            int[] x01 = new int[10];
            double[] y = new double[10];

            switch (m_type_calc)
            {
                case "cpc":
                    for (int a = 0; a < 10; ++a)
                    {
                        x[a] = a + 3;
                        x01[a] = a + 3;
                        y[a] = cpc_m[a, j];
                    }

                    break;
                case "cp":
                    for (int a = 0; a < 10; ++a)
                    {
                        x[a] = a + 3;
                        x01[a] = a + 3;
                        y[a] = cm_m[a, j];
                    }
                    break;
                case "ke":
                    for (int a = 0; a < 10; ++a)
                    {
                        x[a] = a + 3;
                        x01[a] = a + 3;
                        y[a] = ke_m[a, j];
                    }
                    break;

            }
            Func<double, double> ApproximationFunction;
            //Color FunctionColor;
            String FunctionString = "";
            switch (approximationtype)
            {
                case "Линейная":
                    Tuple<double, double> result1 = Fit.Line(x, y);
                    /*FunctionString = String.Format("{0:f3} + {1:f3} * x",
                            result1.Item1, result1.Item2);*/
                    //FunctionString = String.Format("Кд:{0:f3} + {1:f3}+ {2:f3} * x",
                    //      RSquare, result1.Item1, result1.Item2);
                    ApproximationFunction = (x1) => result1.Item1 + result1.Item2 * x1;

                    double[] res_1 = Results(ApproximationFunction);
                    double StError = GoodnessOfFit.PopulationStandardError(res_1, y);
                    double R = GoodnessOfFit.R(res_1, y);
                    double RSquared = GoodnessOfFit.RSquared(res_1, y);
                    double RSquare = CalculateRSquare(x01, y, ApproximationFunction);
                    int n = y.Length;
                    int k = 2; // кол-во параметров в функции
                    double Kds = CalculateKds(n, k, RSquare);
                    double AIC = CalculateAIC(x01, y, ApproximationFunction, k);
                    FunctionString = String.Format("{0:f3}+ {1:f3} * x \n" +
                        "Rsqr = {2:f4} // Кд = {3:f2} // Кдc = {4:f2} // AIC = {5:f2} " +
                        "// StError = {6:f2} // R = {7:f2}",
                             result1.Item1, result1.Item2,
                             RSquared, RSquare, Kds, AIC, StError, R);
                    break;
                case "Квадратичная":
                    double[] result2 = Fit.Polynomial(x, y, 2);
                    //FunctionString = String.Format("{0:f3} + {1:f3} * x + {2:f3} * x ^ 2",
                    //      result2[0], result2[1], result2[2]);
                    ApproximationFunction = (x1) => result2[0] + result2[1] * x1 + result2[2] * x1 * x1;

                    double[] res_2 = Results(ApproximationFunction);
                    double StError_2 = GoodnessOfFit.PopulationStandardError(res_2, y);
                    double R_2 = GoodnessOfFit.R(res_2, y);
                    double RSquared_2 = GoodnessOfFit.RSquared(res_2, y);
                    double RSquare_2 = CalculateRSquare(x01, y, ApproximationFunction);
                    int n2 = y.Length;
                    int k2 = 3;  // кол-во параметров в функции
                    double Kds2 = CalculateKds(n2, k2, RSquare_2);
                    double AIC2 = CalculateAIC(x01, y, ApproximationFunction, k2);
                    FunctionString = String.Format("{0:f3} + {1:f3} * x + {2:f3} * x ^ 2 \n" +
                        "Rsqr = {3:f4} // Кд = {4:f2} // Кдc = {5:f2} // AIC = {6:f2}" +
                        " // StError = {7:f2} // R = {8:f2} ",
                        result2[0], result2[1], result2[2],
                        RSquared_2, RSquare_2, Kds2, AIC2, StError_2, R_2);
                    break;
                case "Кубическая":
                    double[] result3 = Fit.Polynomial(x, y, 3);
                    //FunctionString = String.Format("{0:f3} + {1:f3} * x + {2:f3} * x ^ 2 + {3:f3} * x ^ 3",
                    //        result3[0], result3[1], result3[2], result3[3]);
                    ApproximationFunction = (x1) => result3[0] + result3[1] * x1
                            + result3[2] * x1 * x1 + result3[3] * x1 * x1 * x1;
                    double[] res_3 = Results(ApproximationFunction);
                    double StError_3 = GoodnessOfFit.PopulationStandardError(res_3, y);
                    double R_3 = GoodnessOfFit.R(res_3, y);
                    double RSquared_3 = GoodnessOfFit.RSquared(res_3, y);
                    double RSquare_3 = CalculateRSquare(x01, y, ApproximationFunction);
                    int n3 = y.Length;
                    int k3 = 4;  // кол-во параметров в функции
                    double Kds_3 = CalculateKds(n3, k3, RSquare_3);
                    double AIC3 = CalculateAIC(x01, y, ApproximationFunction, k3);
                    FunctionString = String.Format("{0:f3}+{1:f3}*x+{2:f3}*x^2+{3:f3}*x^3 \n" +
                        "Rsqr = {4:f4} // Кд = {5:f2} // Кдc = {6:f2} // AIC = {7:f2} " +
                        "// StError = {8:f2} // R = {9:f2}",
                            result3[0], result3[1], result3[2], result3[3],
                            RSquare_3, Kds_3, AIC3, StError_3, R_3, RSquared_3);
                    break;
                case "Экспоненциальная":
                    Tuple<double, double> result4 = Fit.Exponential(x, y);
                    if (!Double.IsNaN(result4.Item1) && !Double.IsNaN(result4.Item2))
                    {
                        //FunctionString = String.Format("{0:f3} * exp({1:f3} * x)",
                        //    result4.Item1, result4.Item2);
                        ApproximationFunction = (x1) => result4.Item1 * Math.Exp(result4.Item2 * x1);
                        double[] res_4 = Results(ApproximationFunction);
                        double StError_4 = GoodnessOfFit.PopulationStandardError(res_4, y);
                        double R_4 = GoodnessOfFit.R(res_4, y);
                        double RSquared_4 = GoodnessOfFit.RSquared(res_4, y);
                        double RSquare_4 = CalculateRSquare(x01, y, ApproximationFunction);
                        int n4 = y.Length;
                        int k4 = 2;  // кол-во параметров в функции
                        double Kds_4 = CalculateKds(n4, k4, RSquare_4);
                        double AIC4 = CalculateAIC(x01, y, ApproximationFunction, k4);
                        FunctionString = String.Format("{0:f3} * exp({1:f3} * x) \n" +
                            "Rsqr = {2:f4} // Кд = {3:f2} // Кдc = {4:f2} // AIC = {5:f2} " +
                            "// StError = {6:f2} // R = {7:f2}",
                            result4.Item1, result4.Item2,
                            RSquared_4, RSquare_4, Kds_4, AIC4, StError_4, R_4);
                    }
                    else
                    {
                        ApproximationFunction = (x1) => Double.NaN;
                    }
                    break;
                case "Логарифмическая":
                    Tuple<double, double> result5 = Fit.Logarithm(x, y);
                    //FunctionString = String.Format("{0:f3} + {1:f3} * ln(x)", result5.Item1,
                    //        result5.Item2);
                    ApproximationFunction = (x1) => result5.Item1 + result5.Item2 * Math.Log(x1);
                    double[] res_5 = Results(ApproximationFunction);
                    double StError_5 = GoodnessOfFit.PopulationStandardError(res_5, y);
                    double R_5 = GoodnessOfFit.R(res_5, y);
                    double RSquared_5 = GoodnessOfFit.RSquared(res_5, y);
                    double RSquare_5 = CalculateRSquare(x01, y, ApproximationFunction);
                    int n5 = y.Length;
                    int k5 = 2;  // кол-во параметров в функции
                    double Kds_5 = CalculateKds(n5, k5, RSquare_5);
                    double AIC5 = CalculateAIC(x01, y, ApproximationFunction, k5);
                    FunctionString = String.Format("{0:f3} + {1:f3} * ln(x)  \n" +
                        "Rsqr = {2:f4} // Кд = {3:f2} // Кдc = {4:f2} // AIC = {5:f2}" +
                        " // StError = {6:f2} // R = {7:f2}", 
                        result5.Item1, result5.Item2,
                        RSquared_5, RSquare_5, Kds_5, AIC5, StError_5, R_5);
                    break;
                case "Полиномиальная":
                    var Interpolation = Interpolate.Polynomial(x, y);
                    ApproximationFunction = Interpolation.Interpolate;
                    // double Coeff_6 = GoodnessOfFit.R(ApproximationFunction, y);
                    double[] res_6 = Results(ApproximationFunction);
                    double StError_6 = GoodnessOfFit.PopulationStandardError(res_6, y);
                    double R_6 = GoodnessOfFit.R(res_6, y);
                    double RSquared_6 = GoodnessOfFit.RSquared(res_6, y);
                    double RSquare_6 = CalculateRSquare(x01, y, ApproximationFunction);
                    int n6 = y.Length;
                    int k6 = 2;  // кол-во параметров в функции
                    double Kds_6 = CalculateKds(n6, k6, RSquare_6);
                    //double AIC6 = CalculateAIC(x01, y, ApproximationFunction, k6);
                    FunctionString = String.Format("\nRsqr = {0:f4} // Кд = {1:f2} // Кдc = {2:f2} // StError = {3:f2} // R = {4:f2}",
                        RSquared_6, RSquare_6, Kds_6, StError_6, R_6);
                    break;
                case "Сходящийся график 1":
                    /*Tuple<double, double, double> result7 = Fit.Curve(x, y,
                            (x1, a, b, c) => a + Math.Sin(x1 * b) * Math.Exp(c / x1),
                            MathNet.Numerics.Statistics.Statistics.Mean(x), 0.01,
                            -Math.Abs(x[x.Length - 1]) * (x.Length + 2));
                    FunctionString = String.Format("{0:f3} + sin({1:f3} * x) * exp({2:f3} / x)",
                            result7.Item1, result7.Item2, result7.Item3);
                    ApproximationFunction = (x1) => result7.Item1
                            + Math.Sin(result7.Item2 * x1) * Math.Exp(result7.Item3 / x1);*/
                    double mean = MathNet.Numerics.Statistics.Statistics.Mean(y);
                    double first = y[0];
                    double last = y[y.Length - 1];
                    double[] y1 = new double[y.Length];
                    for (int i = 0; i < y.Length; i++)
                    {
                        y1[i] = y[i] - last;
                    }
                    Complex[] y2 = new Complex[y.Length];
                    y2 = y1.Select(yy => new Complex(yy, 0)).ToArray();
                    Fourier.Forward(y2);
                    double[] scale = Fourier.FrequencyScale(y.Length, (y.Length + 2) * 2);
                    double a1 = Math.Abs(y2[1].Magnitude);
                    double b1 = scale[1];
                    for (int i = 2; i < y2.Length / 2; i++)
                    {
                        if (Math.Abs(y2[i].Magnitude) > a1)
                        {
                            a1 = 2 * y2[i].Magnitude;
                            b1 = scale[i];
                        }
                    }
                    //a1 = 50* (Statistics.Maximum(y1) - Statistics.Minimum(y1));

                    double c1 = Math.Abs(Math.Abs(y1[y1.Length - 1] - last) / a1);
                    /*Console.WriteLine(y1.Aggregate("", (str, yy) => str + " " + yy));
                    Console.WriteLine(y2.Select(yy => yy.Magnitude)
                            .Aggregate("", (str, yy) => str + " " + yy));
                    Console.WriteLine(scale.Aggregate("", (str, yy) => str + " " + yy));
                    Console.WriteLine(a1 + " " + b1 + " " + c1);
                    Console.WriteLine("---------------------");*/

                    Tuple<double, double, double> result7 = Fit.Curve(x, y1,
                        (x1, a, b, c) => a * Math.Exp(-x1 / c) * Math.Sin(Math.PI * (x1 - x[0]) / b),
                         a1, b1, c1);
                    // Console.WriteLine(Statistics.Maximum(y1) + " " + Statistics.Minimum(y1));
                    //FunctionString = String.Format(
                    //        "{0:f3} + {1:f3}) * sin({3:f3} * (x - {4:f3}) / {5:f3})",
                    //        last, result7.Item1, result7.Item3, Math.PI, x[0], result7.Item2);
                    ApproximationFunction = (x1) => last + result7.Item1
                            * Math.Exp(-x1 / result7.Item3)
                            * Math.Sin(Math.PI * (x1 - x[0]) / result7.Item2);
                    double[] res_7 = Results(ApproximationFunction);
                    double StError_7 = GoodnessOfFit.PopulationStandardError(res_7, y);
                    double R_7 = GoodnessOfFit.R(res_7, y);
                    double RSquared_7 = GoodnessOfFit.RSquared(res_7, y);
                    double RSquare_7 = CalculateRSquare(x01, y, ApproximationFunction);
                    int n7 = y.Length;
                    int k7 = 5;  // кол-во параметров в функции
                    double Kds_7 = CalculateKds(n7, k7, RSquare_7);
                    double AIC7 = CalculateAIC(x01, y, ApproximationFunction, k7);
                    FunctionString = String.Format(
                            "{0:f2} + {1:f2} * sin({3:f2} * (x - {4:f2}) / {5:f2})  \n" +
                            "Rsqr = {6:f4} // Кд = {7:f2} // Кдc = {8:f2} // AIC = {9:f2} " +
                            "// StError = {10:f2} // R = {11:f2}",
                            last, result7.Item1, result7.Item3, Math.PI, x[0], result7.Item2,
                            RSquared_7, RSquare_7, Kds_7, AIC7, StError_7, R_7);
                    break;
                case "Сходящийся график 2":
                    /*Tuple<double, double, double> result7 = Fit.Curve(x, y,
                            (x1, a, b, c) => a + Math.Sin(x1 * b) * Math.Exp(c / x1),
                            MathNet.Numerics.Statistics.Statistics.Mean(x), 0.01,
                            -Math.Abs(x[x.Length - 1]) * (x.Length + 2));
                    FunctionString = String.Format("{0:f3} + sin({1:f3} * x) * exp({2:f3} / x)",
                            result7.Item1, result7.Item2, result7.Item3);
                    ApproximationFunction = (x1) => result7.Item1
                            + Math.Sin(result7.Item2 * x1) * Math.Exp(result7.Item3 / x1);*/
                    double mean1 = MathNet.Numerics.Statistics.Statistics.Mean(y);
                    double first1 = y[0];
                    double last1 = y[y.Length - 1];
                    double[] y5 = new double[y.Length];
                    for (int i = 0; i < y.Length; i++)
                    {
                        y5[i] = y[i] - last1;
                    }
                    //Console.WriteLine(y.Aggregate("", (str, yy) => str + " " + yy));
                    //Console.WriteLine(y5.Aggregate("", (str, yy) => str + " " + yy));
                    double a2 = 30 * (Statistics.Maximum(y) - Statistics.Minimum(y));

                    Tuple<double, double, double> result8 = Fit.Curve(x, y5,
                        (x1, a, b, c) => a * Math.Exp(-x1 / c) * Math.Sin(Math.PI * (x1 - x[0]) / b),
                         a2, 1, 1);
                    // Console.WriteLine(Statistics.Maximum(y1) + " " + Statistics.Minimum(y1));
                    //FunctionString = String.Format(
                    //        "{0:f3} + {1:f3} * exp(-x / {2:f3}) * sin({3:f3} * (x - {4:f3}) / {5:f3})",
                    //        last1, result8.Item1, result8.Item3, Math.PI, x[0], result8.Item2);
                    ApproximationFunction = (x1) => last1 + result8.Item1
                            * Math.Exp(-x1 / result8.Item3)
                            * Math.Sin(Math.PI * (x1 - x[0]) / result8.Item2);
                    double[] res_8 = Results(ApproximationFunction);
                    double StError_8 = GoodnessOfFit.PopulationStandardError(res_8, y);
                    double R_8 = GoodnessOfFit.R(res_8, y);
                    double RSquared_8 = GoodnessOfFit.RSquared(res_8, y);
                    double RSquare_8 = CalculateRSquare(x01, y, ApproximationFunction);
                    int n8 = y.Length;
                    int k8 = 6;  // кол-во параметров в функции
                    double Kds8 = CalculateKds(n8, k8, RSquare_8);
                    double AIC8 = CalculateAIC(x01, y, ApproximationFunction, k8);
                    FunctionString = String.Format(
                            "{0:f2}+{1:f2}*exp(-x/{2:f2})*sin({3:f2}*(x - {4:f2})/{5:f2}) \n" +
                            "Rsqr = {6:f4} // Кд = {7:f2} // Кдс = {8:f2} // AIC = {9:f2} " +
                            "// StError = {10:f2} // R = {11:f2}",
                            last1, result8.Item1, result8.Item3, Math.PI, x[0], result8.Item2,
                            RSquared_8, RSquare_8, Kds8, AIC8, StError_8, R_8);
                    break;

                case "Возрастающая экспонента":

                    double[] y10 = new double[y.Length];
                    double mean4 = MathNet.Numerics.Statistics.Statistics.Mean(y);
                    double mean5 = Statistics.Maximum(y);
                    Console.WriteLine(mean5);


                    try
                    {
                        Tuple<double, double> result10 = Fit.Curve(x, y10,
                        (x1, a, b) => a * (1 - Math.Pow(x1, b)),
                         mean5, -100);
                        //Console.WriteLine(mean4);
                        //FunctionString = String.Format(
                        //        "{0:f3} + {1:f3} * exp(-x / {2:f3})",
                        //        result10.Item1, result10.Item2, result10.Item3);
                        ApproximationFunction = (x1) => result10.Item1
                                * (1 - Math.Pow(x1, result10.Item2));
                        double[] res_10 = Results(ApproximationFunction);
                        double StError_10 = GoodnessOfFit.PopulationStandardError(res_10, y);
                        double R_10 = GoodnessOfFit.R(res_10, y);
                        double RSquared_10 = GoodnessOfFit.RSquared(res_10, y);
                        double RSquare_10 = CalculateRSquare(x01, y, ApproximationFunction);
                        int n10 = y.Length;
                        int k10 = 2;  // кол-во параметров в функции
                        double Kds10 = CalculateKds(n10, k10, RSquare_10);
                        double AIC10 = CalculateAIC(x01, y, ApproximationFunction, k10);
                        FunctionString = String.Format("{0:f3} * (1 - x^({1:f3}))    \n" +
                            "Кд = {2:f2} // Кдс = {3:f2} // AIC = {4:f2} " +
                            "// StError = {5:f2} // R = {6:f2} // Rsqr = {7:f4}",
                               result10.Item1, result10.Item2,
                               RSquare_10, Kds10, AIC10, StError_10, R_10, RSquared_10);
                    }
                    catch (MathNet.Numerics.Optimization.MaximumIterationsException)
                    {
                        ApproximationFunction = (x1) => Double.NaN;
                    }
                    break;
                case "Убывающая экспонента":
                    double[] y9 = new double[y.Length];
                    double mean2 = MathNet.Numerics.Statistics.Statistics.Mean(y);
                    //double mean3 = mean2 / 2;
                    double mean3 = Statistics.Minimum(y) / 2;
                    Console.WriteLine(mean3);
                    try
                    {
                        Tuple<double, double, double> result9 = Fit.Curve(x, y9,
                        /*(x1, a, b, c) => a + b * (1 - Math.Exp(-x1 / c)),
                         mean2, -50000, 50000);*/
                        (x1, a, b, c) => a + (b / x1) + (c / Math.Pow(x1, 2)),
                         mean3, mean3, 20000);
                        //Console.WriteLine(mean2);
                        //FunctionString = String.Format(
                        //        "{0:f3} + {1:f3} *(1 - exp(-x / {2:f3}))",
                        //        result9.Item1, result9.Item2, result9.Item3);
                        ApproximationFunction = (x1) => result9.Item1
                                + (result9.Item2 / x1) + (result9.Item3 / Math.Pow(x1, 2));
                        double[] res_9 = Results(ApproximationFunction);
                        double StError_9 = GoodnessOfFit.PopulationStandardError(res_9, y);
                        double R_9 = GoodnessOfFit.R(res_9, y);
                        double RSquared_9 = GoodnessOfFit.RSquared(res_9, y);
                        double RSquare_9 = CalculateRSquare(x01, y, ApproximationFunction);
                        int n9 = y.Length;
                        int k9 = 3;  // кол-во параметров в функции
                        double Kds9 = CalculateKds(n9, k9, RSquare_9);
                        double AIC9 = CalculateAIC(x01, y, ApproximationFunction, k9);
                        FunctionString = String.Format(
                        /*"{0:f3} + {1:f3} *(1 - exp(-x / {2:f3}))       // Кд = {3:f2} // Кдс = {4:f2} // AIC = {5:f2}",
                        result9.Item1, result9.Item2, result9.Item3, RSquare_9, Kds9, AIC9);*/
                        "{0:f3} + ({1:f3}/x) + ({2:f3}/x^2)  \n" +
                        "Rsqr = {3:f4} // Кд = {4:f2} // Кдс = {5:f2} // AIC = {6:f2} " +
                        "// StError = {7:f2} // R = {8:f2}",
                                result9.Item1, result9.Item2, result9.Item3,
                                RSquared_9, RSquare_9, Kds9, AIC9, StError_9, R_9);
                    }
                    catch (MathNet.Numerics.Optimization.MaximumIterationsException)
                    {
                        ApproximationFunction = (x1) => Double.NaN;
                    }

                    break;

                default:
                    return;
            }
            //Console.WriteLine(ApproximationFunction);
            for (double a = 3; a <= 15; a += 0.01)
            {
                list.Add(a, ApproximationFunction.Invoke(a));
            }

            // Создадим кривую 
            if (ApproximationFunction == null)
            {
                MessageBox.Show("Не удалось подобрать коэффициенты. Попробуйте" +
                    "выбрать другую точку", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                LineItem myline = pane.AddCurve(
                        ApproximationPointsType[j] + ": " + FunctionString,
                        list, ApproximationColors[j], SymbolType.None);
                myline.Line.Width = 3.0f;

                //Добавление текста на саму панель zedgraph
                double RSquare = CalculateRSquare(x01, y, ApproximationFunction);

                /*
                TextObj text = new TextObj(String.Format("Кд = {0:f2}", RSquare), x01[0],
                    ApproximationFunction.Invoke(x01[0]));
                text.FontSpec.Border.IsVisible = false;
                text.Tag = "2";
                pane.GraphObjList.Add(text);*/


                //myline.Line.IsVisible = true;
                // Устанавливаем интересующий нас интервал по оси X
                pane.XAxis.Scale.Min = 3;
                pane.XAxis.Scale.Max = 15;
                // Сетка
                // Включаем отображение сетки напротив крупных рисок по оси X
                pane.XAxis.MajorGrid.IsVisible = true;
                // Задаем вид пунктирной линии для крупных рисок по оси X:
                // Длина штрихов равна 10 пикселям, ... 
                pane.XAxis.MajorGrid.DashOn = 10;
                // затем 5 пикселей - пропуск
                pane.XAxis.MajorGrid.DashOff = 5;
                // Включаем отображение сетки напротив крупных рисок по оси Y
                pane.YAxis.MajorGrid.IsVisible = true;
                // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
                pane.YAxis.MajorGrid.DashOn = 10;
                pane.YAxis.MajorGrid.DashOff = 5;
                // Включаем отображение сетки напротив мелких рисок по оси X
                pane.YAxis.MinorGrid.IsVisible = true;
                // Задаем вид пунктирной линии для крупных рисок по оси Y: 
                // Длина штрихов равна одному пикселю, ... 
                pane.YAxis.MinorGrid.DashOn = 1;
                // затем 2 пикселя - пропуск
                pane.YAxis.MinorGrid.DashOff = 2;
                // Включаем отображение сетки напротив мелких рисок по оси Y
                pane.XAxis.MinorGrid.IsVisible = true;
                // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
                pane.XAxis.MinorGrid.DashOn = 1;
                pane.XAxis.MinorGrid.DashOff = 2;


                //pane.AxisChange();
                //pane.i
                //pane.
                // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
                zedGraph.AxisChange();

                // Обновляем график
                zedGraph.Invalidate();
                //pane_d = pane;
            }
        }

        private void get_value()
        {
            if (flag)
            {

                foreach (DirectoryInfo subdirectory in directories)
                {
                    data = new Data();
                    Data.file_path = subdirectory.FullName;
                    try
                    {
                        data.openfile_res1();
                        data.openfile_res2();

                        char[] ch = subdirectory.Name.ToCharArray();
                        if (subdirectory.Name.Length == 4)
                        {
                            //MessageBox.Show(data.temp_elem.all_elements.Count.ToString());
                            nrc = ch[subdirectory.Name.Length - 1].ToString();

                            int ii = Convert.ToInt16(nrc);

                            elements[ii - 3] = data.temp_elem.count_of_elements;

                            x = Convert.ToDouble(tempx);
                            y = Convert.ToDouble(tempy);
                            cm_m_stress(ii - 3, x, y);
                            cps_m_stress(ii - 3, x, y);
                            ke_m_stress(ii - 3, x, y);
                            movement(ii - 3, x, y);
                            //MessageBox.Show(ch[subdirectory.Name.Length-1].ToString());
                        }
                        else
                        {
                            nrc = ch[subdirectory.Name.Length - 2].ToString() + ch[subdirectory.Name.Length - 1].ToString();
                            int ii = Convert.ToInt16(nrc);

                            elements[ii - 3] = data.temp_elem.count_of_elements;
                            x = Convert.ToDouble(tempx);
                            y = Convert.ToDouble(tempy);
                            cm_m_stress(ii - 3, x, y);
                            cps_m_stress(ii - 3, x, y);
                            ke_m_stress(ii - 3, x, y);
                            movement(ii - 3, x, y);
                        }
                    }
                    catch (Exception e)
                    {
                        if (e is FormatException || e is IOException)
                        {
                            Console.WriteLine(e.ToString());
                        }
                        else
                        {
                            throw e;
                        }
                    }

                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            get_value();
            DrawGraph(0, "Сходимость напряжения по Х", "n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            get_value();
            DrawGraph(1, "Сходимость напряжения по Y", "n");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            get_value();
            DrawGraph(2, "Касательное", "n");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            get_value();
            DrawGraph(3, "1-е главное напряжение", "n");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            get_value();
            DrawGraph(4, "2-е главное напряжение", "n");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            get_value();
            DrawGraph(5, "Эквивалентное", "n");
            //DrawAllGraph();
        }

        private void drawMoveXGraph()
        {
            get_value();
            zedGraph.GraphPane.CurveList.Clear();
            ClearCheck();
            CurrentGraphType = GraphType.MoveX;
            DrawGraph(0, "Перемещение по Х", "х");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            drawMoveXGraph();
        }

        private void drawMoveYGraph()
        {
            get_value();
            zedGraph.GraphPane.CurveList.Clear();
            ClearCheck();
            CurrentGraphType = GraphType.MoveY;
            DrawGraph(0, "Перемещение по Y", "у");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            drawMoveYGraph();
            zedGraph.AxisChange();

            // Обновляем график
            zedGraph.Invalidate();
            //pane_d = pane;
        }

        private void ClearCheck()
        {
            /*checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            checkBox6.Checked = false;*/
            comboBox1.SelectedItem = comboBox1.Items[0];
            comboBox2.SelectedItem = comboBox2.Items[0];
            comboBox3.SelectedItem = comboBox3.Items[0];
            comboBox4.SelectedItem = comboBox4.Items[0];
            comboBox5.SelectedItem = comboBox5.Items[0];
            comboBox6.SelectedItem = comboBox6.Items[0];
        }

        private void DrawAllGraph(string typemove)
        {
            // Получим панель для рисования
            //GraphPane pane = zedGraph.GraphPane;
            pane = zedGraph.GraphPane;
            // !!!
            // Изменим тест надписи по оси XМ
            pane.XAxis.Title.Text = "NRC\nКоличество элементов";



            // Изменим параметры шрифта для оси X
            //pane.XAxis.Title.FontSpec.IsUnderline = true;
            //pane.XAxis.Title.FontSpec.IsBold = false;
            //pane.XAxis.Title.FontSpec.FontColor = Color.Blue;

            // Изменим текст по оси Y
            pane.YAxis.Title.Text = "Напряжение";

            // Изменим текст заголовка графика
            pane.Title.Text = "ЕСК";

            // В параметрах шрифта сделаем заливку красным цветом
            pane.Title.FontSpec.Fill.IsVisible = true;

            // Сделаем шрифт не полужирным
            pane.Title.FontSpec.IsBold = false;

            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane.CurveList.Clear();

            // Создадим список точек
            PointPairList list1 = new PointPairList();
            PointPairList list2 = new PointPairList();
            PointPairList list3 = new PointPairList();
            PointPairList list4 = new PointPairList();
            PointPairList list5 = new PointPairList();
            PointPairList list6 = new PointPairList();
            //MessageBox.Show(m_type_calc);


            if (typemove == "n")
            {
                switch (m_type_calc)
                {
                    case "cpc":
                        for (int a = 0; a < 10; ++a)
                        {
                            list1.Add(a + 3, cpc_m[a, 0]);
                            list2.Add(a + 3, cpc_m[a, 1]);
                            list3.Add(a + 3, cpc_m[a, 2]);
                            list4.Add(a + 3, cpc_m[a, 3]);
                            list5.Add(a + 3, cpc_m[a, 4]);
                            list6.Add(a + 3, cpc_m[a, 5]);

                        }

                        break;
                    case "cp":
                        for (int a = 0; a < 10; ++a)
                        {
                            //list.Add(a + 3, cm_m[a, 0]);
                            list1.Add(a + 3, cm_m[a, 0]);
                            list2.Add(a + 3, cm_m[a, 1]);
                            list3.Add(a + 3, cm_m[a, 2]);
                            list4.Add(a + 3, cm_m[a, 3]);
                            list5.Add(a + 3, cm_m[a, 4]);
                            list6.Add(a + 3, cm_m[a, 5]);
                        }
                        break;
                    case "ke":
                        for (int a = 0; a < 10; ++a)
                        {
                            //list.Add(a + 3, ke_m[a, 0]);
                            list1.Add(a + 3, ke_m[a, 0]);
                            list2.Add(a + 3, ke_m[a, 1]);
                            list3.Add(a + 3, ke_m[a, 2]);
                            list4.Add(a + 3, ke_m[a, 3]);
                            list5.Add(a + 3, ke_m[a, 4]);
                            list6.Add(a + 3, ke_m[a, 5]);
                        }
                        break;


                }
            }
            else
            {
                for (int a = 0; a < 10; ++a)
                {
                    list1.Add(a + 3, movx[a, 0]);
                    list2.Add(a + 3, movy[a, 0]);
                }
            }


            // Заполняем список точек
            //for (double x = xmin; x <= xmax; x += 0.01)
            //{
            //list.Add(x, f(x));
            //}

            // Создадим кривую
            if (typemove == "n")
            {
                LineItem myline = pane.AddCurve("", list1, Color.Red, SymbolType.Circle);
                //myline.Line.IsVisible = false;
                myline.Symbol.Fill.Type = FillType.Solid;
                myline.Symbol.Size = 7;
                myline.Line.Width = 3.0f;
                LineItem myline1 = pane.AddCurve("", list2, Color.Green, SymbolType.Circle);
                //myline1.Line.IsVisible = false;
                myline1.Symbol.Fill.Type = FillType.Solid;
                myline1.Symbol.Size = 7;
                myline1.Line.Width = 3.0f;
                LineItem myline2 = pane.AddCurve("", list3, Color.Yellow, SymbolType.Circle);
                //myline2.Line.IsVisible = false;
                myline2.Symbol.Fill.Type = FillType.Solid;
                myline2.Symbol.Size = 7;
                myline2.Line.Width = 3.0f;
                LineItem myline3 = pane.AddCurve("", list4, Color.Blue, SymbolType.Circle);
                //myline3.Line.IsVisible = false;
                myline3.Symbol.Fill.Type = FillType.Solid;
                myline3.Symbol.Size = 7;
                myline3.Line.Width = 3.0f;
                LineItem myline4 = pane.AddCurve("", list5, Color.Chocolate, SymbolType.Circle);
                //myline4.Line.IsVisible = false;
                myline4.Symbol.Fill.Type = FillType.Solid;
                myline4.Symbol.Size = 7;
                myline4.Line.Width = 3.0f;
                LineItem myline5 = pane.AddCurve("", list6, Color.Gray, SymbolType.Circle);
                //myline5.Line.IsVisible = false;
                myline5.Symbol.Fill.Type = FillType.Solid;
                myline5.Symbol.Size = 7;
                myline5.Line.Width = 3.0f;


                myline.Line.IsVisible = интерполяцияПрямойToolStripMenuItem.Checked;
                myline1.Line.IsVisible = интерполяцияПрямойToolStripMenuItem.Checked;
                myline2.Line.IsVisible = интерполяцияПрямойToolStripMenuItem.Checked;
                myline3.Line.IsVisible = интерполяцияПрямойToolStripMenuItem.Checked;
                myline4.Line.IsVisible = интерполяцияПрямойToolStripMenuItem.Checked;
                myline5.Line.IsVisible = интерполяцияПрямойToolStripMenuItem.Checked;
            }
            else
            {
                LineItem myline = pane.AddCurve("", list1, Color.Red, SymbolType.Circle);
                myline.Line.Width = 3.0f;
                myline.Symbol.Fill.Type = FillType.Solid;
                myline.Symbol.Size = 7;
                LineItem myline1 = pane.AddCurve("", list2, Color.Green, SymbolType.Circle);
                myline1.Line.Width = 3.0f;
                myline1.Symbol.Fill.Type = FillType.Solid;
                myline1.Symbol.Size = 7;
                myline.Line.IsVisible = интерполяцияПрямойToolStripMenuItem.Checked;
                myline1.Line.IsVisible = интерполяцияПрямойToolStripMenuItem.Checked;
            }
            //myline.Line.Width = 3.0f;
            // Устанавливаем интересующий нас интервал по оси X
            pane.XAxis.Scale.Min = 3;
            pane.XAxis.Scale.Max = 12;





            // Сетка
            // Включаем отображение сетки напротив крупных рисок по оси X
            pane.XAxis.MajorGrid.IsVisible = true;
            // Задаем вид пунктирной линии для крупных рисок по оси X:
            // Длина штрихов равна 10 пикселям, ... 
            pane.XAxis.MajorGrid.DashOn = 10;
            // затем 5 пикселей - пропуск
            pane.XAxis.MajorGrid.DashOff = 5;
            // Включаем отображение сетки напротив крупных рисок по оси Y
            pane.YAxis.MajorGrid.IsVisible = true;
            // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
            pane.YAxis.MajorGrid.DashOn = 10;
            pane.YAxis.MajorGrid.DashOff = 5;
            // Включаем отображение сетки напротив мелких рисок по оси X
            pane.YAxis.MinorGrid.IsVisible = true;
            // Задаем вид пунктирной линии для крупных рисок по оси Y: 
            // Длина штрихов равна одному пикселю, ... 
            pane.YAxis.MinorGrid.DashOn = 1;
            // затем 2 пикселя - пропуск
            pane.YAxis.MinorGrid.DashOff = 2;
            // Включаем отображение сетки напротив мелких рисок по оси Y
            pane.XAxis.MinorGrid.IsVisible = true;
            // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
            pane.XAxis.MinorGrid.DashOn = 1;
            pane.XAxis.MinorGrid.DashOff = 2;



            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            zedGraph.AxisChange();

            // Обновляем график
            zedGraph.Invalidate();
        }

        public void drawForceECKGraph()
        {
            get_value();
            ClearCheck();
            CurrentGraphType = GraphType.ForceECK;
            DrawAllGraph("n");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            drawForceECKGraph();

        }

        private void button11_Click(object sender, EventArgs e)
        {
            drawMoveECKGraph();
            zedGraph.AxisChange();

            // Обновляем график
            zedGraph.Invalidate();
            //pane_d = pane;
        }

        private void drawMoveECKGraph()
        {
            get_value();
            ClearCheck();
            CurrentGraphType = GraphType.MoveECK;
            DrawAllGraph("m");
        }

        private void SavePaneImage()
        {
            // ДИалог выбора имени файла создаем вручную
            SaveFileDialog dlg = new SaveFileDialog();
            GraphPane pane = zedGraph.MasterPane.PaneList[0];
            dlg.FileName = pane.Title.Text;
            dlg.Filter = "*.png|*.png|*.jpg; *.jpeg|*.jpg;*.jpeg|*.bmp|*.bmp|Все файлы|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // Получием панель по ее индексу

                // Получаем картинку, соответствующую панели
                Bitmap bmp = pane.GetImage();

                // Сохраняем картинку средствами класса Bitmap
                // Формат картинки выбирается исходя из имени выбранного файла
                if (dlg.FileName.EndsWith(".png"))
                {
                    bmp.Save(dlg.FileName, ImageFormat.Png);
                }
                else if (dlg.FileName.EndsWith(".jpg") || dlg.FileName.EndsWith(".jpeg"))
                {
                    bmp.Save(dlg.FileName, ImageFormat.Jpeg);
                }
                else if (dlg.FileName.EndsWith(".bmp"))
                {
                    bmp.Save(dlg.FileName, ImageFormat.Bmp);
                }
                else
                {
                    bmp.Save(dlg.FileName);
                }
            }
        }

        private void сохранитьГрафикиВWordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SavePaneImage();
        }

        private void открытьПоследнийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel2.Text = MainForm.LastFilePath;
            DirectoryInfo maindirectiry = new DirectoryInfo(MainForm.LastFilePath);
            //MainForm.LastFilePath = dlg.SelectedPath;
            //toolStripStatusLabel2.Text = MainForm.LastFilePath;
            if (maindirectiry.Exists)
            {
                directories = maindirectiry.GetDirectories();
                flag = true;
            }
        }

        private void Graphics_Load(object sender, EventArgs e)
        {

        }

        private void Graphics_FormClosed(object sender, FormClosedEventArgs e)
        {
            //AppConfigSettings.WriteSetting("Посдледний открытый проект для графиков", LastFilePath);
        }

        void CopyFile(string sourcefn, string destinfn)
        {
            FileInfo fn = new FileInfo(sourcefn);
            fn.CopyTo(destinfn, true);
        }

        private void wordmake(string filen)
        {
            Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();

            //Загружаем документ
            Microsoft.Office.Interop.Word.Document doc = null;

            object fileName = filen;
            object falseValue = false;
            object trueValue = true;
            object missing = Type.Missing;

            doc = app.Documents.Open(ref fileName, ref missing, ref trueValue,
            ref missing, ref missing, ref missing, ref missing, ref missing,
            ref missing, ref missing, ref missing, ref missing, ref missing,
            ref missing, ref missing, ref missing);
            app.ActiveWindow.View.ReadingLayout = false;
            //Указываем таблицу в которую будем помещать данные (таблица должна существовать в шаблоне документа!)
            //Microsoft.Office.Interop.Word.Table tbl = app.ActiveDocument.Tables[1];

            get_value();
            DrawGraphD(0, "Сходимость напряжения по Х", "n");
            Clipboard.SetImage(make_img());
            doc.Paragraphs[2].Range.Paste();
            DrawGraphD(1, "Сходимость напряжения по Y", "n");
            Clipboard.SetImage(make_img());
            doc.Paragraphs[4].Range.Paste();
            get_value();
            DrawGraphD(2, "Касательное", "n");
            Clipboard.SetImage(make_img());
            doc.Paragraphs[6].Range.Paste();
            get_value();
            DrawGraphD(3, "1-е главное напряжение", "n");
            Clipboard.SetImage(make_img());
            doc.Paragraphs[8].Range.Paste();
            get_value();
            DrawGraphD(4, "2-е главное напряжение", "n");
            Clipboard.SetImage(make_img());
            doc.Paragraphs[10].Range.Paste();
            get_value();
            DrawGraphD(5, "Эквивалентное", "n");
            Clipboard.SetImage(make_img());
            doc.Paragraphs[12].Range.Paste();
            get_value();
            DrawAllGraphD("n");
            Clipboard.SetImage(make_img());
            doc.Paragraphs[14].Range.Paste();
            get_value();
            DrawGraphD(0, "Перемещение по Х", "х");
            Clipboard.SetImage(make_img());
            doc.Paragraphs[16].Range.Paste();
            get_value();
            DrawGraphD(0, "Перемещение по Y", "у");
            Clipboard.SetImage(make_img());
            doc.Paragraphs[18].Range.Paste();
            get_value();
            DrawAllGraphD("m");
            Clipboard.SetImage(make_img());
            doc.Paragraphs[20].Range.Paste();
            //doc.Paragraphs[2].Range.Paste();
            //doc.Paragraphs[3].Range.Paste();
            //doc.Paragraphs[3].Range.Paste();
            //Заполняем в таблицу - 10 записей.
            int a = 0;
            Microsoft.Office.Interop.Word.Table tbl = app.ActiveDocument.Tables[1];
            for (int i = 1; i < 11; i++)
            {
                //tbl.Rows.Add(ref missing);//Добавляем в таблицу строку.
                //Обычно саздаю только строку с заголовками и одну пустую для данных.
                for (int j = 0; j < 6; ++j)
                {
                    switch (m_type_calc)
                    {
                        case "cpc":

                            tbl.Rows[i + 1].Cells[j + 2].Range.Text = Math.Round(cpc_m[a, j], 2).ToString();


                            break;
                        case "cp":

                            tbl.Rows[i + 1].Cells[j + 2].Range.Text = Math.Round(cm_m[a, j], 2).ToString();

                            break;
                        case "ke":

                            tbl.Rows[i + 1].Cells[j + 2].Range.Text = Math.Round(ke_m[a, j], 2).ToString();

                            break;

                    }
                    //tbl.Rows[i + 1].Cells[j+2].Range.Text = "Запись №" + i.ToString();
                    //tbl.Rows[i + 1].Cells[3].Range.Text = "Запись №" + i.ToString();
                    //tbl.Rows[i + 1].Cells[4].Range.Text = "Запись №" + i.ToString();
                }
                if (a < 10)
                    ++a;

            }
            //app.DisplayAlerts = Microsoft.Office.Interop.Word.WdAlertLevel.wdAlertsNone;
            //doc.Save();
            //fileName = @"C:\document.docx";
            object fileFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocument, lockComments = Type.Missing, password = Type.Missing,
            addToRecentFile = Type.Missing, writePassword = Type.Missing,
            readOnlyRecomended = Type.Missing,
            embedTrueTypeFonts = Type.Missing,
            saveNativePictureFormat = Type.Missing,
            saveFormsData = Type.Missing,
            saveAsAOCELetter = Type.Missing;
            //doc.SaveAs(ref fileName, ref fileFormat, ref lockComments, ref password, ref addToRecentFile, ref writePassword, ref readOnlyRecomended, ref embedTrueTypeFonts,  ref saveNativePictureFormat, ref saveFormsData,  ref saveAsAOCELetter);
            //doc.Close(ref wdsavechanges, ref docformat, ref rout);
            //app.Quit();
            //Открываем документ для просмотра.
            //app.Visible = true;
            SaveFileDialog dlg = new SaveFileDialog();
            //doc.Close();
            //app.Quit();

            dlg.FileName = "Графики сходимости напряжений";
            dlg.Filter = "*.doc|*.docx|Все файлы|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                fileName = dlg.FileName;
                //doc.SaveAs(ref fileName, ref fileFormat, ref lockComments, ref password, ref addToRecentFile, ref writePassword, ref readOnlyRecomended, ref embedTrueTypeFonts, ref saveNativePictureFormat, ref saveFormsData, ref saveAsAOCELetter);
                //fileName = dlg.
                doc.Application.ActiveDocument.SaveAs(ref fileName,
                ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing);
            }

            doc.Close(0);
            app.Quit();

        }


        private Bitmap make_img()
        {
            Bitmap bmp = pane_d.GetImage();
            Bitmap to_save = new Bitmap(bmp, 500, 300);
            return to_save;
        }

        private double[] Results(Func<double, double> func)
        {
            double[] res = new double[10];

            for (int i = 0; i < 10; i++)
            {
                res[i] = func.Invoke((double)i + 3);
                //Console.WriteLine(res[i]);
            }
            return res;
        }

        private double CalculateRSquare(int[] x, double[] y, Func<double, double> func)
        {
            double mean = y.Mean();
            /*for (int i = 0; i < y.Length; i++)
            {
                Console.Write(x[i] + " ");
            }
            Console.WriteLine();
            for (int i = 0; i < y.Length; i++)
            {
                Console.Write(y[i] + " ");
            }
            Console.WriteLine();
            for (int i = 3; i < y.Length + 3; i++)
            {
                Console.Write(func.Invoke(i) + " ");
            }
            Console.WriteLine();*/
            return 1 - x.Select(i => Math.Pow(y[i - 3] - func.Invoke((double)i), 2)).Sum()
                / y.Select(yi => Math.Pow(yi - mean, 2)).Sum();

        }

        private double CalculateKds(int n, int k, double Kd)
        {

            return 1 - (1 - Kd) * (n - 1) / (n - k);
        }

        private double CalculateAIC(int[] x, double[] y, Func<double, double> func, int k)
        {
            int n = y.Length;
            return 2 * k / n + Math.Log(x.Select(i => Math.Pow(y[i - 3] - func.Invoke((double)i), 2)).Sum() / n);
        }
        private void DrawGraphD(int j, string stresstype, string typemove)
        {
            // Получим панель для рисования
            //GraphPane pane = zedGraph.GraphPane;
            pane_d = new GraphPane();
            //pane = new GraphPane();
            // !!!
            // Изменим тест надписи по оси X
            pane_d.XAxis.Title.Text = "NRC";

            // Изменим параметры шрифта для оси X
            //pane.XAxis.Title.FontSpec.IsUnderline = true;
            //pane.XAxis.Title.FontSpec.IsBold = false;
            //pane.XAxis.Title.FontSpec.FontColor = Color.Blue;

            // Изменим текст по оси Y
            pane_d.YAxis.Title.Text = "Напряжение";

            // Изменим текст заголовка графика
            pane_d.Title.Text = stresstype;

            // В параметрах шрифта сделаем заливку красным цветом
            //pane.Title.FontSpec.Fill.Brush = new SolidBrush(Color.Red);
            pane_d.Title.FontSpec.Fill.IsVisible = true;

            // Сделаем шрифт не полужирным
            pane_d.Title.FontSpec.IsBold = false;


            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane_d.CurveList.Clear();

            // Создадим список точек
            list = new PointPairList();
            //MessageBox.Show(m_type_calc);

            if (typemove == "n")
            {

                switch (m_type_calc)
                {
                    case "cpc":
                        for (int a = 0; a < 10; ++a)
                        {
                            list.Add(a + 3, cpc_m[a, j]);
                        }

                        break;
                    case "cp":
                        for (int a = 0; a < 10; ++a)
                        {
                            list.Add(a + 3, cm_m[a, j]);
                        }
                        break;
                    case "ke":
                        for (int a = 0; a < 10; ++a)
                        {
                            list.Add(a + 3, ke_m[a, j]);
                        }
                        break;

                }
            }
            else
            {
                if (typemove == "х")
                {
                    for (int a = 0; a < 10; ++a)
                    {
                        list.Add(a + 3, movx[a, 0]);
                    }
                }
                else
                {
                    for (int a = 0; a < 10; ++a)
                    {
                        list.Add(a + 3, movy[a, 0]);
                    }
                }
            }


            // Заполняем список точек
            //for (double x = xmin; x <= xmax; x += 0.01)
            //{
            //list.Add(x, f(x));
            //}

            // Создадим кривую
            LineItem myline = pane_d.AddCurve("", list, Color.Blue, SymbolType.Circle);
            myline.Line.Width = 3.0f;
            // Устанавливаем интересующий нас интервал по оси X
            pane_d.XAxis.Scale.Min = 3;
            pane_d.XAxis.Scale.Max = 12;





            // Сетка
            // Включаем отображение сетки напротив крупных рисок по оси X
            pane_d.XAxis.MajorGrid.IsVisible = true;
            // Задаем вид пунктирной линии для крупных рисок по оси X:
            // Длина штрихов равна 10 пикселям, ... 
            pane_d.XAxis.MajorGrid.DashOn = 10;
            // затем 5 пикселей - пропуск
            pane_d.XAxis.MajorGrid.DashOff = 5;
            // Включаем отображение сетки напротив крупных рисок по оси Y
            pane_d.YAxis.MajorGrid.IsVisible = true;
            // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
            pane_d.YAxis.MajorGrid.DashOn = 10;
            pane_d.YAxis.MajorGrid.DashOff = 5;
            // Включаем отображение сетки напротив мелких рисок по оси X
            pane_d.YAxis.MinorGrid.IsVisible = true;
            // Задаем вид пунктирной линии для крупных рисок по оси Y: 
            // Длина штрихов равна одному пикселю, ... 
            pane_d.YAxis.MinorGrid.DashOn = 1;
            // затем 2 пикселя - пропуск
            pane_d.YAxis.MinorGrid.DashOff = 2;
            // Включаем отображение сетки напротив мелких рисок по оси Y
            pane_d.XAxis.MinorGrid.IsVisible = true;
            // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
            pane_d.XAxis.MinorGrid.DashOn = 1;
            pane_d.XAxis.MinorGrid.DashOff = 2;

            pane_d.AxisChange();
            //pane.AxisChange();
            //pane.i
            //pane.
            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            //zedGraph.AxisChange();

            // Обновляем график
            //zedGraph.Invalidate();
            //pane_d = pane;
        }


        private void DrawAllGraphD(string typemove)
        {
            // Получим панель для рисования
            //GraphPane pane = zedGraph.GraphPane;
            pane_d = new GraphPane();
            // !!!
            // Изменим тест надписи по оси X
            pane_d.XAxis.Title.Text = "NRC";

            // Изменим параметры шрифта для оси X
            //pane.XAxis.Title.FontSpec.IsUnderline = true;
            //pane.XAxis.Title.FontSpec.IsBold = false;
            //pane.XAxis.Title.FontSpec.FontColor = Color.Blue;

            // Изменим текст по оси Y
            pane_d.YAxis.Title.Text = "Напряжение";

            // Изменим текст заголовка графика
            pane_d.Title.Text = "ЕСК";

            // В параметрах шрифта сделаем заливку красным цветом
            //pane.Title.FontSpec.Fill.Brush = new SolidBrush(Color.Red);
            pane_d.Title.FontSpec.Fill.IsVisible = true;

            // Сделаем шрифт не полужирным
            pane_d.Title.FontSpec.IsBold = false;


            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane_d.CurveList.Clear();

            // Создадим список точек
            PointPairList list1 = new PointPairList();
            PointPairList list2 = new PointPairList();
            PointPairList list3 = new PointPairList();
            PointPairList list4 = new PointPairList();
            PointPairList list5 = new PointPairList();
            PointPairList list6 = new PointPairList();
            //MessageBox.Show(m_type_calc);


            if (typemove == "n")
            {
                switch (m_type_calc)
                {
                    case "cpc":
                        for (int a = 0; a < 10; ++a)
                        {
                            list1.Add(a + 3, cpc_m[a, 0]);
                            list2.Add(a + 3, cpc_m[a, 1]);
                            list3.Add(a + 3, cpc_m[a, 2]);
                            list4.Add(a + 3, cpc_m[a, 3]);
                            list5.Add(a + 3, cpc_m[a, 4]);
                            list6.Add(a + 3, cpc_m[a, 5]);

                        }

                        break;
                    case "cp":
                        for (int a = 0; a < 10; ++a)
                        {
                            //list.Add(a + 3, cm_m[a, 0]);
                            list1.Add(a + 3, cm_m[a, 0]);
                            list2.Add(a + 3, cm_m[a, 1]);
                            list3.Add(a + 3, cm_m[a, 2]);
                            list4.Add(a + 3, cm_m[a, 3]);
                            list5.Add(a + 3, cm_m[a, 4]);
                            list6.Add(a + 3, cm_m[a, 5]);
                        }
                        break;
                    case "ke":
                        for (int a = 0; a < 10; ++a)
                        {
                            //list.Add(a + 3, ke_m[a, 0]);
                            list1.Add(a + 3, ke_m[a, 0]);
                            list2.Add(a + 3, ke_m[a, 1]);
                            list3.Add(a + 3, ke_m[a, 2]);
                            list4.Add(a + 3, ke_m[a, 3]);
                            list5.Add(a + 3, ke_m[a, 4]);
                            list6.Add(a + 3, ke_m[a, 5]);
                        }
                        break;

                }
            }
            else
            {
                for (int a = 0; a < 10; ++a)
                {
                    list1.Add(a + 3, movx[a, 0]);
                    list2.Add(a + 3, movy[a, 0]);
                }
            }


            // Заполняем список точек
            //for (double x = xmin; x <= xmax; x += 0.01)
            //{
            //list.Add(x, f(x));
            //}

            // Создадим кривую
            if (typemove == "n")
            {
                LineItem myline = pane_d.AddCurve("", list1, Color.Red, SymbolType.Circle);
                myline.Line.Width = 3.0f;
                LineItem myline1 = pane_d.AddCurve("", list2, Color.Green, SymbolType.Circle);
                myline1.Line.Width = 3.0f;
                LineItem myline2 = pane_d.AddCurve("", list3, Color.Yellow, SymbolType.Circle);
                myline2.Line.Width = 3.0f;
                LineItem myline3 = pane_d.AddCurve("", list4, Color.Blue, SymbolType.Circle);
                myline3.Line.Width = 3.0f;
                LineItem myline4 = pane_d.AddCurve("", list5, Color.Chocolate, SymbolType.Circle);
                myline4.Line.Width = 3.0f;
                LineItem myline5 = pane_d.AddCurve("", list6, Color.Gray, SymbolType.Circle);
                myline5.Line.Width = 3.0f;
            }
            else
            {
                LineItem myline = pane_d.AddCurve("", list1, Color.Red, SymbolType.Circle);
                myline.Line.Width = 3.0f;
                LineItem myline1 = pane_d.AddCurve("", list2, Color.Green, SymbolType.Circle);
                myline1.Line.Width = 3.0f;
            }
            //myline.Line.Width = 3.0f;
            // Устанавливаем интересующий нас интервал по оси X
            pane_d.XAxis.Scale.Min = 3;
            pane_d.XAxis.Scale.Max = 12;





            // Сетка
            // Включаем отображение сетки напротив крупных рисок по оси X
            pane_d.XAxis.MajorGrid.IsVisible = true;
            // Задаем вид пунктирной линии для крупных рисок по оси X:
            // Длина штрихов равна 10 пикселям, ... 
            pane_d.XAxis.MajorGrid.DashOn = 10;
            // затем 5 пикселей - пропуск
            pane_d.XAxis.MajorGrid.DashOff = 5;
            // Включаем отображение сетки напротив крупных рисок по оси Y
            pane_d.YAxis.MajorGrid.IsVisible = true;
            // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
            pane_d.YAxis.MajorGrid.DashOn = 10;
            pane_d.YAxis.MajorGrid.DashOff = 5;
            // Включаем отображение сетки напротив мелких рисок по оси X
            pane_d.YAxis.MinorGrid.IsVisible = true;
            // Задаем вид пунктирной линии для крупных рисок по оси Y: 
            // Длина штрихов равна одному пикселю, ... 
            pane_d.YAxis.MinorGrid.DashOn = 1;
            // затем 2 пикселя - пропуск
            pane_d.YAxis.MinorGrid.DashOff = 2;
            // Включаем отображение сетки напротив мелких рисок по оси Y
            pane_d.XAxis.MinorGrid.IsVisible = true;
            // Аналогично задаем вид пунктирной линии для крупных рисок по оси Y
            pane_d.XAxis.MinorGrid.DashOn = 1;
            pane_d.XAxis.MinorGrid.DashOff = 2;


            pane_d.AxisChange();
            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            //zedGraph.AxisChange();

            // Обновляем график
            //zedGraph.Invalidate();
        }

        public int SelectedFunction()
        {
            int count = 0;
            int selectedIndex = -1;
            if (radioButton6.Checked)
            {
                count++;
                selectedIndex = 0;
            }
            if (radioButton7.Checked)
            {
                count++;
                selectedIndex = 1;
            }
            if (radioButton8.Checked)
            {
                count++;
                selectedIndex = 2;
            }
            if (radioButton9.Checked)
            {
                count++;
                selectedIndex = 3;
            }
            if (radioButton10.Checked)
            {
                count++;
                selectedIndex = 4;
            }
            if (radioButton11.Checked)
            {
                count++;
                selectedIndex = 5;
            }
            return count == 1 ? selectedIndex : -1;
        }

        public void drawAdditionalGraphs()
        {
            pane = zedGraph.GraphPane;
            pane.GraphObjList.RemoveAll(text => text.GetType() == typeof(TextObj)
                    && ((string)text.Tag) == "1");
            // int selectedFunctionIndex = SelectedFunction();
            for (int i = 0; i < additionalFunctions.Count; i++)
            {
                Func<double, double> f = additionalFunctions[i].Function();
                PointPairList list1 = new PointPairList();
                for (double x = 3; x <= 15; x += 0.01)
                {
                    list1.Add(x, f.Invoke(x));
                    if (x == 3)
                    {
                        TextObj text = new TextObj((i + 1).ToString(), x, f.Invoke(x));
                        text.FontSpec.Border.IsVisible = false;
                        text.Tag = "1";
                        pane.GraphObjList.Add(text);
                    }
                    /*if (selectedFunctionIndex != -1)
                    {   
                        double[] y = new double[10];
                        switch (m_type_calc)
                        {
                            case "cpc":
                                for (int j = 0; j < y.Length; j++)
                                {
                                    y[j] = cpc_m[j, selectedFunctionIndex];
                                }
                                break;
                            case "cp":
                                for (int j = 0; j < y.Length; j++)
                                {
                                    y[j] = cm_m[j, selectedFunctionIndex];
                                }
                                break;
                            case "ke":
                                for (int j = 0; j < y.Length; j++)
                                {
                                    y[j] = ke_m[j, selectedFunctionIndex];
                                }
                                break;

                        }
                        int[] x1 = Enumerable.Range(3, 10).ToArray();
                        double RSquare = CalculateRSquare(x1, y, f);
                        int k = additionalFunctions[i].ParametersCount();
                        double Kds = CalculateKds(y.Length, k, RSquare);
                        double AIC = CalculateAIC(x1, y, f, k);
                        TextObj text = new TextObj(
                            String.Format("{0:f2} // {1:f2} // {2:f2}", RSquare, Kds, AIC),
                            x, f.Invoke(x));
                        text.FontSpec.Border.IsVisible = false;
                        text.Tag = "1";
                        pane.GraphObjList.Add(text);
                        
                    }*/
                }
                LineItem myline = pane.AddCurve("", list1, Color.Chocolate,
                                SymbolType.None);
                myline.Line.Width = 3.0f;

            }
            zedGraph.Invalidate();
        }

        public void RecalculateCoefficients()
        {
            int selectedFunctionIndex = SelectedFunction();
            if (enterNumberDialog != null)
            {
                if (selectedFunctionIndex != -1)
                {
                    double[] y = new double[10];
                    switch (m_type_calc)
                    {
                        case "cpc":
                            for (int j = 0; j < y.Length; j++)
                            {
                                y[j] = cpc_m[j, selectedFunctionIndex];
                            }
                            break;
                        case "cp":
                            for (int j = 0; j < y.Length; j++)
                            {
                                y[j] = cm_m[j, selectedFunctionIndex];
                            }
                            break;
                        case "ke":
                            for (int j = 0; j < y.Length; j++)
                            {
                                y[j] = ke_m[j, selectedFunctionIndex];
                            }
                            break;

                    }
                    enterNumberDialog.SetCoefficientStrings(additionalFunctions.Select((f) =>
                    {
                        int[] x = Enumerable.Range(3, 10).ToArray();
                        
                        double[] res = new double[10];

                        for (int i = 0; i < 10; i++)
                        {
                            //res[i] = func.Invoke((double)i + 3);
                            res[i] = f.Function().Invoke((double)i + 3);
                            //Console.WriteLine(res[i]);
                        }
                        double RSquare = CalculateRSquare(x, y, f.Function());
                        double RSquared = GoodnessOfFit.RSquared(res, y);
                        int k = f.ParametersCount();
                        double Kds = CalculateKds(y.Length, k, RSquare);
                        
                        return String.Format("{0:f4}     //     {1:f2}     //     {2:f2}", RSquared, RSquare, Kds);
                    }).ToArray());
                }
                else
                {
                    enterNumberDialog.SetCoefficientStrings(new string[0]);
                }
            }
        }


        public void SecondRecalculateCoefficients()
        {
            int selectedFunctionIndex = SelectedFunction();
            if (enterNumberDialog != null)
            {
                if (selectedFunctionIndex != -1)
                {
                    double[] y = new double[10];
                    switch (m_type_calc)
                    {
                        case "cpc":
                            for (int j = 0; j < y.Length; j++)
                            {
                                y[j] = cpc_m[j, selectedFunctionIndex];
                            }
                            break;
                        case "cp":
                            for (int j = 0; j < y.Length; j++)
                            {
                                y[j] = cm_m[j, selectedFunctionIndex];
                            }
                            break;
                        case "ke":
                            for (int j = 0; j < y.Length; j++)
                            {
                                y[j] = ke_m[j, selectedFunctionIndex];
                            }
                            break;

                    }
                    enterNumberDialog.SetCoefficientStrings2(additionalFunctions.Select((f) =>
                    {
                        int[] x = Enumerable.Range(3, 10).ToArray();

                        double[] res = new double[10];

                        for (int i = 0; i < 10; i++)
                        {
                            //res[i] = func.Invoke((double)i + 3);
                            res[i] = f.Function().Invoke((double)i + 3);
                            //Console.WriteLine(res[i]);
                        }
                        double StError = GoodnessOfFit.PopulationStandardError(res, y);
                        double R = GoodnessOfFit.R(res, y);
                        double RSquared = GoodnessOfFit.RSquared(res, y);
                        double RSquare = CalculateRSquare(x, y, f.Function());
                        int k = f.ParametersCount();
                        double Kds = CalculateKds(y.Length, k, RSquare);
                        double AIC = CalculateAIC(x, y, f.Function(), k);
                        return String.Format("{0:f2}     //     {1:f2}     //     {2:f4}", StError, R, AIC);
                    }).ToArray());
                }
                else
                {
                    enterNumberDialog.SetCoefficientStrings2(new string[0]);
                }
            }
        }

        public void drawAdditionalGraphs2()
        {
            foreach (RegressionFunction f in additionalFunctions)
            {
                PointPairList list1 = new PointPairList();
                for (double x = 3; x <= 15; x += 0.01)
                {
                    list1.Add(x, f.Function().Invoke(x));
                }
                LineItem myline = pane.AddCurve("", list1, Color.DarkSlateBlue,
                                SymbolType.None);
                myline.Line.Width = 3.0f;

            }
            zedGraph.Invalidate();
        }

        private void wordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Чтобы не было ошибок
            // для разработки
            //wordmake(System.Windows.Forms.Application.StartupPath + @"/Patterns/" + "\\Графики сходимости напряжений.docx");
            // для сборки проекта
            wordmake(System.Windows.Forms.Application.StartupPath + "\\Графики сходимости напряжений.docx");
        }


        private void excelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            app.SheetsInNewWorkbook = 1;
            Workbook wb = app.Workbooks.Add(Type.Missing);
            Worksheet sheet = (Worksheet)app.Worksheets.get_Item(1);
            //int offset = 0;


            int a = 0;
            sheet.Cells[1, 2] = "Напряжение по X";
            sheet.Cells[1, 3] = "Напряжение по Y";
            sheet.Cells[1, 4] = "Касательное";
            sheet.Cells[1, 5] = "1-е главное напряжение";
            sheet.Cells[1, 6] = "2-е главное напряжение";
            sheet.Cells[1, 7] = "Эквивалентное";
            for (int i = 1; i < 11; i++)
            {
                sheet.Cells[i + 1, 1] = i + 2;
                //tbl.Rows.Add(ref missing);//Добавляем в таблицу строку.
                //Обычно саздаю только строку с заголовками и одну пустую для данных.
                for (int j = 0; j < 6; ++j)
                {
                    switch (m_type_calc)
                    {
                        case "cpc":

                            sheet.Cells[i + 1, j + 2] = Math.Round(cpc_m[a, j], 2);


                            break;
                        case "cp":

                            sheet.Cells[i + 1, j + 2] = Math.Round(cm_m[a, j], 2);

                            break;
                        case "ke":

                            sheet.Cells[i + 1, j + 2] = Math.Round(ke_m[a, j], 2);

                            break;

                    }
                    //tbl.Rows[i + 1].Cells[j+2].Range.Text = "Запись №" + i.ToString();
                    //tbl.Rows[i + 1].Cells[3].Range.Text = "Запись №" + i.ToString();
                    //tbl.Rows[i + 1].Cells[4].Range.Text = "Запись №" + i.ToString();
                }
                if (a < 10)
                    ++a;

            }
            Range range = sheet.Range["B1:G11"];
            range.NumberFormat = "0.00";
            ChartObjects chartObjects = sheet.ChartObjects(Type.Missing);
            ChartObject chart = chartObjects.Add(400, 5, 500, 200);
            Microsoft.Office.Interop.Excel.Chart chartPage = chart.Chart;
            chartPage.SetSourceData(sheet.Range["A1:G11"], Type.Missing);
            chartPage.ChartType = XlChartType.xlLine;


            SaveFileDialog dlg = new SaveFileDialog();
            //doc.Close();
            //app.Quit();

            dlg.FileName = "Графики сходимости напряжений";
            dlg.Filter = "*.xls|*.xlsx|Все файлы|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string fileName = dlg.FileName;
                //doc.SaveAs(ref fileName, ref fileFormat, ref lockComments, ref password, ref addToRecentFile, ref writePassword, ref readOnlyRecomended, ref embedTrueTypeFonts, ref saveNativePictureFormat, ref saveFormsData, ref saveAsAOCELetter);
                //fileName = dlg.
                app.Application.ActiveWorkbook.SaveAs(fileName,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing);
            }
            wb.Close(0);
            app.Quit();

        }

        private void RedrawAllIfNeeded()
        {
            switch (CurrentGraphType)
            {
                case GraphType.Complex:
                    RedrawComplexGraph();
                    break;
                case GraphType.ForceECK:
                    drawForceECKGraph();
                    drawAdditionalGraphs();
                    break;
            }
        }

        private void RedrawComplexGraph()
        {
            CurrentGraphType = GraphType.Complex;
            zedGraph.GraphPane.CurveList.Clear();
            get_value();
            int graphscount = 0;
            if (radioButton6.Checked)
            {
                DrawGraph(0, "Сходимость напряжения по Х", "n");
                graphscount++;
                if (comboBox1.SelectedItem != comboBox1.Items[0])
                {
                    DrawApproximationGraph(0, "Сходимость напряжения по Х", "n", comboBox1.SelectedItem.ToString());
                }
            }
            if (radioButton7.Checked)
            {
                DrawGraph(1, "Сходимость напряжения по Y", "n");
                graphscount++;
                if (comboBox2.SelectedItem != comboBox2.Items[0])
                {
                    DrawApproximationGraph(1, "Сходимость напряжения по Y", "n", comboBox2.SelectedItem.ToString());
                }
            }
            if (radioButton8.Checked)
            {
                DrawGraph(2, "Касательное", "n");
                graphscount++;
                if (comboBox3.SelectedItem != comboBox3.Items[0])
                {
                    DrawApproximationGraph(2, "Касательное", "n", comboBox3.SelectedItem.ToString());
                }
            }
            if (radioButton9.Checked)
            {
                DrawGraph(3, "1-е главное напряжение", "n");
                graphscount++;
                if (comboBox4.SelectedItem != comboBox4.Items[0])
                {
                    DrawApproximationGraph(3, "1-е главное напряжение", "n", comboBox4.SelectedItem.ToString());
                }
            }
            if (radioButton10.Checked)
            {
                DrawGraph(4, "2-е главное напряжение", "n");
                graphscount++;
                if (comboBox5.SelectedItem != comboBox5.Items[0])
                {
                    DrawApproximationGraph(4, "2-е главное напряжение", "n", comboBox5.SelectedItem.ToString());
                }
            }
            if (radioButton11.Checked)
            {
                DrawGraph(5, "Эквивалентное", "n");
                graphscount++;
                if (comboBox6.SelectedItem != comboBox6.Items[0])
                {
                    DrawApproximationGraph(5, "Эквивалентное", "n", comboBox6.SelectedItem.ToString());
                }
            }
            if (graphscount > 1)
            {
                pane.Title.Text = "Напряжения";
            }
            drawAdditionalGraphs();
            if (graphscount == 0)
            {
                zedGraph.Invalidate();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RedrawComplexGraph();
        }

        private void затухающаяВолнаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFunctionPopUp popUp = new AddFunctionPopUp();
            popUp.ShowDialog();

            double y0 = popUp.Y0;
            double a = popUp.A;
            double b = popUp.B;
            double c = popUp.C;
            double d = popUp.D;

            additionalFunctions.Add(new Waveform(y0, a, b, c, d));

            popUp.Dispose();
            drawAdditionalGraphs();
        }

        private void удалитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            additionalFunctions.Clear();
            RedrawAllIfNeeded();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            RedrawAllIfNeeded();
        }

        public Data Data
        {
            get => default;
            set
            {
            }
        }

        private void открытьПоследнийToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            try
            {
                toolStripStatusLabel2.Text = MainForm.LastFilePath;
                DirectoryInfo maindirectiry = new DirectoryInfo(MainForm.LastFilePath);
                //MainForm.LastFilePath = dlg.SelectedPath;
                //toolStripStatusLabel2.Text = MainForm.LastFilePath;

                if (maindirectiry.Exists)
                {
                    directories = maindirectiry.GetDirectories();
                    flag = true;
                }
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("Проект ранее не открывался",
                                "Данные не выбраны", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Full_screen()
        {
            if (воВесьЭкранToolStripMenuItem.Checked)
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.BringToFront();
                TopMost = true;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.Sizable;
                TopMost = false;
            }
        }

        private void воВесьЭкранToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Full_screen();
        }

        private void боковаяПанельToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Left_Panel()
        {
            if (боковаяПанельToolStripMenuItem.Checked)
            {
                groupBox1.Show();
                groupBox2.Show();
                groupBox3.Show();
                groupBox4.Show();
                groupBox5.Show();
                groupBox6.Show();
                //groupBox7.Show();

                int x = groupBox1.Location.X + groupBox1.Width + 6;
                int y = zedGraph.Location.Y;
                zedGraph.Location = new Point(x, y);
                zedGraph.Width = this.Width - groupBox1.Location.X - x - 16;

            }
            else
            {
                int x = groupBox1.Location.X;
                int y = zedGraph.Location.Y;
                groupBox1.Hide();
                groupBox2.Hide();
                groupBox3.Hide();
                groupBox4.Hide();
                groupBox5.Hide();
                groupBox6.Hide();
                groupBox1.Hide();
                zedGraph.Location = new Point(x, y);
                zedGraph.Width = this.Width - 2 * x - 16;

            }
        }

        private void боковаяПанельToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Left_Panel();
        }

        private void Down_Panel()
        {
            if (строкаСостоянияToolStripMenuItem.Checked)
            {
                statusStrip1.Visible = true;
                int x = statusStrip1.Height;
                zedGraph.Height -= x;


            }
            else
            {
                statusStrip1.Visible = false;
                int x = statusStrip1.Height;
                zedGraph.Height += x;

            }
        }

        private void строкаСостоянияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Down_Panel();
        }

        private void печатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDialog1.ShowDialog();
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += PrintPage1;
            pd.Print();
        }


        private void PrintPage1(object o, PrintPageEventArgs e)
        {
            Bitmap bmp = pane.GetImage();
            Image img = Image.FromHbitmap(bmp.GetHbitmap());
            bmp = ResizeImage(img, 779, 474);
            img = Image.FromHbitmap(bmp.GetHbitmap());
            Point loc = new Point(7, 85);
            e.Graphics.DrawImage(img, loc);
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = System.Drawing.Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            //e.Graphics.DrawImage(Graphics.ActiveForm);
        }

        private void затухающаяВолнаToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }

        private void убывающаяФункцияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exp_func popUp_2 = new exp_func();
            popUp_2.label1.Text = "Убывающая экспонента";
            popUp_2.label4.Text = "f(x) = y0 + a1*e**(-b1*x)";
            popUp_2.ShowDialog();

            double y0 = popUp_2.Y0;
            double a1 = popUp_2.A1;
            double b1 = popUp_2.B1;

            additionalFunctions.Add(new ExponentialDecay(y0, a1, b1));

            popUp_2.Dispose();
            RedrawAllIfNeeded();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enterNumberDialog = new EnterNumberDialog(additionalFunctions, RedrawAllIfNeeded);

            enterNumberDialog.Show();
            RecalculateCoefficients();
            SecondRecalculateCoefficients();

        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            res1 = textBox3.Text;
            res2 = textBox4.Text;
            res3 = textBox5.Text;
            res4 = textBox6.Text;

            double res11, res22, res33, res44;

            try
            {
                res11 = Convert.ToInt32(res1);
                res22 = Convert.ToInt32(res2);
                res33 = Convert.ToInt32(res3);
                res44 = Convert.ToInt32(res4);

                if (res11 > res22)
                {
                    MessageBox.Show("Введены не правильные параметры. Х(мин) должен быть меньше Х(мах)",
                            "Данные не выбраны", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;

                }
                else
                {
                    if (res33 > res44)
                    {
                        MessageBox.Show("Введены не правильные параметры. Y(мин) должен быть меньше Y(мах)",
                                "Данные не выбраны", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        return;
                    }
                    else
                    {
                        pane.XAxis.Scale.Min = res11;
                        pane.XAxis.Scale.Max = res22;
                        pane.YAxis.Scale.Min = res33;
                        pane.YAxis.Scale.Max = res44;
                        // Обновляем график
                        zedGraph.Invalidate();
                    }
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Введены не правильные параметры. Должны быть введены числа",
                                "Данные не выбраны", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }



        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            pane.XAxis.Scale.MinAuto = true;
            pane.XAxis.Scale.MaxAuto = true;
            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MaxAuto = true;
            // Обновим данные об осях
            zedGraph.AxisChange();
            // Обновляем график
            zedGraph.Invalidate();
        }

        private void zedGraph_ContextMenuBuilder_1(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            menuStrip.Items[0].Text = "Копировать";
            menuStrip.Items[1].Text = "Сохранить как картинку…";
            menuStrip.Items[2].Text = "Параметры страницы…";
            menuStrip.Items[3].Text = "Печать…";
            menuStrip.Items[4].Text = "Показывать значения в точках…";
            menuStrip.Items[7].Text = "Установить масштаб по умолчанию…";

            // Некоторые пункты удалим
            menuStrip.Items.RemoveAt(5);
            menuStrip.Items.RemoveAt(5);

            // Добавим свой пункт меню
            //ToolStripItem newMenuItem = new ToolStripMenuItem("Этот пункт меню ничего не делает");
            //menuStrip.Items.Add(newMenuItem);
        }

        private void убывающаяФункцияToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            exp_func popUp_2 = new exp_func();
            popUp_2.label1.Text = "Возрастающая экспонента";
            popUp_2.label4.Text = "f(x) = y0 + a1*(1 - e**(-b1*x))";
            popUp_2.ShowDialog();


            double y0 = popUp_2.Y0;
            double a1 = popUp_2.A1;
            double b1 = popUp_2.B1;

            additionalFunctions.Add(new ExponentialRise(y0, a1, b1));

            popUp_2.Dispose();
            RedrawAllIfNeeded();

        }

        private void экспортФункцииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IFormatter formatter = new BinaryFormatter();


            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = "Функции сходимости напряжений";
            //saveFileDialog1.Filter = "binary files (*.bin)|*.bin|All files (*.*)|*.*";
            //saveFileDialog1.Filter = "All files (*.*)|*.*|(*.bin)|*.bin";
            saveFileDialog1.Filter = "All files (*.*)|*.*|(*.postpr)|*.postpr";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream stream = saveFileDialog1.OpenFile();
                formatter.Serialize(stream, additionalFunctions);
                stream.Close();
            }

        }

        private void импортФункцииToolStripMenuItem_Click(object sender, EventArgs e)
        {

            IFormatter formatter = new BinaryFormatter();


            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.Filter = "binary files (*.bin)|*.bin|All files (*.*)|*.*";
            openFileDialog1.Filter = "All files (*.*)|*.*|binary files (*.postpr)|*.postpr";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Stream stream = openFileDialog1.OpenFile();
                List<RegressionFunction> functions = new List<RegressionFunction>();
                functions = (List<RegressionFunction>)formatter.Deserialize(stream);
                stream.Close();
                additionalFunctions.AddRange(functions);
                RedrawAllIfNeeded();
            }
        }

        private void zedGraph_MouseMove(object sender, MouseEventArgs e)
        {
            double x, y;

            // Пересчитываем пиксели в координаты на графике
            // У ZedGraph есть несколько перегруженных методов ReverseTransform.
            pane.ReverseTransform(e.Location, out x, out y);

            // Выводим результат
            string text = string.Format("Координата точки X: {0:f2};    Y: {1:f2}", x, y);
            statusStrip1.Items[3].Text = text;


        }


        private void интерполяцияПрямойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RedrawAllIfNeeded();
        }

        private void удалитьВсеToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            enterNumberDialog = new EnterNumberDialog(additionalFunctions, RedrawAllIfNeeded);
            enterNumberDialog.Show();
            RecalculateCoefficients();
            SecondRecalculateCoefficients();

        }

        private void sigmaPlotToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Double[,] Array = new double[10, 6];
            int a = 0;
            for (int i = 0; i < 10; i++)
            {
                //Array[i, 3] = i + 3;
                for (int j = 0; j < 6; ++j)
                {

                    switch (m_type_calc)
                    {
                        case "cpc":
                            Array[i, j] = Math.Round(cpc_m[a, j], 2);
                            break;
                        case "cp":
                            Array[i, j] = Math.Round(cm_m[a, j], 2);
                            break;
                        case "ke":
                            Array[i, j] = Math.Round(ke_m[a, j], 2);
                            break;
                    }
                }
                if (a < 10)
                    ++a;
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Графики сходимости напряжений";
            dlg.Filter = "*.csv|*.csv|Все файлы|*.*";
            string filePath;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                filePath = Path.GetFullPath(dlg.FileName);
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    for (int i = 0; i < Array.GetLength(0); i++)
                    {
                        sw.Write("{0:d}" + ";", i + 3);
                        for (int j = 0; j < Array.GetLength(1); j++)
                        {
                            sw.Write("{0,3}" + ";", Array[i, j]);
                        }
                        sw.WriteLine();
                    }
                }
            }
        }

        private void ограниченияГрафикаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ограниченияГрафикаToolStripMenuItem.Checked)
            {
                if (боковаяПанельToolStripMenuItem.Checked)
                {
                    groupBox6.Visible = true;
                }
                else
                {
                    groupBox6.Visible = false;
                }
            }
            else
            {
                groupBox6.Visible = false;
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            RedrawComplexGraph();
            RecalculateCoefficients();
            SecondRecalculateCoefficients();
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            RedrawComplexGraph();
            RecalculateCoefficients();
            SecondRecalculateCoefficients();
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            RedrawComplexGraph();
            RecalculateCoefficients();
            SecondRecalculateCoefficients();
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            RedrawComplexGraph();
            RecalculateCoefficients();
            SecondRecalculateCoefficients();
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            RedrawComplexGraph();
            RecalculateCoefficients();
            SecondRecalculateCoefficients();
        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            RedrawComplexGraph();
            RecalculateCoefficients();
            SecondRecalculateCoefficients();
        }
    }
}

