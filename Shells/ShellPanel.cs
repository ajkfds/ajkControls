using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ajkControls
{
    public partial class ShellPanel : UserControl
    {
        public ShellPanel(Shell shell)
        {
            InitializeComponent();
            this.shell = shell;
            this.shell.LineReceived += shell_LineReceived;

            this.shell.Start();
        }

        public new void Dispose()
        {
            shell.Dispose();
            base.Dispose();
        }

        Shell shell = null;

        private void shell_LineReceived(string lineString)
        {
            logView.AppendLogLine(lineString);
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Return) return;
            string command = textBox.Text;
            textBox.Text = "";
            textBox.Refresh();

            shell.Execute(command);
            e.Handled = true;
        }
    }
}
