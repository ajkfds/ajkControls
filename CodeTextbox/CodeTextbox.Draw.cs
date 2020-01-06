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

                for (int mark = 0; mark < 8; mark++)
                {
                    if (markBitmap[mark] != null) markBitmap[mark].Dispose();
                    markBitmap[mark] = new Bitmap(charSizeX, charSizeY);
                    using (Graphics gc = Graphics.FromImage(markBitmap[mark]))
                    {
                        gc.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
//                        gc.Clear(BackColor);
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
                StringBuilder sb = new StringBuilder();

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
                    int drawLine = 0;
                    int line = lineStart;
                    if (!multiLine) drawLine = document.Lines;

                    while (line <= document.Lines)
                    {
                        if (drawLine >= visibleLines + 2) break; // out of visible area

                        if (!document.IsVisibleLine(line)) // skip invisible lines
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

                        // draw line numbers (right padding)
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
//                                e.Graphics.DrawImage(charBitmap[0, lineString[lineString.Length - i - 1]], new Point(x, y));
                                x = x - charSizeX;
                            }
                        }

                        // draw charactors
                        x = xOffset * charSizeX;
                        drawChars(line, y, e);
                        drawMarkAndSelection(line, y, e);

                        y = y + charSizeY;
                        drawLine++;
                        line++;
                    }
                }

                e.Graphics.FillRectangle(lineNumberBrush, new Rectangle(0, 0, charSizeX * (xOffset - 1) + charSizeX / 2, dbDrawBox.Height));

                if (Editable)
                {
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(100, Color.Black)), new Point(xOffset * charSizeX, caretY + charSizeY), new Point(dbDrawBox.Width, caretY + charSizeY));
                }
                sw.Stop();
                System.Diagnostics.Debug.Print("draw : "+sw.Elapsed.TotalMilliseconds.ToString()+ "ms");
            }
        }

        private void drawChars(int line, int y,PaintEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            IntPtr hDC = e.Graphics.GetHdc();
            IntPtr hFont = this.Font.ToHfont();
            IntPtr hOldFont = (IntPtr)WinApi.SelectObject(hDC, hFont);

            int lineColor = (Color.LightGray.B << 16) + (Color.LightGray.G << 8) + Color.LightGray.R;
            IntPtr pen = WinApi.CreatePen(0, 1, lineColor);
            IntPtr oldPen = (IntPtr)WinApi.SelectObject(hDC, pen);

            int x = xOffset * charSizeX;
            int lineX = 0;
            int start = document.GetLineStartIndex(line);
            int end = start + document.GetLineLength(line);
            {
                for (int i = start; i < end; i++)
                {
                    if (i == document.Length) continue;
                    char ch = document.GetCharAt(i);
                    byte color = document.GetColorAt(i);
                    int xIncrement = 1;
                    if (ch == '\t')
                    {
                        xIncrement = tabSize - (lineX % tabSize);

                        WinApi.MoveToEx(hDC, x + 2, y + charSizeY - 2, IntPtr.Zero);
                        WinApi.LineTo(hDC, x - 2 + xIncrement * charSizeX, y + charSizeY - 2);
                        WinApi.SetPixel(hDC, x - 2 + xIncrement * charSizeX, y + charSizeY - 2, lineColor);

                        WinApi.MoveToEx(hDC, x - 2 + xIncrement * charSizeX, y + charSizeY - 2, IntPtr.Zero);
                        WinApi.LineTo(hDC, x - 2 + xIncrement * charSizeX, y + charSizeY - 8);
                        WinApi.SetPixel(hDC, x - 2 + xIncrement * charSizeX, y + charSizeY - 8, lineColor);

                    }
                    else if (ch == '\r')
                    {
                        WinApi.MoveToEx(hDC, (int)(x + charSizeX * 0.9), (int)(y + charSizeY * 0.2), IntPtr.Zero);
                        WinApi.LineTo(hDC, (int)(x + charSizeX * 0.9), (int)(y + charSizeY * 0.6));
                        WinApi.SetPixel(hDC, (int)(x + charSizeX * 0.9), (int)(y + charSizeY * 0.6), lineColor);

                        WinApi.MoveToEx(hDC, (int)(x + charSizeX * 0.2), (int)(y + charSizeY * 0.6), IntPtr.Zero);
                        WinApi.LineTo(hDC, (int)(x + charSizeX * 0.9), (int)(y + charSizeY * 0.6));
                        WinApi.SetPixel(hDC, (int)(x + charSizeX * 0.9), (int)(y + charSizeY * 0.6), lineColor);

                        WinApi.MoveToEx(hDC, (int)(x + charSizeX * 0.4), (int)(y + charSizeY * 0.4), IntPtr.Zero);
                        WinApi.LineTo(hDC, (int)(x + charSizeX * 0.2), (int)(y + charSizeY * 0.6));
                        WinApi.SetPixel(hDC, (int)(x + charSizeX * 0.2), (int)(y + charSizeY * 0.6), lineColor);

                        WinApi.MoveToEx(hDC, (int)(x + charSizeX * 0.4), (int)(y + charSizeY * 0.8), IntPtr.Zero);
                        WinApi.LineTo(hDC, (int)(x + charSizeX * 0.2), (int)(y + charSizeY * 0.6));
                        WinApi.SetPixel(hDC, (int)(x + charSizeX * 0.2), (int)(y + charSizeY * 0.6), lineColor);
                    }
                    else if (ch == '\n')
                    {
                        if (i == 0 || document.GetCharAt(i - 1) != '\r')
                        {
                            WinApi.MoveToEx(hDC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.2), IntPtr.Zero);
                            WinApi.LineTo(hDC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8));
                            WinApi.SetPixel(hDC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8), lineColor);

                            WinApi.MoveToEx(hDC, (int)(x + charSizeX * 0.4), (int)(y + charSizeY * 0.6), IntPtr.Zero);
                            WinApi.LineTo(hDC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8));
                            WinApi.SetPixel(hDC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8), lineColor);

                            WinApi.MoveToEx(hDC, (int)(x + charSizeX * 0.8), (int)(y + charSizeY * 0.6), IntPtr.Zero);
                            WinApi.LineTo(hDC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8));
                            WinApi.SetPixel(hDC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8), lineColor);

                        }
                    }
                    else
                    {
                        sb.Append(ch);
                        i++;
                        while (i < end)
                        {
                            ch = document.GetCharAt(i);
                            byte nextColor = document.GetColorAt(i);
                            if (ch != ' ')
                            {
                                if (ch == '\t' || ch == '\n' || ch == '\r' || nextColor != color) break;
                            }
                            sb.Append(ch);
                            i++;
                        }

                        int colorNo = Style.IntColorPallet[color];

                        WinApi.SetTextColor(hDC, colorNo);
                        WinApi.TextOut(hDC, x, y, sb.ToString(), sb.Length);

                        xIncrement = sb.Length;
                        sb.Clear();
                        i--;
                    }

                    lineX = lineX + xIncrement;
                    x = x + charSizeX * xIncrement;
                }
            }
            WinApi.SelectObject(hDC, oldPen);
            WinApi.DeleteObject(pen);
            WinApi.DeleteObject((IntPtr)WinApi.SelectObject(hDC, hOldFont));
            e.Graphics.ReleaseHdc(hDC);
        }
        private void drawMarkAndSelection(int line, int y, PaintEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            int x = xOffset * charSizeX;
            int lineX = 0;
            int start = document.GetLineStartIndex(line);
            int end = start + document.GetLineLength(line);
            {
                for (int i = start; i < end; i++)
                {
                    if (i == document.Length) continue;
                    char ch = document.GetCharAt(i);
                    byte color = document.GetColorAt(i);
                    int xIncrement = 1;
                    if (ch == '\t')
                    {
                        xIncrement = tabSize - (lineX % tabSize);
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
                                //IntPtr hdc = e.Graphics.GetHdc();
                                //IntPtr hsrc = ajkControls.WinApi.CreateCompatibleDC(hdc);
                                //IntPtr porg = (IntPtr)ajkControls.WinApi.SelectObject(hsrc, markBitmap[mark].GetHbitmap());

                                //ajkControls.WinApi.BitBlt(hdc, x, y,
                                //    markBitmap[mark].Width, markBitmap[mark].Height, hsrc, 0, 0,
                                //    ajkControls.WinApi.TernaryRasterOperations.SRCCOPY
                                //    );

                                //ajkControls.WinApi.DeleteDC(hsrc);
                                //e.Graphics.ReleaseHdc(hdc);
                                e.Graphics.DrawImage(markBitmap[mark], x, y);
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
            if ( line == document.Lines && document.Length == document.CaretIndex && Editable)
            {
//                y = y - charSizeY;
                e.Graphics.DrawLine(new Pen(Color.Black), new Point(x, y + 2), new Point(x, y + charSizeY - 2));
                e.Graphics.DrawLine(new Pen(Color.Black), new Point(x + 1, y + 2), new Point(x + 1, y + charSizeY - 2));
                caretX = x;
                caretY = y;
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
            while (index < nextLineIndex && x < xPos)
            {
                char ch = document.GetCharAt(index);
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
//                ch = document.GetCharAt(index);
            }

            if (index > 0 && index < nextLineIndex && document.GetCharAt(index) == '\n' && document.GetCharAt(index - 1) == '\r')
            {
                index--;
            }
            if (index < 0) index = 0;

            return index;
        }

    }
}
