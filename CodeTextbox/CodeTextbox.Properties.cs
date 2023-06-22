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

namespace ajkControls.CodeTextbox
{
    public partial class CodeTextbox : UserControl
    {
        public bool Editable { get; set; } = true;

        public int TabSize { get; set; } = 4;
        public Color TabColor { get; set; } = Color.LightGray;
        public Color CrColor { get; set; } = Color.LightGray;
        public Color LfColor { get; set; } = Color.LightGray;
        public Color CarletColor { get; set; } = Color.LightGray;
        public Color CarletUnderlineColor { get; set; } = Color.LightGray;
        public Color BlockUnderlineColor { get; set; } = Color.LightGray;
        public Color SelectionColor { get; set; } = Color.CadetBlue;
        public Color LeftColumnColor { get; set; } = Color.Gray;
        public Color LineNumberColor { get; set; } = Color.Gray;

        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                if (base.BackColor == value) return; // avoid infinite loop on VS designer
                base.BackColor = value;
                Drawer.ReGenarateBuffer = true;
            }
        }
        private bool multiLine = true;
        public bool MultiLine
        {
            get
            {
                return multiLine;
            }
            set
            {
                if (multiLine == value) return;
                multiLine = value;
                System.IntPtr handle = Handle; // avoid windowhandle not created error
                Invoke(new Action(Refresh));
                if (multiLine == false)
                {
                    if (ScrollBarVisible) ScrollBarVisible = false;
                    Height = Drawer.charSizeY;
                    Drawer.ResizeDrawBuffer();
                }
            }
        }

        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                size = value.SizeInPoints;
                Drawer.ResizeCharSize();
            }
        }

        private CodeDrawStyle style = new CodeDrawStyle();
        public CodeDrawStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                if (style == value) return;
                style = value;
                Drawer.ReGenarateBuffer = true;
            }
        }
        public bool ScrollBarVisible
        {
            get
            {
                return vScrollBar.Visible;
            }
            set
            {
                vScrollBar.Visible = value;
                hScrollBar.Visible = value;
            }
        }


    }
}
