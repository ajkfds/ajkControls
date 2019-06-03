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
            dbDrawBox.SetImeEnable(true);
            resizeCharSize();
            this.dbDrawBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.dbDrawBox_MouseWheel);

            this.vScrollBar.Width = Global.ScrollBarWidth;
            this.hScrollBar.Height = Global.ScrollBarWidth;
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        extern static int TextOut(IntPtr hdc, int x, int y, string text, int length);

        [DllImport("gdi32.dll")]
        private static extern int SelectObject(IntPtr hdc, IntPtr hgdiObj);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern int SetTextColor(IntPtr hdc, int color);

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        static extern bool FillRgn(IntPtr hdc, IntPtr hrgn, IntPtr hbr);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect,
            int nBottomRect);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateSolidBrush(uint crColor);

        [DllImport("gdi32.dll")]
        static extern uint SetBkColor(IntPtr hdc, int crColor);


        public event EventHandler CarletLineChanged;
        public event KeyPressEventHandler AfterKeyPressed;
        public event KeyEventHandler AfterKeyDown;
        public event KeyPressEventHandler BeforeKeyPressed;
        public event KeyEventHandler BeforeKeyDown;

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
            document.Replace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, "");
            UpdateVScrollBarRange();
            caretChanged();
            selectionChanged();
            Invoke(new Action(dbDrawBox.Refresh));
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
            Invoke(new Action(dbDrawBox.Refresh));
        }

        public void Paste()
        {
            if (document == null) return;
            if (!Clipboard.ContainsText()) return;
            string newText = Clipboard.GetText();
            document.Replace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, newText);
            document.CaretIndex = document.SelectionStart + newText.Length;
            document.SelectionStart = document.CaretIndex;
            document.SelectionLast = document.CaretIndex;
            UpdateVScrollBarRange();
            caretChanged();
            selectionChanged();
            Invoke(new Action(dbDrawBox.Refresh));
        }

        public void Undo()
        {
            document.Undo();
            UpdateVScrollBarRange();
            caretChanged();
            scrollToCaret();
            selectionChanged();
            Invoke(new Action(dbDrawBox.Refresh));
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
                dbDrawBox.Refresh();
            }
            else
            { // 
                int delta = (int)(Global.WheelSensitivity * e.Delta) * SystemInformation.MouseWheelScrollLines;
                if(delta == 0)
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
                Invoke(new Action(dbDrawBox.Refresh));
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
                document = value;
                UpdateVScrollBarRange();
                if (Handle != null) Invoke(new Action(dbDrawBox.Refresh));
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
        public override Color BackColor {
            get { return base.BackColor; }
            set
            {
                if (base.BackColor == value) return; // avoid infinite loop on VS designer

                base.BackColor = value;
                reGenarateBuffer = true;
            }
        }

        private void CodeTextbox_Resize(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("codeTextbox_resize");
            resizeDrawBuffer();
        }

        private void dbDrawBox_Resize(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("dbDrawdbox_resize");
            //            resizeCharSize();
        }


        int charSizeX = 0;
        int charSizeY = 0;
        int visibleLines = 10;
        private void resizeCharSize()
        {
            System.Diagnostics.Debug.Print("risezicharsize");
            Graphics g = dbDrawBox.CreateGraphics();
            Size fontSize = System.Windows.Forms.TextRenderer.MeasureText(g, "A", Font, new Size(100, 100), TextFormatFlags.NoPadding);
            charSizeX = fontSize.Width;
            charSizeY = fontSize.Height;
            resizeDrawBuffer();
            reGenarateBuffer = true;
        }

        private void resizeDrawBuffer()
        {
            visibleLines = (int)Math.Ceiling((double)(dbDrawBox.Height / charSizeY));
            System.Diagnostics.Debug.Print("risezicharsize visibleLines " + visibleLines.ToString());
            vScrollBar.LargeChange = visibleLines;
            UpdateVScrollBarRange();
        }


        // character & mark graphics buffer
        volatile bool reGenarateBuffer = true;
        Bitmap[,] charBitmap = new Bitmap[16, 128];
        Bitmap[] markBitmap = new Bitmap[8];

        private void createGraphicsBuffer()
        {
            System.Diagnostics.Debug.Print("regen buffer");
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Bitmap bmp = new Bitmap(charSizeX, charSizeY);
            Color controlColor = Color.DarkGray;
            Pen controlPen = new Pen(controlColor);
            
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                for (int colorIndex = 0; colorIndex < 16; colorIndex++)
                {
                    Color color = Style.ColorPallet[colorIndex];
                    int colorNo = (color.B << 16) + (color.G << 8) + color.R;

                    for (int i = 0; i < 0x20; i++)
                    {
                        g.Clear(BackColor);
                        // control codes
                        switch (i)
                        {
                            case '\r':
                                g.DrawLine(controlPen, new Point((int)(charSizeX * 0.9), (int)(charSizeY * 0.2)), new Point((int)(charSizeX * 0.9), (int)(charSizeY * 0.6)));

                                g.DrawLine(controlPen, new Point((int)(charSizeX * 0.2), (int)(charSizeY * 0.6)), new Point((int)(charSizeX * 0.9), (int)(charSizeY * 0.6)));
                                g.DrawLine(controlPen, new Point((int)(charSizeX * 0.4), (int)(charSizeY * 0.4)), new Point((int)(charSizeX * 0.2), (int)(charSizeY * 0.6)));
                                g.DrawLine(controlPen, new Point((int)(charSizeX * 0.4), (int)(charSizeY * 0.8)), new Point((int)(charSizeX * 0.2), (int)(charSizeY * 0.6)));
                                break;
                            case '\n':
                                g.DrawLine(controlPen, new Point((int)(charSizeX * 0.6), (int)(charSizeY * 0.2)), new Point((int)(charSizeX * 0.6), (int)(charSizeY * 0.8)));
                                g.DrawLine(controlPen, new Point((int)(charSizeX * 0.4), (int)(charSizeY * 0.6)), new Point((int)(charSizeX * 0.6), (int)(charSizeY * 0.8)));
                                g.DrawLine(controlPen, new Point((int)(charSizeX * 0.8), (int)(charSizeY * 0.6)), new Point((int)(charSizeX * 0.6), (int)(charSizeY * 0.8)));
                                break;
                            default:
                                g.DrawRectangle(new Pen(Color.DarkGray), new Rectangle(1, 1 + 2, charSizeX - 2, charSizeY - 2 - 2));
                                break;
                        }
                        if (charBitmap[colorIndex, i] != null) charBitmap[colorIndex, i].Dispose();
                        charBitmap[colorIndex, i] = (Bitmap)bmp.Clone();
                    }
                }
                g.Clear(BackColor);

                //                System.Diagnostics.Debug.Print("regen buffer0 " + sw.ElapsedMilliseconds.ToString() + "ms");
                for (int colorIndex = 0; colorIndex < 16; colorIndex++)
                {
                    Color color = Style.ColorPallet[colorIndex];
                    int colorNo = (color.B << 16) + (color.G << 8) + color.R;

                    for (int i = 0x20; i < 128; i++)
                    {
                        //if (i == 0x26)
                        //{
                        //    System.Windows.Forms.TextRenderer.DrawText(gc, "&&", Font, new Point(0, 0), Style.ColorPallet[color], BackColor, TextFormatFlags.NoPadding);
                        //}
                        //else
                        //{
                        //    System.Windows.Forms.TextRenderer.DrawText(gc, ((char)i).ToString(), Font, new Point(0, 0), Style.ColorPallet[color], BackColor, TextFormatFlags.NoPadding);
                        //}
                        IntPtr hDC = g.GetHdc();
                        IntPtr hFont = this.Font.ToHfont();
                        IntPtr hOldFont = (IntPtr)SelectObject(hDC, hFont);

                        SetTextColor(hDC, colorNo);
                        TextOut(hDC, 0, 0, ((char)i).ToString(), 1);

                        DeleteObject((IntPtr)SelectObject(hDC, hOldFont));
                        g.ReleaseHdc(hDC);
                        if (charBitmap[colorIndex, i] != null) charBitmap[colorIndex, i].Dispose();
                        charBitmap[colorIndex, i] = (Bitmap)bmp.Clone();
                    }
                }

            }
//            System.Diagnostics.Debug.Print("regen buffer1 " + sw.ElapsedMilliseconds.ToString() + "ms");
            for (int mark = 0; mark < 8; mark++)
            {
                if (markBitmap[mark] != null) markBitmap[mark].Dispose();
                markBitmap[mark] = new Bitmap(charSizeX, charSizeY);
                using (Graphics gc = Graphics.FromImage(markBitmap[mark]))
                {
                    gc.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    controlPen = new Pen(Style.MarkColor[mark]);
                    switch (Style.MarkStyle[mark])
                    {
                        case MarkStyleEnum.underLine:
                            for (int i = 0; i < 2; i++)
                            {
                                gc.DrawLine(controlPen,
                                new Point(0, (int)(charSizeY * 0.8) + i),
                                new Point((int)charSizeX, (int)(charSizeY * 0.8) + i)
                                );
                            }
                            break;
                        case MarkStyleEnum.wave:
                            for (int i = 0; i < 2; i++)
                            {
                                gc.DrawLine(controlPen,
                                    new Point((int)(charSizeX * 0.25 * 0), (int)(charSizeY * 0.85) + i),
                                    new Point((int)(charSizeX * 0.25 * 1), (int)(charSizeY * 0.8) + i)
                                    );
                                gc.DrawLine(controlPen,
                                    new Point((int)(charSizeX * 0.25 * 1), (int)(charSizeY * 0.8) + i),
                                    new Point((int)(charSizeX * 0.25 * 3), (int)(charSizeY * 0.9) + i)
                                    );
                                gc.DrawLine(controlPen,
                                    new Point((int)(charSizeX * 0.25 * 3), (int)(charSizeY * 0.9) + i),
                                    new Point((int)(charSizeX * 0.25 * 4), (int)(charSizeY * 0.85) + i)
                                    );
                            }
                            break;
                    }
                }

            }
            System.Diagnostics.Debug.Print("regen buffer "+sw.ElapsedMilliseconds.ToString()+"ms");
        }


        public enum MarkStyleEnum
        {
            underLine,
            wave
        }


        // draw image
        private int tabSize = 4;
        public Color selectedColor = Color.FromArgb(60, Color.FromArgb(0x5b,0x7d,0x9f));
        public Color SelectedColor {
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

        public Color lineNumberFillColor = Color.FromArgb(50, Color.SlateGray);
        public Color LineNumberFillColor
        {
            get
            {
                return lineNumberFillColor;
            }
            set
            {
                lineNumberFillColor = value;
            }
        }

        private SolidBrush lineNumberBrush
        {
            get
            {
                return new SolidBrush(lineNumberFillColor);
            }
        }

        private static IconImage plusIcon = new IconImage(Properties.Resources.plus);
        private static IconImage minusIcon = new IconImage(Properties.Resources.minus);
//        private static IconImage dotIcon = new IconImage(Properties.Resources.dot);

        private int xOffset = 0;
        private int caretX = 0;
        private int caretY = 0;
        private int[] actualLineNumbers = new int[] { };

        private void dbDrawBox_DoubleBufferedPaint(PaintEventArgs e)
        {
            System.Diagnostics.Debug.Print("dbDrawBox_DoubleBufferedPaint "+visibleLines.ToString());
            if (reGenarateBuffer)
            {
                createGraphicsBuffer();
                reGenarateBuffer = false;
            }
            if(actualLineNumbers.Length != visibleLines+2)
            {
                actualLineNumbers = new int[visibleLines+2];
            }

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            e.Graphics.Clear(BackColor);
            if (document == null) return;

            int x = 0;
            int lineX = 0;
            int y = 0;
            lock (document)
            {
                if (multiLine)
                {
                    int lines = document.Lines;
                    if (lines < 1000) lines = 1000;
                    xOffset = lines.ToString().Length + 2;
                }
                else
                {
                    xOffset = 0;
                }

                int lineStart = vScrollBar.Value+1;
                int drawLine = 0;
                int line = lineStart;
                if (!multiLine) drawLine = document.Lines;
                while(line < document.Lines)
                {
                    if (drawLine >= visibleLines + 2)
                    {
                        break;
                    }
                    if (!document.IsVisibleLine(line))
                    {
                        e.Graphics.DrawLine(new Pen(Color.FromArgb(50, Color.Black)), new Point(xOffset * charSizeX, y - 1), new Point(dbDrawBox.Width, y -1));
                        line++;
                        while (line < document.Lines)
                        {
                            if (document.IsVisibleLine(line)) break;
                            line++;
                        }
                        continue;
                    }
                    actualLineNumbers[drawLine] = line;

                    // draw line number (right padding)
                    x = (xOffset-3) * charSizeX;
                    if (multiLine)
                    {
                        if (document.IsBlockHeadLine(line))
                        {
                            if (document.IsCollapsed(line))
                            {
                                e.Graphics.DrawImage(plusIcon.GetImage(charSizeY, IconImage.ColorStyle.Blue), new Point(x, y));
                            }
                            else
                            {
                                e.Graphics.DrawImage(minusIcon.GetImage(charSizeY, IconImage.ColorStyle.Blue), new Point(x, y));
                            }
                        }
                        x = x - charSizeX;

                        string lineString = line.ToString();
                        for (int i = 0; i < lineString.Length; i++)
                        {
                            e.Graphics.DrawImage(charBitmap[0, lineString[lineString.Length - i - 1]], new Point(x, y));
                            x = x - charSizeX;
                        }
                    }

                    // draw charactors
                    x = xOffset*charSizeX;
                    lineX = 0;
                    int start = document.GetLineStartIndex(line);
                    int end = start+document.GetLineLength(line);
                    {
                        for (int i = start; i <= end; i++)
                        {
                            if (i == document.Length) continue;
                            char ch = document.GetCharAt(i);
                            byte color = document.GetColorAt(i);
                            int xIncrement = 1;
                            if (ch == '$')
                            {
                                string s = "";
                            }
                            if (ch == '\t')
                            {
                                xIncrement = tabSize - (lineX % tabSize);
                                e.Graphics.DrawLine(new Pen(Color.LightGray), new Point(x + 2, y + charSizeY - 2), new Point(x - 2 + xIncrement * charSizeX, y + charSizeY - 2));
                                e.Graphics.DrawLine(new Pen(Color.LightGray), new Point(x - 2 + xIncrement * charSizeX, y + charSizeY - 2), new Point(x - 2 + xIncrement * charSizeX, y + charSizeY - 8));
                            }
                            else if (ch == '\n')
                            {
                                if (i == 0 || document.GetCharAt(i - 1) != '\r')
                                {
                                    e.Graphics.DrawImage(charBitmap[color, ch], new Point(x, y));
                                }
                            }
                            else if (ch < 128)
                            {
                                e.Graphics.DrawImage(charBitmap[color, ch], new Point(x, y));
                            }
                            else
                            {
                                System.Windows.Forms.TextRenderer.DrawText(e.Graphics, ch.ToString(), Font, new Point(x, y), Color.Gray);
                            }

                            // caret
                            if (i == document.CaretIndex & Editable)
                            {
                                e.Graphics.DrawLine(new Pen(Color.Black), new Point(x, y + 2), new Point(x, y + charSizeY - 2));
                                e.Graphics.DrawLine(new Pen(Color.Black), new Point(x + 1, y + 2), new Point(x + 1, y + charSizeY - 2));
                                caretX = x;
                                caretY = y;
                            }

                            // mark
                            if (document.GetMarkAt(i) != 0)
                            {
                                for (int mark = 0; mark < 8; mark++)
                                {
                                    if ((document.GetMarkAt(i) & (1 << mark)) != 0)
                                    {
                                        e.Graphics.DrawImage(markBitmap[mark], new Point(x, y));
                                    }
                                }
                            }

                            // selection
                            if (i >= document.SelectionStart && i < document.SelectionLast)
                            {
                                if(ch == '\t')
                                {
                                    xIncrement = tabSize - (lineX % tabSize);
                                    e.Graphics.FillRectangle(selectionBrush, new Rectangle(x, y, xIncrement * charSizeX, charSizeY));
                                }
                                else
                                {
                                    e.Graphics.FillRectangle(selectionBrush, new Rectangle(x, y, charSizeX, charSizeY));
                                }
                            }

                            lineX = lineX + xIncrement;
                            x = x + charSizeX * xIncrement;
                        }
                    }
                    y = y + charSizeY;
                    drawLine++;
                    line++;
                }
                if (document.Length == document.CaretIndex & Editable)
                {
                    y = y - charSizeY;
                    e.Graphics.DrawLine(new Pen(Color.Black), new Point(x, y + 2), new Point(x, y + charSizeY - 2));
                    e.Graphics.DrawLine(new Pen(Color.Black), new Point(x + 1, y + 2), new Point(x + 1, y + charSizeY - 2));
                    caretX = x;
                    caretY = y;
                }
            }
            e.Graphics.FillRectangle(lineNumberBrush, new Rectangle(0, 0, charSizeX * (xOffset-1)+charSizeX/2, dbDrawBox.Height));

            if (Editable)
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(100,Color.Black)), new Point(xOffset*charSizeX, caretY + charSizeY), new Point(dbDrawBox.Width, caretY + charSizeY));
            }
            sw.Stop();
//            System.Diagnostics.Debug.Print("draw : "+sw.Elapsed.TotalMilliseconds.ToString()+ "ms");
        }

        public int GetActualLineNo(int drawLineNumber)
        {
            if(actualLineNumbers.Length < drawLineNumber) return actualLineNumbers.Last();
            return actualLineNumbers[drawLineNumber];
        }
        public int GetIndexAt(int x,int y)
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

        private int hitIndex(int x,int y)
        {
            //int line = y / charSizeY + vScrollBar.Value+1;
            int line = GetActualLineNo(y / charSizeY);
            if (line > document.Lines-1) line = document.Lines-1;
            if (line < 1) line = 1;
            int hitX = x / charSizeX - xOffset;

            int index = document.GetLineStartIndex(line);
            int nextLineIndex = index + document.GetLineLength(line);

            int xPos = 0;
            char ch = document.GetCharAt(index);
            while (index < nextLineIndex)
            {
                if (ch == '\r' || ch == '\n') break;
                if(ch == '\t')
                {
                    xPos = xPos + tabSize - (xPos % tabSize);
                }
                else
                {
                    xPos++;
                }
                if (xPos > hitX) break;
                index++;
                ch = document.GetCharAt(index);
            }

            if(index > 0 && document.GetCharAt(index) == '\n' && document.GetCharAt(index-1) == '\r')
            {
                index--;
            }
            if (index < 0) index = 0;
            
            return index;
        }

        public void SetSelection(int index,int length)
        {
            document.CaretIndex = index+length;
            document.SelectionStart = index;
            document.SelectionLast = index+length;
            selectionChanged();
            Invoke(new Action(dbDrawBox.Refresh));
            System.Diagnostics.Debug.Print("setSelection:" + document.SelectionStart.ToString() + "->" + document.SelectionLast.ToString());
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
            Invoke(new Action(dbDrawBox.Refresh));
        }
        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void dbDrawBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (document == null || !Editable) return;

            if (e.Button == MouseButtons.Left)
            {
                if (e.X < xOffset * charSizeX)
                {
                    int drawLine = e.Y / charSizeY+1;
                    int line = drawLine;
                    if (document.IsCollapsed(line))
                    {
                        document.ExpandBlock(line);
                    }
                    else
                    {
                        document.CollapseBlock(line);
                    }
                    UpdateVScrollBarRange();
                    Invoke(new Action(dbDrawBox.Refresh));
                    return;
                }

                int index = hitIndex(e.X, e.Y);
                if(document.CaretIndex != index || document.SelectionStart != index || document.SelectionLast != index)
                {
                    document.CaretIndex = index;
                    document.SelectionStart = index;
                    document.SelectionLast = index;
                    state = uiState.selecting;
                    selectionChanged();
                    Invoke(new Action(dbDrawBox.Refresh));
                }
                else
                {
                    state = uiState.selecting;
                }
            }
        }

        private void dbDrawBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (document == null || !Editable) return;

            if (state == uiState.selecting)
            {
                int index = hitIndex(e.X, e.Y);
                if (document.SelectionStart == document.SelectionLast)
                {
                    if(document.SelectionStart < index)
                    {
                        document.SelectionLast = index;
                    }
                    else
                    {
                        document.SelectionStart = index;
                    }
                }
                else if (document.SelectionStart == document.CaretIndex)
                {
                    document.SelectionStart = index;
                }
                else if(document.SelectionLast == document.CaretIndex)
                {
                    document.SelectionLast = index;
                }
                document.CaretIndex = index;
                selectionChanged();

                Invoke(new Action(dbDrawBox.Refresh));
            }
            this.OnMouseMove(e);
        }

        private void dbDrawBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (document == null || !Editable) return;

            if (e.Button == MouseButtons.Left)
            {
                if (state == uiState.selecting)
                {
                    int index = hitIndex(e.X, e.Y);
                    if(document.CaretIndex != index || document.SelectionLast != index)
                    {
                        document.CaretIndex = index;
                        document.SelectionLast = index;
                        state = uiState.idle;
                        selectionChanged();
                        Invoke(new Action(dbDrawBox.Refresh));
                    }
                    else
                    {
                        state = uiState.idle;
                    }
                }
            }
        }

        private void dbDrawBox_MouseLeave(object sender, EventArgs e)
        {
            if (document == null || !Editable) return;

            if (state == uiState.selecting)
            {
                document.CaretIndex = document.SelectionStart;
                document.SelectionLast = document.SelectionStart;
                state = uiState.idle;
                selectionChanged();
                Invoke(new Action(dbDrawBox.Refresh));
            }
            this.OnMouseLeave(e);
        }

        private void dbDrawBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (document == null || !Editable) return;

            if (state == uiState.selecting)
            {
                if(document.CaretIndex != document.SelectionStart || document.SelectionLast != document.SelectionStart)
                {
                    document.CaretIndex = document.SelectionStart;
                    document.SelectionLast = document.SelectionStart;
                    state = uiState.idle;
                    selectionChanged();
                    Invoke(new Action(dbDrawBox.Refresh));
                }
                else
                {
                    state = uiState.idle;
                }
            }
            this.OnMouseDoubleClick(e);
        }

        public void ScrollToCaret()
        {
            scrollToCaret();
            selectionChanged();
            Invoke(new Action(dbDrawBox.Refresh));
        }

        private bool skipKeyPress = false;
        private void dbDrawBox_KeyDown(object sender, KeyEventArgs e)
        {
            skipKeyPress = false;
            if (document == null || !Editable) return;

            int lineBeforeEdit = document.GetLineAt(document.CaretIndex);
            if (BeforeKeyDown != null) BeforeKeyDown(this, e);
            if (e.Handled)
            {
                skipKeyPress = true;
                scrollToCaret();
                selectionChanged();
                Invoke(new Action(dbDrawBox.Refresh));
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.C:
                    if (e.Modifiers == Keys.Control)
                    {
                        Copy();
                        scrollToCaret();
                        selectionChanged();
                        if (CarletLineChanged != null) CarletLineChanged(this, EventArgs.Empty);
                        e.Handled = true;
                    }
                    break;
                case Keys.X:
                    if (e.Modifiers == Keys.Control)
                    {
                        Cut();
                        scrollToCaret();
                        selectionChanged();
                        e.Handled = true;
                    }
                    break;
                case Keys.V:
                    if (e.Modifiers == Keys.Control)
                    {
                        Paste();
                        scrollToCaret();
                        selectionChanged();
                        e.Handled = true;
                    }
                    break;
                case Keys.Z:
                    if(e.Modifiers == Keys.Control)
                    {
                        Undo();
                        e.Handled = true;
                    }
                    break;
                case Keys.Left:
                    {
                        if (document.CaretIndex < 1) break;
                        bool onSelectionLast = false;
                        if(document.SelectionLast == document.CaretIndex && document.SelectionStart != document.SelectionLast)
                        {
                            onSelectionLast = true;
                        }

                        if (document.CaretIndex > 0 && document.GetCharAt(document.CaretIndex) == '\n' && document.GetCharAt(document.CaretIndex - 1) == '\r')
                        {
                            document.CaretIndex = document.CaretIndex - 2;
                        }
                        else
                        {
                            document.CaretIndex--;
                        }
                        caretChanged();
                        if (e.Modifiers == Keys.Shift)
                        {
                            if (onSelectionLast)
                            {
                                document.SelectionLast = document.CaretIndex;
                            }
                            else
                            {
                                document.SelectionStart = document.CaretIndex;
                            }
                        }
                        else
                        {
                            document.SelectionStart = document.CaretIndex;
                            document.SelectionLast = document.CaretIndex;
                        }
                        selectionChanged();
                        Invoke(new Action(dbDrawBox.Refresh));
                        e.Handled = true;
                    }
                    break;
                case Keys.Right:
                    {
                        if (document.CaretIndex >= document.Length) break;
                        bool onSelectionStart = false;
                        if (document.SelectionStart == document.CaretIndex && document.SelectionStart != document.SelectionLast)
                        {
                            onSelectionStart = true;
                        }
                        if (document.CaretIndex != document.Length - 1 && document.GetCharAt(document.CaretIndex) == '\r' && document.GetCharAt(document.CaretIndex + 1) == '\n')
                        {
                            document.CaretIndex = document.CaretIndex + 2;
                        }
                        else
                        {
                            document.CaretIndex++;
                        }
                        caretChanged();
                        if (e.Modifiers == Keys.Shift)
                        {
                            if (onSelectionStart)
                            {
                                document.SelectionStart = document.CaretIndex;
                            }
                            else
                            {
                                document.SelectionLast = document.CaretIndex;
                            }
                        }
                        else
                        {
                            document.SelectionStart = document.CaretIndex;
                            document.SelectionLast = document.CaretIndex;
                            selectionChanged();
                        }
                        e.Handled = true;
                    }
                    break;
                case Keys.Up:
                    {
                        if (!multiLine) break;
                        bool onSelectionLast = false;
                        if (document.SelectionLast == document.CaretIndex && document.SelectionStart != document.SelectionLast)
                        {
                            onSelectionLast = true;
                        }
                        int line = document.GetLineAt(document.CaretIndex);
                        if (line == 1) break;
                        int headindex = document.GetLineStartIndex(line);
                        int xPosition = document.CaretIndex - headindex;
                        line--;

                        // skip invisible lines
                        while (line > 1 && !document.IsVisibleLine(line))
                        {
                            line--;
                        }
                        if(!document.IsVisibleLine(line)) line = document.GetLineAt(document.CaretIndex);

                        headindex = document.GetLineStartIndex(line);
                        int lineLength = document.GetLineLength(line);
                        if (lineLength < xPosition)
                        {
                            document.CaretIndex = headindex + lineLength-1;
                        }
                        else
                        {
                            document.CaretIndex = headindex + xPosition;
                        }
                        caretChanged();
                        if (e.Modifiers == Keys.Shift)
                        {
                            if(onSelectionLast)
                            {
                                document.SelectionLast = document.CaretIndex;
                            }
                            else
                            {
                                document.SelectionStart = document.CaretIndex;
                            }
                        }
                        else
                        {
                            document.SelectionStart = document.CaretIndex;
                            document.SelectionLast = document.CaretIndex;
                        }
                        scrollToCaret();
                        selectionChanged();
                        e.Handled = true;
                    }
                    break;
                case Keys.Down:
                    {
                        if (!multiLine) break;
                        bool onSelectionStart = false;
                        if (document.SelectionStart == document.CaretIndex && document.SelectionStart != document.SelectionLast)
                        {
                            onSelectionStart = true;
                        }
                        int line = document.GetLineAt(document.CaretIndex);
                        if (line == document.Lines-1) break;
                        int headindex = document.GetLineStartIndex(line);
                        int xPosition = document.CaretIndex - headindex;
                        line++;

                        // skip invisible lines
                        while (line < document.Lines && !document.IsVisibleLine(line))
                        {
                            line++;
                        }
                        if (!document.IsVisibleLine(line)) line = document.GetLineAt(document.CaretIndex);

                        headindex = document.GetLineStartIndex(line);
                        int lineLength = document.GetLineLength(line);
                        if (lineLength < xPosition)
                        {
                            document.CaretIndex = headindex + lineLength-1;
                        }
                        else
                        {
                            document.CaretIndex = headindex + xPosition;
                        }
                        caretChanged();
                        if (e.Modifiers == Keys.Shift)
                        {
                            if (onSelectionStart)
                            {
                                document.SelectionStart = document.CaretIndex;
                            }
                            else
                            {
                                document.SelectionLast = document.CaretIndex;
                            }
                        }
                        else
                        {
                            document.SelectionStart = document.CaretIndex;
                            document.SelectionLast = document.CaretIndex;
                        }
                        scrollToCaret();
                        selectionChanged();
                        e.Handled = true;
                    }
                    break;
                case Keys.Delete:
                    if(document.SelectionStart == document.SelectionLast)
                    {
                        if (document.CaretIndex == document.Length) break;
                        if(document.CaretIndex != document.Length - 1 && document.GetCharAt(document.CaretIndex) == '\r' && document.GetCharAt(document.CaretIndex+1) == '\n')
                        {
                            document.Replace(document.CaretIndex, 2, 0, "");
                        }else
                        {
                            document.Replace(document.CaretIndex, 1, 0, "");
                        }
                    }
                    else
                    {
                        document.Replace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, "");
                    }
                    UpdateVScrollBarRange();
                    caretChanged();
                    selectionChanged();
                    e.Handled = true;
                    break;
                case Keys.Back:
                    if (document.SelectionStart == document.SelectionLast)
                    {
                        if (document.CaretIndex == 0) break;
                        if (document.CaretIndex > 1 && document.GetCharAt(document.CaretIndex-2) == '\r' && document.GetCharAt(document.CaretIndex-1) == '\n')
                        {
                            document.Replace(document.CaretIndex-2, 2, 0, "");
                        }
                        else
                        {
                            document.Replace(document.CaretIndex - 1, 1, 0, "");
                        }
                    }
                    else
                    {
                        document.Replace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, "");
                    }
                    UpdateVScrollBarRange();
                    caretChanged();
                    selectionChanged();
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    if (!multiLine) break;
                    if (e.Modifiers == Keys.Shift)
                    {
                        if (document.SelectionStart == document.SelectionLast)
                        {
                            document.Replace(document.CaretIndex, 0, 0, "\n");
                        }
                        else
                        {
                            document.Replace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, "\n");
                        }
                        if (document.CaretIndex >= document.SelectionStart) document.SelectionStart = document.SelectionStart + 1;
                        if (document.CaretIndex >= document.SelectionLast) document.SelectionStart = document.SelectionLast + 1;
                        document.CaretIndex = document.CaretIndex + 1;
                    }
                    else
                    {
                        if (document.SelectionStart == document.SelectionLast)
                        {
                            document.Replace(document.CaretIndex, 0, 0, "\r\n");
                        }
                        else
                        {
                            document.Replace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, "\r\n");
                        }
                        if (document.CaretIndex >= document.SelectionStart) document.SelectionStart = document.SelectionStart + 2;
                        if (document.CaretIndex >= document.SelectionLast) document.SelectionStart = document.SelectionLast + 2;
                        document.CaretIndex = document.CaretIndex + 2;
                    }
                    document.SelectionLast = document.SelectionStart;
                    UpdateVScrollBarRange();
                    caretChanged();
                    scrollToCaret();
                    selectionChanged();
                    e.Handled = true;
                    break;
                case Keys.Tab:
                    // multiple lines indent
                    if (document.SelectionStart == document.SelectionLast) break;
                    int lineStart = document.GetLineAt(document.SelectionStart);
                    int lineLast = document.GetLineAt(document.SelectionLast);
                    if (lineStart == lineLast) break;
                    if (e.Modifiers == Keys.Shift)
                    {
                        for (int i = lineStart; i < lineLast; i++)
                        {
                            if(document.GetCharAt(document.GetLineStartIndex(i)) == '\t' || document.GetCharAt(document.GetLineStartIndex(i)) == ' ')
                            {
                                document.Replace(document.GetLineStartIndex(i), 1, 0, "");
                            }
                        }
                        document.SelectionStart = document.GetLineStartIndex(lineStart);
                        document.SelectionLast = document.GetLineStartIndex(lineLast);
                    }
                    else
                    {
                        for(int i = lineStart; i < lineLast; i++)
                        {
                            document.Replace(document.GetLineStartIndex(i), 0, 0, "\t");
                        }
                        document.SelectionStart = document.GetLineStartIndex(lineStart);
                        document.SelectionLast = document.GetLineStartIndex(lineLast);
                    }
                    UpdateVScrollBarRange();
                    caretChanged();
                    scrollToCaret();
                    selectionChanged();
                    e.Handled = true;
                    break;
                default:
                    break;
            }

            if (AfterKeyDown != null) AfterKeyDown(this, e);
            int lineAfterEdit = document.GetLineAt(document.CaretIndex);
            if(lineBeforeEdit != lineAfterEdit)
            {
                if (CarletLineChanged != null) CarletLineChanged(this, EventArgs.Empty);
            }

            if (e.Handled)
            {
                Invoke(new Action(dbDrawBox.Refresh));
                skipKeyPress = true;
            }
        }

        private void dbDrawBox_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void dbDrawBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (document == null || !Editable) return;
            if (skipKeyPress)
            {
                skipKeyPress = false;
                e.Handled = true;
                return;
            }
            if (BeforeKeyPressed != null) BeforeKeyPressed(this, e);
            if (e.Handled)
            {
                Invoke(new Action(dbDrawBox.Refresh));
                return;
            }

            char inChar = e.KeyChar;
            int prevIndex = document.CaretIndex;
            if(document.GetLineStartIndex(document.GetLineAt(prevIndex)) != prevIndex && prevIndex != 0)
            {
                prevIndex--;
            }

            if((inChar < 127 && inChar >= 0x20) || inChar == '\t' || inChar > 0xff)
            {
                if(document.SelectionStart == document.SelectionLast)
                {
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                    if(inChar == '\t' || inChar == ' ')
                    {   // use default color for tab and space
                        document.Replace(document.CaretIndex, 0, 0, inChar.ToString());
                    }
                    else
                    {
                        document.Replace(document.CaretIndex, 0, document.GetColorAt(prevIndex), inChar.ToString());
                    }
                    document.CaretIndex++;
                    sw.Stop();
                    UpdateVScrollBarRange();
//                    System.Diagnostics.Debug.Print("edit : " + sw.Elapsed.TotalMilliseconds.ToString()+"ms");
                }
                else
                {
                    document.Replace(document.SelectionStart, document.SelectionLast-document.SelectionStart, document.GetColorAt(prevIndex), inChar.ToString());
                    document.CaretIndex = document.SelectionStart + 1;
                    document.SelectionStart = document.CaretIndex;
                    document.SelectionLast = document.CaretIndex;
                    UpdateVScrollBarRange();
                }
                if (AfterKeyPressed != null) AfterKeyPressed(this, e);
                Invoke(new Action(dbDrawBox.Refresh));
            }
        }


        private void caretChanged()
        {
            if (document == null) return;
            if (document.CaretIndex == 0) return;

            // move caret on CRLF to CR
            if(document.GetCharAt(document.CaretIndex) == '\n' && document.GetCharAt(document.CaretIndex-1) == '\r')
            {
                document.CaretIndex--;
            }
        }

        private void selectionChanged()
        {
            if (document == null) return;
            System.Diagnostics.Debug.Print("to changed:" + document.SelectionStart.ToString() + "->" + document.SelectionLast.ToString());

            if (document.SelectionStart > document.SelectionLast)
            {
                int last = document.SelectionLast;
                document.SelectionLast = document.SelectionStart;
                document.SelectionStart = last;
            }
            if (document.SelectionStart > document.Length) document.SelectionStart = document.Length;
            if (document.SelectionLast > document.Length) document.SelectionLast = document.Length;
            System.Diagnostics.Debug.Print("changed:" + document.SelectionStart.ToString() + "->" + document.SelectionLast.ToString());
        }

        /// <summary>
        /// move vscrollbar to carlet visible position
        /// </summary>
        private void scrollToCaret()
        {
            if (document == null) return;

            int line = document.GetVisibleLine(document.GetLineAt(document.CaretIndex));

            if (line < vScrollBar.Value)
            {
                vScrollBar.Value = line;
            }
            else if (line >= visibleLines + vScrollBar.Value)
            {
                int v = line - visibleLines;
                if (v < vScrollBar.Minimum)
                {
                    vScrollBar.Value = vScrollBar.Minimum;
                }else if (v > vScrollBar.Maximum)
                {
                    vScrollBar.Value = vScrollBar.Maximum;
                }
                else
                {
                    if (v + 1 < vScrollBar.Maximum)
                    {
                        vScrollBar.Value = v+1; // to skip half visible line 
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

    }
}
