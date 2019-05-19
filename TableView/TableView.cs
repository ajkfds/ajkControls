using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ajkControls.TableView
{
    public partial class TableView : UserControl
    {
        public TableView()
        {
            InitializeComponent();
        }

        public int HeaderHeight
        {
            get; set;
        }

        private int columns = 1;
        public int Columns
        {
            get
            {
                return columns;
            }
            set
            {
                columns = value;
                Widths.Clear();
                for(int i = 0; i < columns; i++)
                {
                    Widths.Add(doubleBufferedDrawBox.Width / columns);
                }
            }
        }

        public List<TableItem> TableItems = new List<TableItem>();
        public List<int> Widths = new List<int>();

        private int lineHeight = 10;
        private int lines = 1;

        private void DoubleBufferedDrawBox_DoubleBufferedPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            Size fontSize = System.Windows.Forms.TextRenderer.MeasureText(e.Graphics, "A", Font, new Size(100, 100), TextFormatFlags.NoPadding);
            lineHeight = fontSize.Height;
            lines = (doubleBufferedDrawBox.Height - HeaderHeight) / lineHeight;
            List<int> x = new List<int>();
            int y = 0;
            x.Add(0);
            foreach(int width in Widths)
            {
                x.Add(x.Last() + width);
            }

            for(int index = 0; index < TableItems.Count;index++)
            {
                TableItems[index].Draw(e.Graphics, Font,x,y,lineHeight);
                y = y + lineHeight + 2;
            }
        }

        private void DoubleBufferedDrawBox_Resize(object sender, EventArgs e)
        {

        }
    }
}
