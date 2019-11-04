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
        int charSizeX = 0;
        int charSizeY = 0;
        int visibleLines = 10;
        private void resizeCharSize()
        {
//            System.Diagnostics.Debug.Print("risezicharsize");
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
//            System.Diagnostics.Debug.Print("risezicharsize visibleLines " + visibleLines.ToString());
            vScrollBar.LargeChange = visibleLines;
            UpdateVScrollBarRange();
        }


        // character & mark graphics buffer
        volatile bool reGenarateBuffer = true;
        Bitmap[,] charBitmap = new Bitmap[16, 128];
        Bitmap[] markBitmap = new Bitmap[8];

        private void createGraphicsBuffer()
        {
            unsafe
            {
//                System.Diagnostics.Debug.Print("regen buffer");
//                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
//                sw.Start();

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
                            case MarkStyleEnum.fill:
                                gc.FillRectangle(new SolidBrush(Style.MarkColor[mark]), 0, 0, charSizeX, charSizeY);
                                break;
                        }
                    }

                }
//                System.Diagnostics.Debug.Print("regen buffer " + sw.ElapsedMilliseconds.ToString() + "ms");
            }
        }


        public enum MarkStyleEnum
        {
            underLine,
            wave,
            fill
        }

        private void dbDrawBox_DoubleBufferedPaint(PaintEventArgs e)
        {
            unsafe
            {

//                System.Diagnostics.Debug.Print("dbDrawBox_DoubleBufferedPaint " + visibleLines.ToString());

                if (reGenarateBuffer)
                {
                    createGraphicsBuffer();
                    reGenarateBuffer = false;
                }
                if (actualLineNumbers.Length != visibleLines + 2)
                {
                    actualLineNumbers = new int[visibleLines + 2];
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

                    int lineStart = document.GetActialLineNo(vScrollBar.Value + 1);
//                    System.Diagnostics.Debug.Print("ls" + lineStart.ToString());
                    int drawLine = 0;
                    int line = lineStart;
                    if (!multiLine) drawLine = document.Lines;
                    while (line <= document.Lines)
                    {
                        if (drawLine >= visibleLines + 2)
                        {
                            break;
                        }
                        if (!document.IsVisibleLine(line))
                        {
                            e.Graphics.DrawLine(new Pen(Color.FromArgb(50, Color.Black)), new Point(xOffset * charSizeX, y - 1), new Point(dbDrawBox.Width, y - 1));
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
                        x = (xOffset - 3) * charSizeX;
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
                        x = xOffset * charSizeX;
                        lineX = 0;
                        int start = document.GetLineStartIndex(line);
                        int end = start + document.GetLineLength(line);
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
                                    for (int mark = 0; mark < 7; mark++)
                                    {
                                        if ((document.GetMarkAt(i) & (1 << mark)) != 0)
                                        {
                                            e.Graphics.DrawImage(markBitmap[mark], new Point(x, y));
                                        }
                                    }
                                    if ((document.GetMarkAt(i) & (1 << 7)) != 0)
                                    {
                                        e.Graphics.FillRectangle(new SolidBrush(Style.MarkColor[7]), x, y, charSizeX, charSizeY);
                                    }
                                }

                                // selection
                                if (i >= document.SelectionStart && i < document.SelectionLast)
                                {
                                    if (ch == '\t')
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

                e.Graphics.FillRectangle(lineNumberBrush, new Rectangle(0, 0, charSizeX * (xOffset - 1) + charSizeX / 2, dbDrawBox.Height));

                if (Editable)
                {
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(100, Color.Black)), new Point(xOffset * charSizeX, caretY + charSizeY), new Point(dbDrawBox.Width, caretY + charSizeY));
                }
                sw.Stop();
                //            System.Diagnostics.Debug.Print("draw : "+sw.Elapsed.TotalMilliseconds.ToString()+ "ms");
            }
        }

        private int hitIndex(int x, int y)
        {
            //int line = y / charSizeY + vScrollBar.Value+1;
            if (y < 0) return 0;
            int line = GetActualLineNo(y / charSizeY);
            if (line > document.Lines - 1) line = document.Lines - 1;
            if (line < 1) line = 1;
            int hitX = x / charSizeX - xOffset;

            int index = document.GetLineStartIndex(line);
            int nextLineIndex = index + document.GetLineLength(line);

            int xPos = 0;
            char ch = document.GetCharAt(index);
            while (index < nextLineIndex)
            {
                if (ch == '\r' || ch == '\n') break;
                if (ch == '\t')
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

            if (index > 0 && document.GetCharAt(index) == '\n' && document.GetCharAt(index - 1) == '\r')
            {
                index--;
            }
            if (index < 0) index = 0;

            return index;
        }

        public int getX(int targetIndex)
        {
            int line = document.GetLineAt(targetIndex);
            int index = document.GetLineStartIndex(line);
            int nextLineIndex = index + document.GetLineLength(line);

            int xPos = 0;
            char ch = document.GetCharAt(index);
            while (index < targetIndex)
            {
                if (ch == '\r' || ch == '\n') break;
                if (ch == '\t')
                {
                    xPos = xPos + tabSize - (xPos % tabSize);
                }
                else
                {
                    xPos++;
                }
                index++;
                ch = document.GetCharAt(index);
            }

            if (index > 0 && document.GetCharAt(index) == '\n' && document.GetCharAt(index - 1) == '\r')
            {
                index--;
            }
            if (index < 0) index = 0;

            return index;
        }

        private int getXPos(int index, int line)
        {
            int i = document.GetLineStartIndex(line);
            int xPos = 0;
            char ch = document.GetCharAt(i);
            while (i < index)
            {
                if (ch == '\r' || ch == '\n') break;
                if (ch == '\t')
                {
                    xPos = xPos + tabSize - (xPos % tabSize);
                }
                else
                {
                    xPos++;
                }
                i++;
                ch = document.GetCharAt(i);
            }
            return xPos;
        }

        private int getIndex(int xPos, int line)
        {
            int index = document.GetLineStartIndex(line);
            int nextLineIndex = index + document.GetLineLength(line);

            int x = 0;
            char ch = document.GetCharAt(index);
            while (index < nextLineIndex && x < xPos)
            {
                if (ch == '\r' || ch == '\n') break;
                if (ch == '\t')
                {
                    x = x + tabSize - (x % tabSize);
                }
                else
                {
                    x++;
                }
                index++;
                ch = document.GetCharAt(index);
            }

            if (index > 0 && document.GetCharAt(index) == '\n' && document.GetCharAt(index - 1) == '\r')
            {
                index--;
            }
            if (index < 0) index = 0;

            return index;
        }

    }
}
