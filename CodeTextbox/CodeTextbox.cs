//Copyright(c) 2018 ajkfds

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

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
        public CodeTextbox()
        {
            InitializeComponent();
//            dbDrawBox.SetImeEnable(true);
            resizeCharSize();
//            this.dbDrawBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.dbDrawBox_MouseWheel);

            this.vScrollBar.Width = Global.ScrollBarWidth;
            this.hScrollBar.Height = Global.ScrollBarWidth;
        }

        public event EventHandler CarletLineChanged;
        public event KeyPressEventHandler AfterKeyPressed;
        public event KeyEventHandler AfterKeyDown;
        public event KeyPressEventHandler BeforeKeyPressed;
        public event KeyEventHandler BeforeKeyDown;
        public event Action SelectionChanged;

        public void Cut()
        {
            if (document == null) return;
            if (document.SelectionStart == document.SelectionLast) return;
            string clipText = document.CreateString(document.SelectionStart, document.SelectionLast - document.SelectionStart);
            try
            {
                Clipboard.SetText(clipText);
            }
            catch { }
            documentReplace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, "");
            UpdateVScrollBarRange();
            caretChanged();
            selectionChanged();
            Invoke(new Action(Refresh));
        }

        public void Copy()
        {
            if (document == null) return;
            if (document.SelectionStart == document.SelectionLast) return;
            string clipText = document.CreateString(document.SelectionStart, document.SelectionLast - document.SelectionStart);
            try
            {
                Clipboard.SetText(clipText);
            }
            catch { }
            UpdateVScrollBarRange();
            caretChanged();
            selectionChanged();
            Invoke(new Action(Refresh));
        }

        public void Paste()
        {
            if (document == null) return;
            if (!Clipboard.ContainsText()) return;
            string newText = Clipboard.GetText();
            documentReplace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, newText);
            document.CaretIndex = document.SelectionStart + newText.Length;
            document.SelectionStart = document.CaretIndex;
            document.SelectionLast = document.CaretIndex;
            UpdateVScrollBarRange();
            caretChanged();
            selectionChanged();
            Invoke(new Action(Refresh));
        }

        public void Undo()
        {
            document.Undo();
            UpdateVScrollBarRange();
            caretChanged();
            scrollToCaret();
            selectionChanged();
            Invoke(new Action(Refresh));
        }

        public void SelectAll()
        {
            document.SelectionStart = 0;
            document.SelectionLast = document.Length - 1;
            selectionChanged();
            Invoke(new Action(Refresh));
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
                reGenarateBuffer = true;
            }
        }

        private float size = 8;
        private void dbDrawBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            { // zoom up/down
                if (e.Delta < 0) size--;
                if (e.Delta > 0) size++;
                if (size < 5) size = 5;
                if (size > 20) size = 20;
                this.Font = new Font(this.Font.FontFamily, size, this.Font.Style);
                Refresh();
            }
            else
            { // 
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

        public bool Editable { get; set; } = true;

        private bool multiLine = true;
        public bool MultiLine {
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
                    Height = charSizeY;
                    resizeDrawBuffer();
                }
            }
        }

        private Document document;
        public Document Document
        {
            get
            {
                return document;
            }
            set
            {
                if (document == value) return;
                if (document != null)
                {
                    document.Replaced = null;
                }
                document = value;
                if (document != null)
                {
                    document.Replaced = hilightUpdateWhenDocReplaced;
                }
                UpdateVScrollBarRange();
                caretChanged();
                scrollToCaret();
                if (Handle != null) Invoke(new Action(Refresh));
            }
        }

        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                size = value.SizeInPoints;
                resizeCharSize();
            }
        }

        //

        private void CodeTextbox_Resize(object sender, EventArgs e)
        {
            //            System.Diagnostics.Debug.Print("codeTextbox_resize");
            resizeDrawBuffer();
        }

        private void dbDrawBox_Resize(object sender, EventArgs e)
        {
            //           System.Diagnostics.Debug.Print("dbDrawdbox_resize");
            //            resizeCharSize();
        }




        // draw image
        private int tabSize = 4;

        private static IconImage plusIcon = new IconImage(Properties.Resources.plus);
        private static IconImage minusIcon = new IconImage(Properties.Resources.minus);

        private int xOffset = 0;
        private int caretX = 0;
        private int caretY = 0;
        private int[] actualLineNumbers = new int[] { };


        public int GetActualLineNo(int drawLineNumber)
        {
            if (actualLineNumbers.Length < drawLineNumber) return actualLineNumbers.Last();
            return actualLineNumbers[drawLineNumber];
        }
        public int GetIndexAt(int x, int y)
        {
            return hitIndex(x, y);
        }

        public Point GetCaretTopPoint()
        {
            return new Point(caretX, caretY);
        }

        public Point GetCaretBottomPoint()
        {
            return new Point(caretX, caretY + charSizeY);
        }


        public void SetSelection(int index, int length)
        {
            document.CaretIndex = index + length;
            document.SelectionStart = index;
            document.SelectionLast = index + length;
            selectionChanged();
            Invoke(new Action(Refresh));
        }


        // UI ///////////////////////////////////////////////////////////////////////////////

        private enum uiState
        {
            idle,
            selecting
        }

        private uiState state = CodeTextbox.uiState.idle;

        private void vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (document == null) return;
            selectionChanged();
            Invoke(new Action(Refresh));
        }
        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {

        }



        public void ScrollToCaret()
        {
            selectionChanged();
            scrollToCaret();
            Invoke(new Action(Refresh));
        }

        public void documentReplace(int index, int replaceLength, byte colorIndex, string text)
        {
            document.Replace(index, replaceLength, colorIndex, text);
        }



        private void caretChanged()
        {
            if (document == null) return;
            if (document.CaretIndex == 0) return;

            // move caret on CRLF to CR
            if (document.GetCharAt(document.CaretIndex) == '\n' && document.GetCharAt(document.CaretIndex - 1) == '\r')
            {
                document.CaretIndex--;
            }
        }


        private void selectionChanged()
        {
            if (document == null) return;

            if (document.SelectionStart > document.SelectionLast)
            {
                int last = document.SelectionLast;
                document.SelectionLast = document.SelectionStart;
                document.SelectionStart = last;
            }
            if (document.SelectionStart > document.Length) document.SelectionStart = document.Length;
            if (document.SelectionLast > document.Length) document.SelectionLast = document.Length;

            if (SelectionChanged != null) SelectionChanged();
        }

        public void DoAtVisibleLines(Action<int> action)
        {
            int lineStart = document.GetActialLineNo(vScrollBar.Value + 1);
            int drawLine = 0;
            int line = lineStart;
            while (line < document.Lines)
            {
                if (drawLine >= visibleLines + 2)
                {
                    break;
                }
                if (!document.IsVisibleLine(line))
                {
                    line++;
                    while (line < document.Lines)
                    {
                        if (document.IsVisibleLine(line)) break;
                        line++;
                    }
                    continue;
                }

                action(line);

                drawLine++;
                line++;
            }
        }

        /// <summary>
        /// move vscrollbar to carlet visible position
        /// </summary>
        private void scrollToCaret()
        {
            if (document == null) return;

            int line = document.GetVisibleLine(document.GetLineAt(document.CaretIndex));

            if (line - 1 < vScrollBar.Value)
            {
                if (line < 1) vScrollBar.Value = 0;
                else vScrollBar.Value = line - 1;
            }
            else if (line >= visibleLines + vScrollBar.Value)
            {
                int v = line - visibleLines;
                if (v < vScrollBar.Minimum)
                {
                    vScrollBar.Value = vScrollBar.Minimum;
                } else if (v > vScrollBar.Maximum)
                {
                    vScrollBar.Value = vScrollBar.Maximum;
                }
                else
                {
                    if (v + 1 < vScrollBar.Maximum)
                    {
                        vScrollBar.Value = v + 1; // to skip half visible line 
                    }
                    else
                    {
                        vScrollBar.Value = v;
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void scrollToSelection()
        {
            if (document == null) return;

            int startLine = document.GetVisibleLine(document.GetLineAt(document.SelectionStart));
            int lastLine = document.GetVisibleLine(document.GetLineAt(document.SelectionLast));

            if (startLine < vScrollBar.Value)
            {
                vScrollBar.Value = startLine;
            }
            else if (lastLine > visibleLines + vScrollBar.Value)
            {
                int v = lastLine - visibleLines;
                if (v >= 0) vScrollBar.Value = lastLine - visibleLines;
            }
            else
            {
                return;
            }
        }

        private void UpdateVScrollBarRange()
        {
            if (document != null) vScrollBar.Maximum = document.VisibleLines;
        }

        IntPtr _OriginalWndProcObj = IntPtr.Zero;
        WinApi.WNDPROC _CustomWndProcObj = null;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            //_HighlighterDelayTimer = new WinFormsTimer();
            //_HighlighterDelayTimer.Tick += delegate {
            //    _HighlighterDelayTimer.Enabled = false;
            //    if (_Impl != null)
            //        _Impl.ExecHighlighter();
            //};

            // rewrite window procedure at first
            RewriteWndProc();

            // set default value for each scroll bar
            // (setting scroll bar range forces the window to have style of WS_VSCROLL/WS_HSCROLL)
            //WinApi.SetScrollRange(Handle, false, 0, 1, 1);
            //WinApi.SetScrollRange(Handle, true, 0, 1, 1);

            //base.Cursor = Cursors.IBeam;
            //this.Font = base.Font;
            //this.BorderStyle = _BorderStyle;

            //WinApi.CreateCaret(Handle, _CaretSize);
            //WinApi.SetCaretPos(0, 0);

            //// calculate scrollbar width
            //using (ScrollBar sb = new VScrollBar())
            //{
            //    _ScrollBarWidth = sb.Width;
            //}
        }
        void RewriteWndProc()
        {
            const int GWL_WNDPROC = -4;

            _OriginalWndProcObj = WinApi.GetWindowLong(Handle, GWL_WNDPROC);
            if (_CustomWndProcObj == null)
            {
                _CustomWndProcObj = new WinApi.WNDPROC(this.CustomWndProc);
            }

            WinApi.SetWindowLong(Handle, GWL_WNDPROC, _CustomWndProcObj);
        }

        //region Custom Window Procedure (handling v/h scroll and paint event etc.)
        IntPtr CustomWndProc(IntPtr window, UInt32 message, IntPtr wParam, IntPtr lParam)
        {
            if (message == WinApi.WM_PAINT)
            {
                WinApi.PAINTSTRUCT ps;

                // .NET's Paint event does not inform invalidated region when double buffering was disabled.
                // In addition to this, Control.SetStyle is not supported in Compact Framework
                // and thus enabling double buffering seems impossible.
                // Therefore painting logic is called here.
                unsafe
                {
                    WinApi.BeginPaint(window, &ps);

                    Rectangle rect = new Rectangle(ps.paint.left, ps.paint.top, ps.paint.right - ps.paint.left, ps.paint.bottom - ps.paint.top);
                    draw(rect);
//                    _Impl.HandlePaint(rect);

                    WinApi.EndPaint(window, &ps);
                }

                // return zero here to prevent executing original painting logic of Control class.
                // (if the original logic runs,
                // we will get invalid(?) update region from BeginPaint API in Windows XP or former.)
                return IntPtr.Zero;
            }
            return WinApi.CallWindowProc(_OriginalWndProcObj, window, message, wParam, lParam);
        }


    }
}
