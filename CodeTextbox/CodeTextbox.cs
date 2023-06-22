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


namespace ajkControls.CodeTextbox
{

    public partial class CodeTextbox : UserControl
    {
        public CodeTextbox()
        {
            InitializeComponent();


            //            dbDrawBox.SetImeEnable(true);
            Drawer.ResizeCharSize();
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.CodeTextbox_MouseWheel);

            this.vScrollBar.Width = Global.ScrollBarWidth;
            this.hScrollBar.Height = Global.ScrollBarWidth;

        }

        #region Handler Definition

        Drawer _drawer;
        private Drawer Drawer
        {
            get
            {   // initialize drawer instance here to support code designer instantiation
                if (_drawer == null) _drawer = new Drawer(this, hScrollBar, vScrollBar);
                return _drawer;
            }
        }
        private KeyHandler _keyHandler;
        private KeyHandler KeyHandler
        {
            get
            {
                if (_keyHandler == null) _keyHandler = new KeyHandler(this, Drawer, hScrollBar, vScrollBar);
                return _keyHandler;
            }
        }

        MouseHandler _mouseHandler;
        private MouseHandler MouseHandler
        {
            get
            {
                if (_mouseHandler == null) _mouseHandler = new MouseHandler(this, Drawer, this.hScrollBar, this.vScrollBar);
                return _mouseHandler;
            }
        }
        Highlighter _highlighter;
        private Highlighter Highlighter
        {
            get
            {
                if (_highlighter == null) _highlighter = new Highlighter(this);
                return _highlighter;
            }
        }

        #endregion


        public enum MarkStyleEnum
        {
            underLine,
            wave,
            wave_inv,
            fill
        }

        public event EventHandler CarletLineChanged;
        public event KeyPressEventHandler AfterKeyPressed;
        public event KeyEventHandler AfterKeyDown;
        public event KeyPressEventHandler BeforeKeyPressed;
        public event KeyEventHandler BeforeKeyDown;
        public event Action SelectionChanged;

        internal void CallCarletLineChanged(EventArgs e)
        {
            if (CarletLineChanged != null) CarletLineChanged(this, e);
        }
        internal void CallAfterKeyPressed(KeyPressEventArgs e)
        {
            if (AfterKeyPressed != null) AfterKeyPressed(this, e);
        }
        internal void CallAfterKeyDown(KeyEventArgs e)
        {
            if (AfterKeyDown != null) AfterKeyDown(this, e);
        }
        internal void CallBeforeKeyPressed(KeyPressEventArgs e)
        {
            if (BeforeKeyPressed != null) BeforeKeyPressed(this, e);
        }
        internal void CallBeforeKeyDown(KeyEventArgs e)
        {
            if (BeforeKeyDown != null) BeforeKeyDown(this, e);
        }

        #region Message Handler

        IntPtr _OriginalWndProcObj = IntPtr.Zero;
        Primitive.WinApi.WNDPROC _CustomWndProcObj = null;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // rewrite window procedure at first
            RewriteWndProc();
        }
        private void RewriteWndProc()
        {
            const int GWL_WNDPROC = -4;

            _OriginalWndProcObj = Primitive.WinApi.GetWindowLong(Handle, GWL_WNDPROC);
            if (_CustomWndProcObj == null)
            {
                _CustomWndProcObj = new Primitive.WinApi.WNDPROC(this.CustomWndProc);
            }
            Primitive.WinApi.SetWindowLong(Handle, GWL_WNDPROC, _CustomWndProcObj);
        }

        // Custom Window Procedure (partilly based on Azuki Editor Code
        IntPtr CustomWndProc(IntPtr window, UInt32 message, IntPtr wParam, IntPtr lParam)
        {
            if (message == Primitive.WinApi.WM_PAINT)
            {
                Primitive.WinApi.PAINTSTRUCT ps;

                // .NET's Paint event does not inform invalidated region when double buffering was disabled.
                // In addition to this, Control.SetStyle is not supported in Compact Framework
                // and thus enabling double buffering seems impossible.
                // Therefore painting logic is called here.
                unsafe
                {
                    Primitive.WinApi.BeginPaint(window, &ps);

                    Rectangle rect = new Rectangle(ps.paint.left, ps.paint.top, ps.paint.right - ps.paint.left, ps.paint.bottom - ps.paint.top);
                    Drawer.Draw(rect);

                    Primitive.WinApi.EndPaint(window, &ps);
                }

                // return zero here to prevent executing original painting logic of Control class.
                // (if the original logic runs,
                // we will get invalid(?) update region from BeginPaint API in Windows XP or former.)
                return IntPtr.Zero;
            }
            return Primitive.WinApi.CallWindowProc(_OriginalWndProcObj, window, message, wParam, lParam);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // restrict erace back ground
            //DO_NOT//base.OnPaintBackground( e );
        }

        #endregion


        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }

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


        private float size = 8;
        private void CodeTextbox_MouseWheel(object sender, MouseEventArgs e)
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


        #region Message Handler

        // key handler
        private void CodeTextbox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            KeyHandler.PreviewKeyDown(sender, e);
        }

        private void CodeTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            KeyHandler.KeyDown(sender, e);
        }

        private void CodeTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            KeyHandler.KeyUp(sender, e);
        }

        private void CodeTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyHandler.KeyPress(sender, e);
        }

        // mouse handler
        private void CodeTextbox_MouseEnter(object sender, EventArgs e)
        {
            MouseHandler.CodeTextbox_MouseEnter(sender, e);
        }

        private void CodeTextbox_MouseHover(object sender, EventArgs e)
        {
            MouseHandler.CodeTextbox_MouseHover(sender, e);
        }
        private void CodeTextbox_MouseDown(object sender, MouseEventArgs e)
        {
            MouseHandler.CodeTextbox_MouseDown(sender, e);
        }

        private void CodeTextbox_MouseMove(object sender, MouseEventArgs e)
        {
            MouseHandler.CodeTextbox_MouseMove(sender, e);
        }

        private void CodeTextbox_MouseUp(object sender, MouseEventArgs e)
        {
            MouseHandler.CodeTextbox_MouseUp(sender, e);
        }
        private void CodeTextbox_MouseLeave(object sender, EventArgs e)
        {
            MouseHandler.CodeTextbox_MouseLeave(sender, e);
        }

        private void CodeTextbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MouseHandler.CodeTextbox_MouseDoubleClick(sender, e);
        }

        // highlight handler
        private void hilightUpdateWhenDocReplaced(int index, int replaceLength, byte colorIndex, string text)
        {
            Highlighter.HilightUpdateWhenDocReplaced(index, replaceLength, colorIndex, text);
        }
        public void MoveToNextHighlight(out bool moved)
        {
            Highlighter.MoveToNextHighlight(out moved);
        }

        public void GetHighlightPosition(int highlightIndex, out int highlightStart, out int highlightLast)
        {
            Highlighter.GetHighlightPosition(highlightIndex, out highlightStart, out highlightLast);
        }

        public void SelectHighlight(int highlightIndex)
        {
            Highlighter.SelectHighlight(highlightIndex);
        }

        public int GetHighlightIndex(int index)
        {
            return Highlighter.GetHighlightIndex(index);
        }

        public void ClearHighlight()
        {
            Highlighter.ClearHighlight();
        }

        public void AppendHighlight(int highlightStart, int highlightLast)
        {
            Highlighter.AppendHighlight(highlightStart, highlightLast);
        }

        public void ReDrawHighlight()
        {
            Highlighter.ReDrawHighlight();
        }

        // drawer


        #endregion

        protected Document document;
        public virtual Document Document
        {
            get
            {
                if (document == null || document.IsDisposed) return null;
                return document;
            }
            set
            {
                if (document == value) return;
                if (document != null)
                {
                    ClearHighlight();
                    document.Replaced = null;
                }
                document = value;
                if (document != null)
                {
                    document.Replaced = hilightUpdateWhenDocReplaced;
                }
                Drawer.Clean();
                UpdateVScrollBarRange();
                caretChanged();
                scrollToCaret();
                if (Handle != null) Invoke(new Action(Refresh));
            }
        }




        public Point GetCaretTopPoint()
        {
            return Drawer.GetCaretTopPoint();
        }

        public Point GetCaretBottomPoint()
        {
            return Drawer.GetCaretBottomPoint();
        }

        //

        private void CodeTextbox_Resize(object sender, EventArgs e)
        {
            //            System.Diagnostics.Debug.Print("codeTextbox_resize");
            Drawer.ResizeDrawBuffer();
        }

        private void dbDrawBox_Resize(object sender, EventArgs e)
        {
            //           System.Diagnostics.Debug.Print("dbDrawdbox_resize");
            //            resizeCharSize();
        }

        // draw image
        public int GetActualLineNo(int drawLineNumber)
        {
            return Drawer.GetActualLineNo(drawLineNumber);
        }
        public int GetIndexAt(int x, int y)
        {
            return Drawer.hitIndex(x, y);
        }

        public void Invalidate(int beginIndex, int endIndex)
        {
            lock (document)
            {
                Drawer.InvalidateLines(document.GetLineAt(beginIndex), document.GetLineAt(endIndex));
            }
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

        internal enum uiState
        {
            idle,
            selecting
        }

        internal uiState state = CodeTextbox.uiState.idle;

        private void vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (document == null) return;
            selectionChanged();
            Invoke(new Action(Refresh));
        }
        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {

        }
        private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void hScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (document == null) return;
            selectionChanged();
            Invoke(new Action(Refresh));
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



        internal void caretChanged()
        {
            if (document == null) return;
            if (document.CaretIndex == 0) return;

            // move caret on CRLF to CR
            if (document.GetCharAt(document.CaretIndex) == '\n' && document.GetCharAt(document.CaretIndex - 1) == '\r')
            {
                document.CaretIndex--;
            }
        }


        internal void selectionChanged()
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
                if (drawLine >= Drawer.visibleLines + 2)
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
        internal void scrollToCaret()
        {
            if (document == null) return;

            int line = document.GetVisibleLine(document.GetLineAt(document.CaretIndex));

            if (line - 1 < vScrollBar.Value)
            {
                if (line < 1) vScrollBar.Value = 0;
                else vScrollBar.Value = line - 1;
            }
            else if (line >= Drawer.visibleLines + vScrollBar.Value)
            {
                int v = line - Drawer.visibleLines;
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
            else if (lastLine > Drawer.visibleLines + vScrollBar.Value)
            {
                int v = lastLine - Drawer.visibleLines;
                if (v >= 0) vScrollBar.Value = lastLine - Drawer.visibleLines;
            }
            else
            {
                return;
            }
        }

        internal void UpdateVScrollBarRange()
        {
            if (document != null) vScrollBar.Maximum = document.VisibleLines;
        }

    }
}
