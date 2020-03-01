using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace ajkControls
{
    public partial class CodeTextbox : UserControl
    {
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                if (base.BackColor == value) return; // avoid infinite loop on VS designer

                base.BackColor = value;
                reGenarateBuffer = true;
            }
        }


        private Color tabColor = Color.LightGray;
        public Color TabColor
        {
            get
            {
                return tabColor;
            }
            set
            {
                tabColor = value;
            }
        }

        private Color crColor = Color.LightGray;
        public Color CrColor
        {
            get
            {
                return crColor;
            }
            set
            {
                crColor = value;
            }
        }

        private Color lfColor = Color.LightGray;
        public Color LfColor
        {
            get
            {
                return lfColor;
            }
            set
            {
                lfColor = value;
            }
        }

        private Color carletColor = Color.LightGray;
        public Color CarletColor
        {
            get
            {
                return carletColor;
            }
            set
            {
                carletColor = value;
            }
        }

        private Color carletUnderlineColor = Color.LightGray;
        public Color CarletUnderlineColor
        {
            get
            {
                return carletUnderlineColor;
            }
            set
            {
                carletUnderlineColor = value;
            }
        }

        //        public Color selectedColor = Color.FromArgb(60, Color.FromArgb(0x5b, 0x7d, 0x9f));

        private Color selectionColor = Color.CadetBlue;
        public Color SelectionColor
        {
            get
            {
                return selectionColor;
            }
            set
            {
                selectionColor = value;
            }
        }

        private Color leftColumnColor = Color.Gray;
        public Color LeftColumnColor
        {
            get
            {
                return leftColumnColor;
            }
            set
            {
                leftColumnColor = value;
            }
        }

        private Color lineNumberColor = Color.Gray;
        public Color LineNumberColor
        {
            get
            {
                return lineNumberColor;
            }
            set
            {
                lineNumberColor = value;
            }
        }

    }
}
