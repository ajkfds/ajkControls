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
            this.doubleBufferedDrawBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.doubleBufferedDrawBox_MouseWheel);

            vScrollBar.Width = Global.ScrollBarWidth;
        }

        public int HeaderHeight
        {
            get; set;
        }

        public Color selectedColor = Color.FromArgb(60, Color.FromArgb(0x5b, 0x7d, 0x9f));
        public Color SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                selectedColor = value;
            }
        }

        private SolidBrush selectionBrush
        {
            get
            {
                return new SolidBrush(selectedColor);
            }
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
                for (int i = 0; i < columns; i++)
                {
                    Widths.Add(doubleBufferedDrawBox.Width / columns);
                }
                recalcWidth();
            }
        }

        public int LineHeight
        {
            get
            {
                return lineHeight;
            }
        }

        private int stretchableCoulmn = 0;
        public int StretchableCoulmn
        {
            get
            {
                return stretchableCoulmn;
            }
            set
            {
                stretchableCoulmn = value;
                recalcWidth();
            }
        }

        private void recalcWidth()
        {
            int width = 0;
            for (int i = 0; i < columns; i++)
            {
                if (i == stretchableCoulmn) continue;
                width += Widths[i];
            }
            if( 0 <= stretchableCoulmn  && stretchableCoulmn < Widths.Count && (doubleBufferedDrawBox.Width - width) > 0)
            {
                Widths[stretchableCoulmn] = doubleBufferedDrawBox.Width - width;
            }
        }

        private void DoubleBufferedDrawBox_Resize(object sender, EventArgs e)
        {
            reCalcParameters();
            recalcWidth();
        }

        private void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            reCalcParameters();
            doubleBufferedDrawBox.Invalidate();
        }
        private void doubleBufferedDrawBox_MouseWheel(object sender, MouseEventArgs e)
        {
            int delta = (int)(Global.WheelSensitivity * e.Delta) * SystemInformation.MouseWheelScrollLines;
            if (delta == 0)
            {
                if (e.Delta > 0) delta = 1;
                if (e.Delta < 0) delta = -1;
            }
            int value = vScrollBar.Value - delta;
            if (value < vScrollBar.Minimum) value = vScrollBar.Minimum;
            if (value > vScrollBar.Maximum - vScrollBar.LargeChange) value = vScrollBar.Maximum - vScrollBar.LargeChange;
            if (value < 0) value = 0;
            vScrollBar.Value = value;
            reCalcParameters();
            doubleBufferedDrawBox.Invalidate();
        }

        public List<TableItem> TableItems = new List<TableItem>();
        public TableItem SelectedItem = null;
        public List<int> Widths = new List<int>();

        private int lineHeight = 10;

        private int startLine = 0;
        private int lines = 1;
        private void reCalcParameters()
        {
            lines = (doubleBufferedDrawBox.Height - HeaderHeight) / lineHeight;
            if (lines < 0) return;
            vScrollBar.LargeChange = lines;
            startLine = vScrollBar.Value;
        }


        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        private void DoubleBufferedDrawBox_DoubleBufferedPaint(PaintEventArgs e)
        {
            sw.Reset();
            sw.Start();
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Size fontSize = System.Windows.Forms.TextRenderer.MeasureText(e.Graphics, "A", Font, new Size(100, 100), TextFormatFlags.NoPadding);
            lineHeight = fontSize.Height;

            reCalcParameters();

            int y = lineHeight;
            int lineCount = 0;

            List<Rectangle> rectangles = new List<Rectangle>();
            for (int index = startLine; index < TableItems.Count; index++)
            {
                int x = 0;
                rectangles.Clear();
                foreach (int width in Widths)
                {
                    rectangles.Add(new Rectangle(x, y, width, lineHeight));
                    x += width;
                }

                TableItems[index].Draw(e.Graphics, Font, rectangles);

                y = y + lineHeight + 2;
                lineCount++;
                if (lineCount > lines) break;
            }

            y = lineHeight;
            lineCount = 0;
            for (int index = startLine; index < TableItems.Count; index++)
            {
                int x = 0;
                rectangles.Clear();
                foreach (int width in Widths)
                {
                    rectangles.Add(new Rectangle(x, y, width, lineHeight));
                    x += width;
                }

                TableItems[index].PostDraw(e.Graphics, Font, rectangles);

                if (TableItems[index] == SelectedItem)
                {
                    e.Graphics.FillRectangle(selectionBrush, new Rectangle(0, y, doubleBufferedDrawBox.Width, lineHeight));
                }

                y = y + lineHeight + 2;
                lineCount++;
                if (lineCount > lines) break;
            }
            sw.Stop();
            System.Diagnostics.Debug.Print("table " + sw.ElapsedMilliseconds+"ms");
        }

        public TableItem HitTest(int x,int y)
        {
            int line = (y - HeaderHeight) / (lineHeight+2) + startLine;
            if (line < 1) return null;
            line--;
            if (TableItems.Count <= line) return null;
            return TableItems[line];
        }

        private void DoubleBufferedDrawBox_MouseMove(object sender, MouseEventArgs e)
        {
            TableItem item = HitTest(e.X, e.Y);
            if (SelectedItem != item)
            {
                SelectedItem = item;
                doubleBufferedDrawBox.Invalidate();
            }
        }
    }
}
