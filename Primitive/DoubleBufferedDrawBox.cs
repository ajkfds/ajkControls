using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ajkControls.Primitive
{
    public partial class DoubleBufferedDrawBox : UserControl
    {
        public DoubleBufferedDrawBox()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
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

        // activate scecial input keys
        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }

        // enable ime
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
