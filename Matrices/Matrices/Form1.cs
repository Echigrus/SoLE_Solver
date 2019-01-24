using System; //объявление используемых библиотек
using System.Collections.Generic;
using System.Windows.Forms;
namespace Matrices
{
    public partial class Form1 : Form
    { //глобальные статические переменные класса
        static int n, i, j, r1, r2;
        static double[,] m1, m2, ta;
        static double[] x, b, det, xc;
        static Boolean ifSLAEhomogenous, mComplete = false, mFormed = false;
        static double mainDet;
        AboutBox1 a; //окно «О программе»

        public Form1()
        {
            InitializeComponent();
            List<string> methods = new List<string> {
                "Метод Крамера", "Метод Гаусса", "Матричный метод"
            };
            domainUpDown1.Items.AddRange(methods);
            domainUpDown1.SelectedIndex = 0; //окошко выбора метода решения
            label11.Text = "Статус: введите размер матрицы";
        }

        private void Form1_Load(object sender, EventArgs e)
        {//обработка нажатий
            a = new AboutBox1();
            button2.Click += new EventHandler(this.setBtn_Click);
            button1.Click += new EventHandler(this.solveBtn_Click);
            button3.Click += new EventHandler(this.cleanBtn_Click);
            linkLabel1.Click += new EventHandler(this.linkLabel1_Clicked);
        }

        private void setBtn_Click(object sender, System.EventArgs e)
        { //кнопка, создающая таблицы для заполнения
            if (int.TryParse(textBox1.Text, out n) && n > 0)
            { //проверка n на целочисленное положительное
                n = Convert.ToInt32(textBox1.Text);
                m1 = new double[n, n];
                b = new double[n];
                x = new double[n];
                m2 = new double[n, n + 1];

                dataGridView1.RowCount = n;
                dataGridView1.ColumnCount = n;
                for (i = 0; i < n; i++)
                { //размер столбца адаптируется под n элементов
                    if (n <= 6) dataGridView1.Columns[i].Width = 360 / n;
                    else dataGridView1.Columns[i].Width = 60;
                    dataGridView1.Columns[i].HeaderText = "a" + (i + 1);
                }

                dataGridView2.RowCount = n;
                dataGridView2.ColumnCount = 1;
                dataGridView2.Columns[0].Width = 90;

                dataGridView3.RowCount = n;
                dataGridView3.ColumnCount = 1;
                dataGridView3.Columns[0].Width = 90;
                label11.Text = "Статус: заполните A и B";
                mFormed = true; //означает, что таблицы построены
            }
            else
            {
                mFormed = false; mComplete = false;
                label11.Text = "Статус: размер матрицы - целое положительное число";
            }
        }

        private void solveBtn_Click(object sender, System.EventArgs e)
        {
            if (mFormed)
            {
                mComplete = true;
                for (i = 0; i < n; i++)
                {
                    for (j = 0; j < n; j++) if (dataGridView1.Rows[i].Cells[j].Value == null) mComplete = false; //проверка, нет ли пустых полей
                    if (dataGridView3.Rows[i].Cells[0].Value == null) mComplete = false;
                }
                if (mComplete)
                {
                    ifSLAEhomogenous = true; //определение однородности матрицы
                    for (i = 0; i < n; i++)
                    {
                        b[i] = Convert.ToDouble(dataGridView3.Rows[i].Cells[0].Value);
                        if (b[i] != 0) ifSLAEhomogenous = false;
                        m2[i, n] = b[i]; //заполнение крайнего правого столбца дополненной матрицы
                        for (j = 0; j < n; j++)
                        {
                            m1[i, j] = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value);
                            m2[i, j] = m1[i, j];
                        }
                    }
                    mainDet = Det(m1);
                    label2.Text = "Определитель: " + mainDet;
                    r1 = Rank(n, n, m1); r2 = Rank(n, n + 1, m2);
                    label5.Text = "Ранг матрицы: " + r1 + "(" + r2 + ")";

                    if (ifSLAEhomogenous) label10.Text = "Матрица однородна: да";
                    else label10.Text = "Матрица однородна: нет";

                    if (r1 == r2) //определение кол-ва решений по Кронекеру-Капелли
                    {
                        label9.Text = "Матрица совместна: да";
                        if (r1 == n && mainDet != 0)
                        {
                            label6.Text = "Решений: одно";
                            if (domainUpDown1.SelectedIndex == 1)
                            { //метод Гаусса
                                label4.Text = "Матрица, отформатированная методом Гаусса";
                                FormatSystem(n, n + 1, m2);
                                x = GaussMethod(m2);
                                for (i = 0; i < n; ++i) dataGridView2.Rows[i].Cells[0].Value = x[i];
                            }
                            else if (domainUpDown1.SelectedIndex == 0)
                            { //метод Крамера
                                label4.Text = "Опр-ли для мод. A (номер опр-ля соотв. столбцу, заменённому на B)";
                                dataGridView4.RowCount = n;
                                dataGridView4.ColumnCount = 2;
                                x = Cramer(m1, n, b);
                                for (i = 0; i < n; ++i) dataGridView2.Rows[i].Cells[0].Value = x[i];
                            }
                            else if (domainUpDown1.SelectedIndex == 2)
                            { //матричный метод
                                label4.Text = "Транспонированная союзная матрица";
                                MatMet(m1, b);
                            }
                            label11.Text = "Статус: найдено частное решение";
                        }
                        if (r1 < n || mainDet == 0)
                        { //если решений бесконечно много, будет выведена таблица треугольного вида, форматированная прямым ходом метода Гаусса
                            label6.Text = "Решений: ∞";
                            FormatSystem(n, n + 1, m2);
                            label11.Text = "Статус: найдено общее решение";
                        }
                    }
                    else
                    {
                        label4.Text = "Матрица, отформатированная методом Гаусса";
                        label6.Text = "Решений: нет";
                        label11.Text = "Статус: система не имеет решений";
                        label9.Text = "Матрица совместна: нет";
                    }
                }
                else label11.Text = "Статус: обнаружены пустые поля, заполните их";
            }
            else label11.Text = "Статус: сначала создайте матрицу";
        }

        private void cleanBtn_Click(object sender, System.EventArgs e)
        { //кнопка очистки полей
            if (mFormed)
            {
                for (i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView3.Rows[i].Cells[0].Value = null;
                    dataGridView2.Rows[i].Cells[0].Value = null;
                    for (j = 0; j < dataGridView1.ColumnCount; j++)
                    {
                        dataGridView1.Rows[i].Cells[j].Value = null;
                    }
                }
                for (i = 0; i < dataGridView4.RowCount; i++)
                    for (j = 0; j < dataGridView4.ColumnCount; j++) dataGridView4.Rows[i].Cells[j].Value = null;
                label11.Text = "Статус: таблицы очищены";
            }
            else label11.Text = "Статус: сначала постройте таблицу";
        }

        private void linkLabel1_Clicked(object sender, System.EventArgs e)
        { // вызов окна «О программе»
            a.ShowDialog();
        }

        public double[] Cramer(double[,] a, int size, double[] b)
        {
            int i, k, j;
            det = new double[size + 1]; xc = new double[size];
            det[0] = Det(a);
            ta = new double[size, size];
            for (i = 0; i < size; i++)
            {
                for (j = 0; j < size; j++)
                {
                    for (k = 0; k < size; k++)
                    { //заполнение временного A элементами оригинального А
                        ta[j, k] = a[j, k];
                    }
                }
                for (k = 0; k < size; k++)
                { //заполнение столбца k элементами b
                    ta[k, i] = b[k];
                }
                det[i + 1] = Det(ta); //поиск определителя для каждого из случаев
                dataGridView4.Rows[i].Cells[0].Value = "det" + (i + 1);
                dataGridView4.Rows[i].Cells[1].Value = det[i + 1];
                xc[i] = det[i + 1] / det[0];

            }
            return xc; //метод возвращает массив неизвестных
        }

        public static double Det(double[,] a)
        {
            int m = a.GetLength(0), p = a.GetLength(1), idet;
            double d = 0;
            if (m == p) //проверка на то, квадратная ли матрица
            {
                if (m > 2)
                {//для матриц выше второго порядка применяется рекурсивный метод вычисления определителя через понижение порядка матрицы
                    for (idet = 0; idet < m; idet++)
                    {
                        if (idet % 2 == 1) d = d - a[0, idet] * Pivot(a, 0, idet);
                        else d = d + a[0, idet] * Pivot(a, 0, idet);
                    }
                    return d;
                }
                else if (m == 2) return a[0, 0] * a[1, 1] - a[0, 1] * a[1, 0];
                else if (m == 1) return a[0, 0];
                else return 0;
            }
            else return 0;
        }

        public static double Pivot(double[,] a, int row, int col)
        {
            int m = a.GetLength(0), ip, jp;
            double[,] tp = new double[m - 1, m - 1];
            for (ip = 0; ip < m; ip++)
            {//временная матрица порядка m-1 заполняется из A
                for (jp = 0; jp < m; jp++)
                { //столбец и строка, указанные в аргументах, игнорируются
                    if (ip < row && jp < col) tp[ip, jp] = a[ip, jp];
                    else if (ip > row && jp < col) tp[ip - 1, jp] = a[ip, jp];
                    else if (ip < row && jp > col) tp[ip, jp - 1] = a[ip, jp];
                    else if (ip > row && jp > col) tp[ip - 1, jp - 1] = a[ip, jp];
                }
            }

            return Det(tp);
        }

        void FormatSystem(int row, int col, double[,] a)
        {//форматирование системы методом Гаусса
            int i, j, k;
            double E, temp;
            double[] tempm = new double[col];
            for (k = 0; k < row; k++) 
            {
                if (a[k, k] != 0) 
                {
                    for (i = k + 1; i < row; i++) 
                    {
                        temp = a[k, k];
                        for (j = 0; j < col; j++) { a[k, j] /= temp; } 
                        E = -a[i, k] / a[k, k]; 
                        for (j = k; j < col; j++)
                        {
                            a[i, j] += E * a[k, j]; 
                        }
                    }
                }
                else
                {
                    for (i = 0; i < k; i++)
                    {
                        if (i > k && a[i, k] != 0)
                        {
                            for (j = 0; j < col; j++) tempm[j] = a[k, j];
                            for (j = 0; j < col; j++) a[k, j] = a[i, j];
                            for (j = 0; j < col; j++) a[i, j] = tempm[j];
                        }
                    }
                }
            }

            dataGridView4.RowCount = row;
            dataGridView4.ColumnCount = col;
            for (k = 0; k < col; ++k)
            {
                if (col <= 6) dataGridView4.Columns[k].Width = 360 / col;
                else dataGridView4.Columns[k].Width = 60;
                for (i = 0; i < row; ++i) dataGridView4.Rows[i].Cells[k].Value = a[i, k];
            }   
        }
        public static double[] GaussMethod(double[,] a)
        {//обратный ход метода Гаусса
            int m = a.GetUpperBound(0);//ширина
            int n = a.GetUpperBound(1);//высота
            int i = 0, j;
            double[] x;
            x = new double[n];

            for (i = n - 1; i >= 0; --i)
            {
                x[i] = a[i, n] / a[i, i];
                for (j = i + 1; j < n; ++j)
                {
                    x[i] -= x[j] * (a[i, j] / a[i, i]);
                }
            }

            return x;
        }

        public void MatMet(double[,] a, double[] b)
        {//матричный метод
            int size = b.GetLength(0), mi, mj;
            double tempadj, detA = Det(a);
            double[] xa = new double[size];
            double[,] backA = new double[size, size];

            dataGridView4.RowCount = size;
            dataGridView4.ColumnCount = size;

            for (mi = 0; mi < size; mi++)
            {
                if (size <= 6) dataGridView4.Columns[mi].Width = 360 / size;
                else dataGridView4.Columns[mi].Width = 60;
                tempadj = 0;
                for (mj = 0; mj < size; mj++)
                {
                    if ((mi + mj + 2) % 2 == 0) backA[mi, mj] = Pivot(a, mj, mi);
                    else backA[mi, mj] = -Pivot(a, mj, mi);
                    dataGridView4.Rows[mi].Cells[mj].Value = backA[mi, mj];
                    backA[mi, mj] = backA[mi, mj] / detA;
                    tempadj = tempadj + backA[mi, mj] * b[mj];
                }
                xa[mi] = tempadj;
                dataGridView2.Rows[mi].Cells[0].Value = xa[mi];
            }
        }

        public int Rank(int row, int col, double[,] a)
        {//вычисление ранга по количеству ненулевых строк после приведения к треугольному виду
            int iz, jz, kz, rank=0;
            double E, tr;
            double[] tempm = new double[col];
            Boolean cleanLine = false;
            double[,] ra = new double[row, col];
            for (kz = 0; kz < row; kz++)
                for (iz = 0; iz < col; iz++) ra[kz, iz] = a[kz, iz];
            for (kz = 0; kz < row; kz++){
                if (ra[kz, kz] != 0){
                    for (iz = kz + 1; iz < row; iz++)
                    {
                        tr = ra[kz, kz];
                        for (jz = 0; jz < col; jz++) ra[kz, jz] /= tr;
                        E = -ra[iz, kz] / ra[kz, kz];
                        for (jz = kz; jz < col; jz++)
                        {
                            ra[iz, jz] += E * ra[kz, jz];
                        }
                    }
                }
                else
                {
                    for (iz = 0; iz < kz; iz++)
                    {
                        if (iz > kz && ra[iz, kz] != 0)
                        {
                            for (jz = 0; jz < col; jz++) tempm[jz] = ra[kz, jz];
                            for (jz = 0; jz < col; jz++) ra[kz, jz] = ra[iz, jz];
                            for (jz = 0; jz < col; jz++) ra[iz, jz] = tempm[jz];
                        }
                    }
                }
            }
            for (iz = 0; iz < row; iz++){
                cleanLine = false;
                for (jz = 0; jz < col; jz++) if (ra[iz, jz] != 0) cleanLine = true;
                if (cleanLine) rank++;
            }
            return rank;
        }
    }
}
