using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace ajkControls.ColorLabel
{
    public partial class ColorLabelList : UserControl, IEnumerable<ColorLabel>
    {
        public ColorLabelList()
        {
            InitializeComponent();
            //            this.vScrollBar.Width = Global.ScrollBarWidth;
        }

        public IEnumerator<ColorLabel> GetEnumerator()
        {
            return labels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ColorLabel>)labels).GetEnumerator();
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

        private List<ColorLabel> labels = new List<ajkControls.ColorLabel.ColorLabel>();

        public void Clear()
        {
            labels.Clear();
            updateLabels();
            Invalidate();
        }

        public void Add(ColorLabel colorLabel)
        {
            labels.Add(colorLabel);
        }

        public ColorLabel this [int index]
        {
            get
            {
                return labels[index];
            }
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
//        private int bottomMargin = 4;
        private int leftMargin = 4;
//        private int rightMargin = 4;
        private int itemGap = 2;

        Dictionary<ColorLabel, int> labelDrawY = new Dictionary<ColorLabel, int>();
        private void doubleBufferedDrawBox_DoubleBufferedPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            int width = 0;
            int y = 0;

            labelDrawY.Clear();
            for (int i = vScrollBar.Value; i < labels.Count; i++)
            {
                ColorLabel label = labels[i];
                labelDrawY.Add(label, y);
                Size size = label.GetSize(e.Graphics, Font);
                if (size.Width > width) width = size.Width;
                y += size.Height;
                y += itemGap;
                if (y > Height) break;
            }

            foreach (var labelInfo in labelDrawY)
            {
                labelInfo.Key.Draw(e.Graphics, leftMargin, labelInfo.Value + topMargin, Font, ForeColor, doubleBufferedDrawBox.BackColor);
            }

            e.Graphics.DrawRectangle(new Pen(Color.Gray), 0, 0, Width - 1, Height - 1);
        }

        private void hitTest(int x, int y, out ColorLabel hitResult)
        {
            hitResult = null;
            foreach (var labelInfo in labelDrawY)
            {
                if (y > labelInfo.Value)
                {
                    hitResult = labelInfo.Key;
                }
            }
        }

        public delegate void DColorLabelClicked(ColorLabel colorLabel);
        public event DColorLabelClicked ColorLabelClicked;


        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            scrollPosition = vScrollBar.Value;
        }

        private void doubleBufferedDrawBox_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        private void doubleBufferedDrawBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyPress(e);
        }

        private void doubleBufferedDrawBox_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
        }

        private void doubleBufferedDrawBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            OnPreviewKeyDown(e);
        }

        private void doubleBufferedDrawBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (ColorLabelClicked == null) return;
            ColorLabel hitLabel;
            hitTest(e.X, e.Y, out hitLabel);
            if (hitLabel == null) return;
            ColorLabelClicked(hitLabel);
        }
    }
}
