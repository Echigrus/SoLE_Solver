using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Matrices
{
    partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();
            this.Text = "О программе";
            this.labelProductName.Text = "Программа для анализа и поиска решения СЛАУ";
            this.labelVersion.Text = "Версия 1.1";
            this.labelCopyright.Text = "Автор: Дмитрий Андреев";
            this.labelCompanyName.Text = "Echigrus | HansaDev Team";
            this.textBoxDescription.Text = "Данная программа написана в рамках проектной работы для Конкурса им. В.И.Вернадского 2018-2019 года. Назначение: " +
                "анализ матричного представления СЛАУ для определения свойств системы (ранг, определитель, совместность, однородность, наличие решений, их количество) " +
                "и её решения, если таковое существует. Программа работает с полными квадратными матрицами и предоставляет возможность использования следующих способов решения СЛАУ:"+ Environment.NewLine+
                "- метод Гаусса" + Environment.NewLine+
                "- метод Крамера" + Environment.NewLine +
                "- матричный метод";
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://github.com/Echigrus/SoLE_Solver");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:echigrus@gmail.com");
        }
    }
}
