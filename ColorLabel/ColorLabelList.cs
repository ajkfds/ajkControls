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
    public partial class ColorLabelList : UserControl
    {
        public ColorLabelList()
        {
            InitializeComponent();
//            this.vScrollBar.Width = Global.ScrollBarWidth;
        }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }

            set
            {
                base.BackColor = value;
                doubleBufferedDrawBox.BackColor = value;
            }
        }

        private List<ColorLabel> labels = new List<ajkControls.ColorLabel>();

        public void Clear()
        {
            labels.Clear();
            updateLabels();
            Invalidate();
        }

        public void AppendColorLabel(ColorLabel colorLabel)
        {
            labels.Add(colorLabel);
            updateLabels();
            Invalidate();
        }

        private void updateLabels()
        {
            vScrollBar.Maximum = labels.Count;
            if (vScrollBar.Value > vScrollBar.Maximum) vScrollBar.Value = vScrollBar.Maximum;
        }

        public IReadOnlyList<ColorLabel> ColorLabels
        {
            get
            {
                return labels;
            }
        }

        private int scrollPosition = 0;

        private int topMargin = 4;
        private int bottomMargin = 4;
        private int leftMargin = 4;
        private int rightMargin = 4;
        private int itemGap = 2;

        private void doubleBufferedDrawBox_DoubleBufferedPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            int width = 0;
            int height = 0;
            List<int> y = new List<int>();
            foreach (ColorLabel item in labels)
            {
                y.Add(height);
                Size size = item.GetSize(e.Graphics, Font);
                if (size.Width > width) width = size.Width;
                height += size.Height;
                height += itemGap;
            }
//            Width = width + leftMargin + rightMargin;
//            Height = height + topMargin + bottomMargin;

            int i = 0;
            foreach (ColorLabel item in labels)
            {
                item.Draw(e.Graphics, leftMargin, y[i] + topMargin, Font, ForeColor, doubleBufferedDrawBox.BackColor);
                i++;
            }
            e.Graphics.DrawRectangle(new Pen(Color.Gray), 0, 0, Width - 1, Height - 1);
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            scrollPosition = vScrollBar.Value;
        }
    }
}
