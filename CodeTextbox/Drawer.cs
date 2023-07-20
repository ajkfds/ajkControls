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

/// <summary>
/// CodeTextbox Image Draw Module
/// </summary>
namespace ajkControls.CodeTextbox
{
    public class Drawer
    {
        public Drawer(CodeTextbox codeTextBox,HScrollBar hScrollBar,VScrollBar vScrollBar)
        {
            this.codeTextbox = codeTextBox;
            this.vScrollBar = vScrollBar;
            this.hScrollBar = hScrollBar;
            Cache = new DrawCache(codeTextBox);
        }

        CodeTextbox codeTextbox;
        HScrollBar hScrollBar;
        VScrollBar vScrollBar;


        public DrawCache Cache;

        public void ResizeCharSize()
        {
            Cache.ResizeCharSize();
        }

        public void ResizeDrawBuffer()
        {
            Cache.ResizeDrawBuffer();
        }

        public void InvalidateLines(int beginLine, int endLine)
        {
            int beginDrawLine = 0;
            int endDrawLine = Cache.actualLineNumbers.Length;

            for (int i = 0; i < Cache.actualLineNumbers.Length; i++)
            {
                if (Cache.actualLineNumbers[i] < beginLine) beginDrawLine = i;
                if (endLine < Cache.actualLineNumbers[i])
                {
                    endDrawLine = i;
                    break;
                }
            }
            codeTextbox.Invalidate(new Rectangle(0, beginDrawLine * Cache.charSizeY, codeTextbox.Width, (endDrawLine - beginDrawLine) * Cache.charSizeY + 100));
        }

        public void Clean()
        {
            Cache.actualLineNumbers = new int[] { };
        }

        public int GetActualLineNo(int drawLineNumber)
        {
            if (Cache.actualLineNumbers.Length == 0) return 0;
            if (Cache.actualLineNumbers.Length < drawLineNumber) return Cache.actualLineNumbers.Last();
            return Cache.actualLineNumbers[drawLineNumber];
        }

        public Point GetCaretTopPoint()
        {
            return new Point(Cache.caretX, Cache.caretY);
        }

        public Point GetCaretBottomPoint()
        {
            return new Point(Cache.caretX, Cache.caretY + Cache.charSizeY);
        }

        public enum MarkStyleEnum
        {
            underLine,
            wave,
            wave_inv,
            fill
        }

        SolidBrush lineNumberTextBrush = new SolidBrush(Color.Silver);

        public void Draw(Rectangle clipRect)
        {
            unsafe
            {
                Document document = codeTextbox.Document;
                StringBuilder sb = new StringBuilder();

                if (Cache.ReGenarateBuffer)
                {
                    Cache.CreateGraphicsCashe();
                    Cache.ReGenarateBuffer = false;
                }
                if (Cache.actualLineNumbers.Length != Cache.visibleLines + 2)
                {
                    Cache.actualLineNumbers = new int[Cache.visibleLines + 2];
                }

                Primitive.GraWin graphics = new Primitive.GraWin(codeTextbox.Handle, new Primitive.FontInfo(codeTextbox.Font));

                graphics.BeginPaint(clipRect);

                graphics.BackColor = codeTextbox.BackColor;

                if (document == null)
                {
                    graphics.EndPaint();
                    return;
                }
                lock (document)
                {
                    if (codeTextbox.MultiLine)
                    {
                        int lines = document.Lines;
                        if (lines < 999) lines = 999;
                        Cache.xOffset = lines.ToString().Length + 4;
                    }
                    else
                    {
                        Cache.xOffset = 0;
                    }

                    int lineStart = document.GetActialLineNo(vScrollBar.Value + 1);

                    int drawLine = 0;
                    int line = lineStart;
                    if (!codeTextbox.MultiLine) drawLine = document.Lines;

                    drawChars(graphics, clipRect);

                    drawLine = 0;
                    line = lineStart;
                    if (!codeTextbox.MultiLine) drawLine = document.Lines;
                }

                graphics.EndPaint();
            }
        }


        private void drawChars(Primitive.GraWin graphics, Rectangle clipRect)
        {
            Document document = codeTextbox.Document;
            //sw.Reset();
            int scrollH = hScrollBar.Value * Cache.charSizeX;

            StringBuilder sb = new StringBuilder(256);

            IntPtr hFont = codeTextbox.Font.ToHfont();
            IntPtr hOldFont = (IntPtr)Primitive.WinApi.SelectObject(graphics.DC, hFont);

            int lineStart = document.GetActialLineNo(vScrollBar.Value + 1);
            int drawLine = 0;
            if (!codeTextbox.MultiLine) drawLine = document.Lines;

            int x = -clipRect.X;
            int y = -clipRect.Y;

            // skip invisible lines on the top side
            while (y < 0)
            {
                if (document.Lines >= lineStart) break;
                if (!document.IsVisibleLine(lineStart)) // skip invisible lines
                {
                }
                else
                {
                    y = y + Cache.charSizeY;
                    drawLine++;
                }
                lineStart++;
            }

            int line = lineStart;
            while (line <= document.Lines)
            {
                if (drawLine >= Cache.visibleLines + 2) break; // exit : out of visible area
                if (y > clipRect.Y + clipRect.Height + Cache.charSizeY) break;

                if (!document.IsVisibleLine(line)) // skip invisible lines
                {
                    Primitive.WinApi.MoveToEx(graphics.DC, (int)(Cache.xOffset * Cache.charSizeX), (int)(y - 1), IntPtr.Zero);
                    Primitive.WinApi.LineTo(graphics.DC, (int)(codeTextbox.Width), (int)(y - 1));
                    Primitive.WinApi.LineTo(graphics.DC, (int)(codeTextbox.Width), (int)(y));
                    Primitive.WinApi.LineTo(graphics.DC, (int)(Cache.xOffset * Cache.charSizeX), (int)(y));
                    Primitive.WinApi.SetPixel(graphics.DC, (int)(codeTextbox.Width), (int)(y - 1), Primitive.WinApi.GetColor(codeTextbox.BlockUnderlineColor));

                    line++;
                    while (line < document.Lines)
                    {
                        if (document.IsVisibleLine(line)) break;
                        line++;
                    }
                    continue;
                }
                Cache.actualLineNumbers[drawLine] = line;

                // draw line numbers (right padding)
                x = (Cache.xOffset - 3) * Cache.charSizeX - clipRect.X;
                Primitive.WinApi.SetBkMode(graphics.DC, 0); // set opaque background mode

                int lineX;
                int start;
                int end;

                // draw charactors
                x = Cache.xOffset * Cache.charSizeX - clipRect.X - scrollH;
                lineX = 0;
                start = document.GetLineStartIndex(line);
                end = start + document.GetLineLength(line);
                {
                    for (int i = start; i < end; i++)
                    {
                        if (i == document.Length) continue;
                        char ch = document.GetCharAt(i);
                        byte color = document.GetColorAt(i);
                        int xIncrement = 1;
                        if (ch == '\t')
                        {
                            xIncrement = codeTextbox.TabSize - (lineX % codeTextbox.TabSize);
                            Cache.DrawTab(graphics, x, y, xIncrement);

                        }
                        else if (ch == '\r')
                        {
                            Cache.DrawCr(graphics, x, y);
                        }
                        else if (ch == '\n')
                        {
                            if (i == 0 || document.GetCharAt(i - 1) != '\r')
                            {
                                Cache.DrawLf(graphics, x, y);
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

                            int colorNo = Primitive.WinApi.GetColor(codeTextbox.Style.ColorPallet[color]);

                            Primitive.WinApi.SetTextColor(graphics.DC, colorNo);
                            Primitive.WinApi.ExtTextOut(graphics.DC, x, y, 0, sb.ToString());

                            xIncrement = sb.Length;
                            sb.Clear();
                            i--;
                        }
                        lineX = lineX + xIncrement;
                        x = x + Cache.charSizeX * xIncrement;
                    }
                }

                Primitive.WinApi.SetBkMode(graphics.DC, 1); // transperaent background mode

                // draw mark
                x = Cache.xOffset * Cache.charSizeX - scrollH;
                lineX = 0;
                start = document.GetLineStartIndex(line);
                end = start + document.GetLineLength(line);
                {
                    for (int i = start; i < end; i++)
                    {
                        if (i == document.Length) continue;
                        char ch = document.GetCharAt(i);
                        byte color = document.GetColorAt(i);
                        int xIncrement = 1;
                        if (ch == '\t')
                        {
                            xIncrement = codeTextbox.TabSize - (lineX % codeTextbox.TabSize);
                        }

                        // selection
                        if (i >= document.SelectionStart && i < document.SelectionLast)
                        {
                            if (ch == '\t')
                            {   // tab selection
                                xIncrement = codeTextbox.TabSize - (lineX % codeTextbox.TabSize);
                                Cache.DrawSelection(graphics, x, y, xIncrement);
                            }
                            else
                            {   // selection
                                Cache.DrawSelection(graphics, x, y);
                            }
                        }


                        // mark
                        if (document.GetMarkAt(i) != 0)
                        {
                            for (int mark = 7; mark >= 0; mark--)
                            {
                                if ((document.GetMarkAt(i) & (1 << mark)) != 0)
                                {
                                    Cache.DrawMark(graphics, x, y, mark);
                                }
                            }
                        }

                        // caret
                        if (i == document.CaretIndex & codeTextbox.Editable)
                        {
                            Cache.DrawCarlet(graphics, x, y);
                            Cache.caretX = x;
                            Cache.caretY = y;
                        }

                        lineX = lineX + xIncrement;
                        x = x + Cache.charSizeX * xIncrement;
                    }
                }


                // carlet at EOF
                if (line == document.Lines && document.Length == document.CaretIndex && codeTextbox.Editable)
                {
                    Cache.DrawCarlet(graphics, x, y);
                    Cache.caretX = x;
                    Cache.caretY = y;
                }

                y = y + Cache.charSizeY;
                drawLine++;
                line++;
            }
            if(drawLine < Cache.actualLineNumbers.Length) Cache.actualLineNumbers[drawLine] = 0;

            Cache.caretX += clipRect.X;
            Cache.caretY += clipRect.Y;

            // underline @ carlet
            if (codeTextbox.Editable)
            {
                Primitive.WinApi.SelectObject(graphics.DC, Cache.tabPen);
                Primitive.WinApi.MoveToEx(graphics.DC, Cache.xOffset * Cache.charSizeX - clipRect.X, Cache.caretY + Cache.charSizeY - clipRect.Y, IntPtr.Zero);
                Primitive.WinApi.LineTo(graphics.DC, codeTextbox.Width - clipRect.X, Cache.caretY + Cache.charSizeY - clipRect.Y);
            }

            Primitive.WinApi.SetBkMode(graphics.DC, 1);
            {
                // left column
                IntPtr hrgn = Primitive.WinApi.CreateRectRgn(-clipRect.X, -clipRect.Y, Cache.charSizeX * (Cache.xOffset - 1) + Cache.charSizeX / 2, codeTextbox.Height);
                IntPtr hbrush = Primitive.WinApi.CreateSolidBrush(Primitive.WinApi.GetColor(codeTextbox.LeftColumnColor));
                Primitive.WinApi.FillRgn(graphics.DC, hrgn, hbrush);
                Primitive.WinApi.DeleteObject(hbrush);
                Primitive.WinApi.DeleteObject(hrgn);
            }

            //y = 0;
            //for(int dline = 0; dline < Cache.visibleLines; dline++)
            //{
            //    if (Cache.actualLineNumbers[dline] == 0) break;
            //    line = Cache.actualLineNumbers[dline];

            //    // line number
            //    string lineString = line.ToString();
            //    Primitive.WinApi.SetTextColor(graphics.DC, Primitive.WinApi.GetColor(Color.Silver));
            //    Primitive.WinApi.ExtTextOut(graphics.DC, Cache.xOffset * Cache.charSizeX- Cache.charSizeX - lineString.Length * Cache.charSizeX, y, 0, lineString);

            //    if (document.IsBlockHeadLine(line))
            //    {
            //        if (document.IsCollapsed(line))
            //        {   // plus mark
            //            Cache.DrawPuls(graphics, 0, y);
            //        }
            //        else
            //        {   // minus mark
            //            Cache.DrawMinus(graphics, 0, y);
            //        }
            //    }
            //    y = y + Cache.charSizeY;
            //}

            line = lineStart;
            y = -clipRect.Y;
            drawLine = 0;
            while (line <= document.Lines)
            {
                if (drawLine >= Cache.visibleLines + 2) break; // exit : out of visible area
                if (y > clipRect.Y + clipRect.Height + Cache.charSizeY) break;

                if (!document.IsVisibleLine(line)) // skip invisible lines
                {
                    line++;
                    while (line < document.Lines)
                    {
                        if (document.IsVisibleLine(line)) break;
                        line++;
                    }
                    continue;
                }

                // line number
                string lineString = line.ToString();
                Primitive.WinApi.SetTextColor(graphics.DC, Primitive.WinApi.GetColor(Color.Silver));
                Primitive.WinApi.ExtTextOut(graphics.DC, Cache.xOffset * Cache.charSizeX - Cache.charSizeX - lineString.Length * Cache.charSizeX, y, 0, lineString);

                if (document.IsBlockHeadLine(line))
                {
                    if (document.IsCollapsed(line))
                    {   // plus mark
                        Cache.DrawPuls(graphics, 0, y);
                    }
                    else
                    {   // minus mark
                        Cache.DrawMinus(graphics, 0, y);
                    }
                }

                y = y + Cache.charSizeY;
                drawLine++;
                line++;
            }

            Primitive.WinApi.DeleteObject((IntPtr)Primitive.WinApi.SelectObject(graphics.DC, hOldFont));
        }




        public int hitIndex(int x, int y)
        {
            Document document = codeTextbox.Document;
            //int line = y / charSizeY + vScrollBar.Value+1;
            if (y < 0) return 0;
            int line = codeTextbox.GetActualLineNo(y / Cache.charSizeY);
            if (line > document.Lines - 1) line = document.Lines - 1;
            if (line < 1) line = 1;
            int hitX = x / Cache.charSizeX - Cache.xOffset;

            int index = document.GetLineStartIndex(line);
            int nextLineIndex = index + document.GetLineLength(line);

            int xPos = 0;
            char ch = document.GetCharAt(index);
            while (index < nextLineIndex)
            {
                if (ch == '\r' || ch == '\n') break;
                if (ch == '\t')
                {
                    xPos = xPos + codeTextbox.TabSize - (xPos % codeTextbox.TabSize);
                }
                else
                {
                    xPos++;
                }
                if (xPos > hitX) break;
                index++;
                if (index >= document.Length)
                {
                    index--;
                    break;
                }
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
            Document document = codeTextbox.Document;
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
                    xPos = xPos + codeTextbox.TabSize - (xPos % codeTextbox.TabSize);
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

        public int getXPos(int index, int line)
        {
            Document document = codeTextbox.Document;
            int i = document.GetLineStartIndex(line);
            int xPos = 0;
            char ch = document.GetCharAt(i);
            while (i < index)
            {
                if (ch == '\r' || ch == '\n') break;
                if (ch == '\t')
                {
                    xPos = xPos + codeTextbox.TabSize - (xPos % codeTextbox.TabSize);
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

        public int getIndex(int xPos, int line)
        {
            Document document = codeTextbox.Document;
            int index = document.GetLineStartIndex(line);
            int nextLineIndex = index + document.GetLineLength(line);

            int x = 0;
            while (index < nextLineIndex && x < xPos)
            {
                char ch = document.GetCharAt(index);
                if (ch == '\r' || ch == '\n') break;
                if (ch == '\t')
                {
                    x = x + codeTextbox.TabSize - (x % codeTextbox.TabSize);
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