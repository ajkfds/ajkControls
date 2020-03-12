using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ajkControls.Scrollbar
{
    public partial class ScrollBar : UserControl
    {
        public ScrollBar()
        {
            InitializeComponent();
        }


        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }

            set
            {
                base.ForeColor = value;
                doubleBufferedDrawBox.BackColor = value;
            }
        }
    }
}
