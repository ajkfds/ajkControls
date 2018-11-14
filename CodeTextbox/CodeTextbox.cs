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
    public partial class CodeTextbox : UserControl
    {
        public CodeTextbox()
        {
            InitializeComponent();
            dbDrawBox.SetImeEnable(true);
            resizeCharSize();
            this.dbDrawBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.dbDrawBox_MouseWheel);
        }

        public event EventHandler CarletLineChanged;
        public event KeyPressEventHandler AfterKeyPressed;
        public event KeyEventHandler AfterKeyDown;


        private float size = 8;
        private void dbDrawBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (e.Delta < 0) size--;
                if (e.Delta > 0) size++;
                if (size < 5) size = 5;
                if (size > 20) size = 20;
                this.Font = new Font(this.Font.FontFamily, size);
                resizeCharSize();
                reGenarateBuffer = true;
                Invoke(new Action(dbDrawBox.Refresh));
            }
            else
            {
                int value = vScrollBar.Value - e.Delta * SystemInformation.MouseWheelScrollLines / 4;
                if (value < vScrollBar.Minimum) value = vScrollBar.Minimum;
                if (value > vScrollBar.Maximum - vScrollBar.LargeChange) value = vScrollBar.Maximum - vScrollBar.LargeChange;
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
                if(multiLine == false)
                {
                    if (ScrollBarVisible) ScrollBarVisible = false;
                    resizeCharSize();
                    Height = charSizeY;
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
                reGenarateBuffer = true;
            }
        }

        //
        public override Color BackColor {
            get { return base.BackColor; }
            set
            {
                if (base.BackColor != value) // avoid infinite loop on VS designer
                {
                    base.BackColor = value;
                    reGenarateBuffer = true;
                }
            }
        }

        private void CodeTextbox_Resize(object sender, EventArgs e)
        {
            resizeCharSize();
        }

        int charSizeX = 0;
        int charSizeY = 0;
        int visibleLines = 10;
        private void resizeCharSize()
        {
            Graphics g = dbDrawBox.CreateGraphics();
            Size fontSize = System.Windows.Forms.TextRenderer.MeasureText(g, "A", Font, new Size(100, 100), TextFormatFlags.NoPadding);
            charSizeX = fontSize.Width;
            charSizeY = fontSize.Height;

            visibleLines = (int)Math.Ceiling( (double)(dbDrawBox.Height / charSizeY) );
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
            for (int color = 0; color < 16; color++)
            {
                for (int i = 0; i < 128; i++)
                {
                    if(charBitmap[color, i] != null)
                    {
                        charBitmap[color, i].Dispose();
                    }
                    charBitmap[color, i] = new Bitmap(charSizeX, charSizeY);
                    using (Graphics gc = Graphics.FromImage(charBitmap[color, i]))
                    {
                        gc.Clear(BackColor);
                        gc.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        if (i < 0x20)
                        { // control codes
                            Color controlColor = Color.DarkGray;
                            Pen controlPen = new Pen(controlColor);
                            switch (i)
                            {
                                case '\r':
                                    gc.DrawLine(controlPen, new Point((int)(charSizeX * 0.9), (int)(charSizeY * 0.2)), new Point((int)(charSizeX * 0.9), (int)(charSizeY * 0.6)));

                                    gc.DrawLine(controlPen, new Point((int)(charSizeX * 0.2), (int)(charSizeY * 0.6)), new Point((int)(charSizeX * 0.9), (int)(charSizeY * 0.6)));
                                    gc.DrawLine(controlPen, new Point((int)(charSizeX * 0.4), (int)(charSizeY * 0.4)), new Point((int)(charSizeX * 0.2), (int)(charSizeY * 0.6)));
                                    gc.DrawLine(controlPen, new Point((int)(charSizeX * 0.4), (int)(charSizeY * 0.8)), new Point((int)(charSizeX * 0.2), (int)(charSizeY * 0.6)));
                                    break;
                                case '\n':
                                    gc.DrawLine(controlPen, new Point((int)(charSizeX * 0.6), (int)(charSizeY * 0.2)), new Point((int)(charSizeX * 0.6), (int)(charSizeY * 0.8)));
                                    gc.DrawLine(controlPen, new Point((int)(charSizeX * 0.4), (int)(charSizeY * 0.6)), new Point((int)(charSizeX * 0.6), (int)(charSizeY * 0.8)));
                                    gc.DrawLine(controlPen, new Point((int)(charSizeX * 0.8), (int)(charSizeY * 0.6)), new Point((int)(charSizeX * 0.6), (int)(charSizeY * 0.8)));
                                    break;
                                default:
                                    gc.DrawRectangle(new Pen(Color.DarkGray), new Rectangle(1, 1 + 2, charSizeX - 2, charSizeY - 2 - 2));
                                    break;
                            }

                        }
                        else
                        {
                            if (i == 0x26)
                            {
                                System.Windows.Forms.TextRenderer.DrawText(gc, "&&", Font, new Point(0, 0), Style.ColorPallet[color], BackColor, TextFormatFlags.NoPadding);
                            }
                            else
                            {
                                System.Windows.Forms.TextRenderer.DrawText(gc, ((char)i).ToString(), Font, new Point(0, 0), Style.ColorPallet[color], BackColor, TextFormatFlags.NoPadding);
                            }
                        }
                    }
                }
            }
            for (int mark = 0; mark < 8; mark++)
            {
                if (markBitmap[mark] != null) markBitmap[mark].Dispose();
                markBitmap[mark] = new Bitmap(charSizeX, charSizeY);
                using (Graphics gc = Graphics.FromImage(markBitmap[mark]))
                {
                    gc.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    Pen controlPen = new Pen(Style.MarkColor[mark]);
                    switch (Style.MarkStyle[mark])
                    {
                        case MarkStyleEnum.underLine:
                            for (int i = 0; i < 2; i++)
                            {
                                gc.DrawLine(controlPen,
                                new Point(0, (int)(charSizeY * 0.8)+i),
                                new Point((int)charSizeX, (int)(charSizeY * 0.8)+i)
                                );
                            }
                            break;
                        case MarkStyleEnum.wave:
                            for(int i = 0; i < 2; i++)
                            {
                                gc.DrawLine(controlPen,
                                    new Point((int)(charSizeX * 0.25 * 0), (int)(charSizeY * 0.85)+i),
                                    new Point((int)(charSizeX * 0.25 * 1), (int)(charSizeY * 0.8)+i)
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
        }

        public enum MarkStyleEnum
        {
            underLine,
            wave
        }


        // draw image
        private int tabSize = 4;
        private SolidBrush selectionBrush = new SolidBrush(Color.FromArgb(100, Color.Turquoise));
        private SolidBrush lineNumberBrush = new SolidBrush(Color.FromArgb(50, Color.SlateGray));

        private int xOffset = 0;
        private void dbDrawBox_DoubleBufferedPaint(PaintEventArgs e)
        {
            if (reGenarateBuffer)
            {
                createGraphicsBuffer();
                reGenarateBuffer = false;
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
                xOffset = document.Length.ToString().Length + 1;
                int line = vScrollBar.Value;
                if (!multiLine) line = document.Lines - 1;
                while(line < document.Lines)
                {
                    // draw line number (right padding)
                    x = (xOffset-2) * charSizeX;
                    string lineString = line.ToString();
                    for(int i = 0; i < lineString.Length; i++)
                    {
                        e.Graphics.DrawImage(charBitmap[0, lineString[lineString.Length-i-1]], new Point(x, y));
                        x = x - charSizeX;
                    }

                    // draw charactors
                    x = xOffset*charSizeX;
                    lineX = 0;
                    int start = document.GetLineStartIndex(line);
                    int end = start+document.GetLineLength(line);
                    if (start == end)
                    { // blank line (for last line)
                        // caret
                        if (start == document.CaretIndex & Editable)
                        {
                            e.Graphics.DrawLine(new Pen(Color.Black), new Point(x, y + 2), new Point(x, y + charSizeY - 2));
                            e.Graphics.DrawLine(new Pen(Color.Black), new Point(x + 1, y + 2), new Point(x + 1, y + charSizeY - 2));
                        }
                    }
                    else
                    {
                        for (int i = start; i < end; i++)
                        {
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
                                if (x == 0 || document.GetCharAt(i - 1) != '\r')
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
                                e.Graphics.FillRectangle(selectionBrush, new Rectangle(x, y, charSizeX, charSizeY));
                            }

                            lineX = lineX + xIncrement;
                            x = x + charSizeX * xIncrement;
                        }
                    }
                    y = y + charSizeY;
                    line++;
                }
            }
            e.Graphics.FillRectangle(lineNumberBrush, new Rectangle(0, 0, charSizeX * xOffset, dbDrawBox.Height));

            sw.Stop();
            System.Diagnostics.Debug.Print("draw : "+sw.Elapsed.TotalMilliseconds.ToString()+ "ms");
        }

        public int GetIndexAt(int x,int y)
        {
            return hitIndex(x, y);
        }

        private int hitIndex(int x,int y)
        {
            int line = y / charSizeY + vScrollBar.Value;
            if (line > document.Lines-1) line = document.Lines-1;
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
                    xPos = xPos + tabSize;
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
                document.CaretIndex = index;
                if(document.SelectionStart > index)
                {
                    document.SelectionLast = document.SelectionStart;
                    document.SelectionStart = index;
                }
                else
                {
                    document.SelectionLast = index;
                }
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

        public void Undo()
        {
            document.Undo();
            scrollToCaret();
            selectionChanged();
            Invoke(new Action(dbDrawBox.Refresh));
        }

        public void ScrollToCaret()
        {
            scrollToCaret();
            selectionChanged();
            Invoke(new Action(dbDrawBox.Refresh));
        }

        private void dbDrawBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (document == null || !Editable) return;

            switch (e.KeyCode)
            {
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
                            document.SelectionStart = document.CaretIndex;
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
                            document.SelectionLast = document.CaretIndex;
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
                case Keys.Up:
                    {
                        if (!multiLine) break;
                        int line = document.GetLineAt(document.CaretIndex);
                        if (line == 0) break;
                        int headindex = document.GetLineStartIndex(line);
                        int xPosition = document.CaretIndex - headindex;
                        line--;

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
                            document.SelectionStart = document.CaretIndex;
                        }
                        else
                        {
                            document.SelectionStart = document.CaretIndex;
                            document.SelectionLast = document.CaretIndex;
                        }
                        scrollToCaret();
                        selectionChanged();
                        if (CarletLineChanged != null) CarletLineChanged(this, EventArgs.Empty);
                        Invoke(new Action(dbDrawBox.Refresh));
                        e.Handled = true;
                    }
                    break;
                case Keys.Down:
                    {
                        if (!multiLine) break;
                        int line = document.GetLineAt(document.CaretIndex);
                        if (line == document.Lines-1) break;
                        int headindex = document.GetLineStartIndex(line);
                        int xPosition = document.CaretIndex - headindex;
                        line++;

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
                            document.SelectionLast = document.CaretIndex;
                        }
                        else
                        {
                            document.SelectionStart = document.CaretIndex;
                            document.SelectionLast = document.CaretIndex;
                        }
                        scrollToCaret();
                        selectionChanged();
                        if (CarletLineChanged != null) CarletLineChanged(this, EventArgs.Empty);
                        Invoke(new Action(dbDrawBox.Refresh));
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
                    Invoke(new Action(dbDrawBox.Refresh));
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
                    Invoke(new Action(dbDrawBox.Refresh));
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
                    if (CarletLineChanged != null) CarletLineChanged(this, EventArgs.Empty);
                    Invoke(new Action(dbDrawBox.Refresh));
                    e.Handled = true;
                    break;
                case Keys.Tab:
                    if (document.SelectionStart == document.SelectionLast) break;
                    int lineStart = document.GetLineAt(document.SelectionStart);
                    int lineLast = document.GetLineAt(document.SelectionLast);
                    if (lineStart == lineLast) break;
                    if (e.Modifiers == Keys.Shift)
                    {
                        for (int i = lineStart; i < lineLast; i++)
                        {
                            if(document.GetCharAt(document.GetLineStartIndex(i)) == '\t')
                            {
                                document.Replace(document.GetLineStartIndex(i), 1, 0, "");
                            }
                        }
                    }
                    else
                    {
                        for(int i = lineStart; i < lineLast; i++)
                        {
                            document.Replace(document.GetLineStartIndex(i), 0, 0, "\t");
                        }
                    }
                    UpdateVScrollBarRange();
                    caretChanged();
                    scrollToCaret();
                    selectionChanged();
                    Invoke(new Action(dbDrawBox.Refresh));
                    e.Handled = true;
                    break;
                default:
                    break;
            }
            if (AfterKeyDown != null) AfterKeyDown(this, e);
        }

        private void dbDrawBox_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void dbDrawBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (document == null || !Editable) return;
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
                    document.Replace(document.CaretIndex, 0, document.GetColorAt(prevIndex), inChar.ToString());
                    document.CaretIndex++;
                    sw.Stop();
                    UpdateVScrollBarRange();
                    System.Diagnostics.Debug.Print("edit : " + sw.Elapsed.TotalMilliseconds.ToString()+"ms");
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

            if(document.SelectionStart > document.SelectionLast)
            {
                int last = document.SelectionLast;
                document.SelectionLast = document.SelectionStart;
                document.SelectionStart = last;
            }
            if (document.SelectionStart > document.Length) document.SelectionStart = document.Length;
            if (document.SelectionLast > document.Length) document.SelectionLast = document.Length;
        }

        /// <summary>
        /// move vscrollbar to carlet visible position
        /// </summary>
        private void scrollToCaret()
        {
            if (document == null) return;

            int line = document.GetLineAt(document.CaretIndex);

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

            int startLine = document.GetLineAt(document.SelectionStart);
            int lastLine = document.GetLineAt(document.SelectionLast);

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
            if (document != null) vScrollBar.Maximum = document.Lines;
        }

    }
}
