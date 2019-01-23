using System;
using System.Windows.Forms;

namespace Matrices
{
    public partial class Form1 : Form
    {
        static int n, i, j, r1, r2;
        static double[,] m1, m2, ta;
        static double[] x, b, det, xc;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void setBtn_Click(object sender, System.EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out n))
            {
                n = Convert.ToInt32(textBox1.Text);
                m1 = new double[n, n];
                b = new double[n];
                x = new double[n];
                m2 = new double[n, n+1];
                dataGridView1.RowCount = n;
                dataGridView1.ColumnCount = n;
                dataGridView2.RowCount = n;
                dataGridView2.ColumnCount = 1;
                dataGridView3.RowCount = n;
                dataGridView3.ColumnCount = 1;
            }
        }

        private void solveBtn_Click(object sender, System.EventArgs e)
        {
            for (i = 0; i < n; i++)
            {
                b[i]= Convert.ToDouble(dataGridView3.Rows[i].Cells[0].Value);
                m2[i, n] = b[i];
                for (j = 0; j < n; j++)
                {
                    m1[i, j] = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value);
                    m2[i, j] = m1[i, j];
                }
            }
            label2.Text= "Определитель: " + Det(m1);
            r1 = Rank(n, n, m1); r2 = Rank(n, n+1, m2);
            label5.Text = "Ранг матрицы: " + r1 + "(" + r2 + ")";
            if (r1 == r2)
            {
                if (r1 == n)
                {
                    label6.Text = "Решений: одно";
                    if (checkBox1.Checked)
                    {
                        FormatSystem(n, n+1, m2);
                        x= GaussMethod(m2);
                        for (i = 0; i < n; ++i) dataGridView2.Rows[i].Cells[0].Value = x[i];
                    }
                    else
                    {
                        dataGridView4.RowCount = n;
                        dataGridView4.ColumnCount = 2;
                        x = Cramer(m1, n, b);
                        for (i = 0; i < n; ++i) dataGridView2.Rows[i].Cells[0].Value = x[i];
                    }
                }
                if(r1<n) 
                {
                    label6.Text = "Решений: ∞";
                    FormatSystem(n, n+1, m2);
                }
            }
            else label6.Text = "Решений: нет";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Click += new EventHandler(this.setBtn_Click);
            button1.Click += new EventHandler(this.solveBtn_Click);
            checkBox1.Click += new EventHandler(this.ch1_Click);
            checkBox2.Click += new EventHandler(this.ch2_Click);
        }

        public void ch1_Click(object sender, System.EventArgs e)
        {
            checkBox2.Checked = false;
        }
        public void ch2_Click(object sender, System.EventArgs e)
        {
            checkBox1.Checked = false;
        }

        public double[] Cramer(double[,] a, int size, double[] b) {
            int i, k, j;
            det = new double[size + 1]; xc = new double[size];
            det[0] = Det(a);
            ta = new double[size, size];
            for (i = 0; i < size; i++)
            {
                for (j = 0; j < size; j++)
                {
                    for (k = 0; k < size; k++) {
                        ta[j, k] = a[j, k];
                    }
                }
                for (k = 0; k < size; k++)
                {
                    ta[k, i] = b[k];
                }
                det[i + 1] = Det(ta);
                dataGridView4.Rows[i].Cells[0].Value = "det"+(i+1);
                dataGridView4.Rows[i].Cells[1].Value = det[i + 1];
                xc[i] = det[i + 1] / det[0];
                
            }
            return xc;
        }

        public static double Det(double[,] a)
        {
            int m = a.GetLength(0), p = a.GetLength(1), idet;
            double d = 0;
            if (m == p)
            {
                if (m > 2)
                {
                    for (idet = 0; idet < m; idet++) {
                        if(idet%2==1) d = d - a[0, idet] * Pivot(a, 0, idet);
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

        public static double Pivot(double[,] a, int row, int col) {
            int m = a.GetLength(0), ip, jp;
            double[,] tp = new double[m-1, m-1];            
            for (ip = 0; ip < m; ip++) {
                for (jp = 0; jp < m; jp++) {
                    if (ip < row && jp < col) tp[ip, jp] = a[ip, jp];
                    else if (ip > row && jp < col) tp[ip - 1, jp] = a[ip, jp];
                    else if (ip < row && jp > col) tp[ip, jp - 1] = a[ip, jp];
                    else if (ip > row && jp > col) tp[ip - 1, jp - 1] = a[ip, jp];
                }
            }

            return Det(tp);
        }

        void FormatSystem(int nf, int mf, double[,] a)
        {
            int i, j, k;
            double E, temp;
            double[] tempm = new double[mf];
            for (k = 0; k < nf; k++) //перебор строк
            {
                if (a[k, k] != 0) //главная диагональ
                {
                    for (i = k + 1; i < nf; i++) //от k+1 до конца
                    {
                        temp = a[k, k];
                        for (j = 0; j < mf; j++) { a[k, j] /= temp; } //делит строку k на a[k][k]
                        E = -a[i, k] / a[k, k]; //делитель
                        for (j = k; j < mf; j++)
                        {
                            a[i, j] += E * a[k, j]; //все строки вниз умножаются на делитель
                        }
                    }
                }
                else
                {
                    for (i = 0; i < k; i++)
                    {
                        if (i > k && a[i, k] != 0)
                        {
                            for (j = 0; j < mf; j++) tempm[j] = a[k, j];
                            for (j = 0; j < mf; j++) a[k, j] = a[i, j];
                            for (j = 0; j < mf; j++) a[i, j] = tempm[j];
                        }
                    }
                }
            }

            dataGridView4.RowCount = nf;
            dataGridView4.ColumnCount = mf;
            for (k = 0; k < nf; ++k)
                for (i = 0; i < mf; ++i) dataGridView4.Rows[k].Cells[i].Value = a[k,i];
               

        }
        public static double[] GaussMethod(double[,] a)
        {
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

        public int Rank(int nr, int mr, double[,] a)
        {
            int iz, jz, kz, rank=0;
            double E, tr;
            double[] tempm = new double[mr];
            Boolean cleanLine = false;
            double[,] ra = new double[nr, mr];
            for (kz = 0; kz < nr; kz++)
                for (iz = 0; iz < mr; iz++) ra[kz, iz] = a[kz, iz];
            for (kz = 0; kz < nr; kz++) //вертикаль
            {
                if (ra[kz, kz] != 0) //главная диагональ
                {
                    for (iz = kz + 1; iz < nr; iz++) //от k+1 до конца
                    {
                        tr = ra[kz, kz];
                        for (jz = 0; jz < mr; jz++) ra[kz, jz] /= tr; //делит строку k на a[k][k]
                        E = -ra[iz, kz] / ra[kz, kz]; //делитель
                        for (jz = kz; jz < mr; jz++)
                        {
                            ra[iz, jz] += E * ra[kz, jz]; //все строки вниз умножаются на делитель
                        }
                    }
                }
                else
                {
                    for (iz = 0; iz < kz; iz++)
                    {
                        if (iz > kz && ra[iz, kz] != 0)
                        {
                            for (jz = 0; jz < mr; jz++) tempm[jz] = ra[kz, jz];
                            for (jz = 0; jz < mr; jz++) ra[kz, jz] = ra[iz, jz];
                            for (jz = 0; jz < mr; jz++) ra[iz, jz] = tempm[jz];
                        }
                    }
                }
            }
            for (iz = 0; iz < nr; iz++)
            {
                cleanLine = false;
                for (jz = 0; jz < mr; jz++)
                {
                    if (ra[iz, jz] != 0) cleanLine = true;
                }
                if (cleanLine) rank++;
            }
            return rank;

        }
    }
}
