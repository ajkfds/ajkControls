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
    public partial class DoubleBufferedDrawBox : UserControl
    {
        public DoubleBufferedDrawBox()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        public delegate void DoubleBufferedPaintHandler(PaintEventArgs e);
        public event DoubleBufferedPaintHandler DoubleBufferedPaint;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (DoubleBufferedPaint != null) DoubleBufferedPaint(e);
        }

        // 特殊キーの無効化
        protected override bool IsInputKey(Keys keyData)
        {
            return true;
            if (keyData == (Keys.Z | Keys.Control)) return true;
            if (keyData == (Keys.X | Keys.Control)) return true;
            if (keyData == (Keys.C | Keys.Control)) return true;
            if (keyData == (Keys.V | Keys.Control)) return true;
            if (keyData == Keys.Tab) return true;
            if (keyData == (Keys.Tab | Keys.Shift)) return true;
            if (keyData == (Keys.Down | Keys.Shift)) return true;
            if (keyData == (Keys.Up | Keys.Shift)) return true;
            if (keyData == (Keys.Left | Keys.Shift)) return true;
            if (keyData == (Keys.Right | Keys.Shift)) return true;
            if (keyData == Keys.Right || keyData == Keys.Left || keyData == Keys.Up || keyData == Keys.Down) return true;
            return base.IsInputKey(keyData);
        }

        // IMEを有効にする
        public void SetImeEnable(bool value)
        {
            canEnableIme = value;
        }

        private bool canEnableIme = true;
        protected override bool CanEnableIme
        {
            get
            {
                return canEnableIme;
            }
        }

    }
}
