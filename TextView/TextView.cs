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
    public partial class TextView : UserControl
    {
        public TextView()
        {
            InitializeComponent();
        }

        private StringBuilder sBuilder = new StringBuilder();

        public override string Text
        {
            set
            {
                sBuilder.Clear();
                AppendText(value);
            }
            get
            {
                return sBuilder.ToString();
            }
        }

        public void AppendText(string text)
        {
            sBuilder.Append(text);
            dBDrawBox.Invalidate();
        }

        private void dBDrawBox_DoubleBufferedPaint(PaintEventArgs e)
        {
            System.Windows.Forms.TextRenderer.DrawText(e.Graphics, Text, Font, new Point(0, 0), Color.Gray);
        }
    }
}
