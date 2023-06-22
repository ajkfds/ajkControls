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
        }

        CodeTextbox codeTextbox;
        HScrollBar hScrollBar;
        VScrollBar vScrollBar;


        private static Primitive.IconImage plusIcon = new Primitive.IconImage(Properties.Resources.plus);
        private static Primitive.IconImage minusIcon = new Primitive.IconImage(Properties.Resources.minus);

        private int[] actualLineNumbers = new int[] { };
        public int charSizeX = 1;
        public int charSizeY = 1;
        public int visibleLines = 10;

        public int xOffset = 0;


        // char size cache
        public int caretX = 0;
        public int caretY = 0;
        public void ResizeCharSize()
        {
            Graphics g = codeTextbox.CreateGraphics();
            Size fontSize = System.Windows.Forms.TextRenderer.MeasureText(g, "A", codeTextbox.Font, new Size(100, 100), TextFormatFlags.NoPadding);
            charSizeX = fontSize.Width;
            charSizeY = fontSize.Height;
            ResizeDrawBuffer();
            ReGenarateBuffer = true;
        }

        public void ResizeDrawBuffer()
        {
            visibleLines = (int)Math.Ceiling((double)(codeTextbox.Height / charSizeY));
            vScrollBar.LargeChange = visibleLines;
        }

        public void InvalidateLines(int beginLine, int endLine)
        {
            int beginDrawLine = 0;
            int endDrawLine = actualLineNumbers.Length;

            for (int i = 0; i < actualLineNumbers.Length; i++)
            {
                if (actualLineNumbers[i] < beginLine) beginDrawLine = i;
                if (endLine < actualLineNumbers[i])
                {
                    endDrawLine = i;
                    break;
                }
            }
            codeTextbox.Invalidate(new Rectangle(0, beginDrawLine * charSizeY, codeTextbox.Width, (endDrawLine - beginDrawLine) * charSizeY + 100));
        }

        public void Clean()
        {
            actualLineNumbers = new int[] { };
        }

        public int GetActualLineNo(int drawLineNumber)
        {
            if (actualLineNumbers.Length == 0) return 0;
            if (actualLineNumbers.Length < drawLineNumber) return actualLineNumbers.Last();
            return actualLineNumbers[drawLineNumber];
        }

        public Point GetCaretTopPoint()
        {
            return new Point(caretX, caretY);
        }

        public Point GetCaretBottomPoint()
        {
            return new Point(caretX, caretY + charSizeY);
        }

        // character & mark graphics Cache
        public volatile bool ReGenarateBuffer = true;
        private Bitmap[] markBitmap = new Bitmap[8];
        private Bitmap plusBitmap;
        private Bitmap minusBitmap;
        private Bitmap selectionBitmap;

        private void createGraphicsCashe()
        {
            unsafe
            {
                //                Bitmap bmp = new Bitmap(charSizeX, charSizeY);
                for (int mark = 0; mark < 8; mark++)
                {
                    if (markBitmap[mark] != null) markBitmap[mark].Dispose();
                    markBitmap[mark] = new Bitmap(charSizeX, charSizeY, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (Graphics gc = Graphics.FromImage(markBitmap[mark]))
                    {
                        gc.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        gc.Clear(Color.Transparent);
                        drawMark(gc, mark);
                    }
                    //                    markBitmap[mark].Save(@"mark" + mark.ToString() + ".bmp");
                }

            }

            {
                selectionBitmap = new Bitmap(charSizeX, charSizeY, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(selectionBitmap);

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.Clear(System.Drawing.Color.FromArgb(150, codeTextbox.SelectionColor.R, codeTextbox.SelectionColor.G, codeTextbox.SelectionColor.B));
            }

            {
                plusBitmap = new Bitmap(charSizeY, charSizeY);
                Graphics g = Graphics.FromImage(plusBitmap);
                g.Clear(codeTextbox.LeftColumnColor);
                g.DrawImage(plusIcon.GetImage(charSizeY, Primitive.IconImage.ColorStyle.Blue), 0, 0);
            }
            {
                minusBitmap = new Bitmap(charSizeY, charSizeY);
                Graphics g = Graphics.FromImage(minusBitmap);
                g.Clear(codeTextbox.LeftColumnColor);
                g.DrawImage(minusIcon.GetImage(charSizeY, Primitive.IconImage.ColorStyle.Blue), 0, 0);
            }

        }

        private void drawMark(Graphics gc, int mark)
        {
            Bitmap canvas = new Bitmap(charSizeX, charSizeY, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(canvas);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(Color.Black);
            Pen pen = new Pen(Color.White, 2);

            switch (codeTextbox.Style.MarkStyle[mark])
            {
                case CodeTextbox.MarkStyleEnum.underLine:
                    for (int i = 0; i < 2; i++)
                    {
                        g.DrawLine(pen,
                            new Point(0, (int)(charSizeY * 0.8) + i),
                            new Point((int)charSizeX, (int)(charSizeY * 0.8) + i)
                        );
                    }
                    break;
                case CodeTextbox.MarkStyleEnum.wave:
                    //         0.25*-1 0.25*0  0.25*1 0.25*2 0.25*3 0.25*4 0.25*5   
                    // 0.8                :       +                    :       +     
                    // 0.85               :                            :            
                    // 0.9        +       :                    +       :            

                    //                    for (int i = 0; i < 2; i++)
                    {
                        int i = 0;
                        g.DrawLine(pen,
                            new Point((int)(charSizeX * 0.25 * -1), (int)(charSizeY * 0.95) + i),
                            new Point((int)(charSizeX * 0.25 * 1), (int)(charSizeY * 0.8) + i)
                            );
                        g.DrawLine(pen,
                            new Point((int)(charSizeX * 0.25 * 1), (int)(charSizeY * 0.8) + i),
                            new Point((int)(charSizeX * 0.25 * 3), (int)(charSizeY * 0.95) + i)
                            );
                        g.DrawLine(pen,
                            new Point((int)(charSizeX * 0.25 * 3), (int)(charSizeY * 0.95) + i),
                            new Point((int)(charSizeX * 0.25 * 5), (int)(charSizeY * 0.8) + i)
                            );
                    }
                    break;
                case CodeTextbox.MarkStyleEnum.wave_inv:
                    //         0.25*-1 0.25*0  0.25*1 0.25*2 0.25*3 0.25*4 0.25*5   
                    // 0.8                :       +                    :       +     
                    // 0.85               :                            :            
                    // 0.9        +       :                    +       :            

                    //                    for (int i = 0; i < 2; i++)
                    {
                        int i = 0;
                        g.DrawLine(pen,
                            new Point((int)(charSizeX * 0.25 * -1), (int)(charSizeY * 0.8) + i),
                            new Point((int)(charSizeX * 0.25 * 1), (int)(charSizeY * 0.95) + i)
                            );
                        g.DrawLine(pen,
                            new Point((int)(charSizeX * 0.25 * 1), (int)(charSizeY * 0.95) + i),
                            new Point((int)(charSizeX * 0.25 * 3), (int)(charSizeY * 0.8) + i)
                            );
                        g.DrawLine(pen,
                            new Point((int)(charSizeX * 0.25 * 3), (int)(charSizeY * 0.8) + i),
                            new Point((int)(charSizeX * 0.25 * 5), (int)(charSizeY * 0.95) + i)
                            );
                    }
                    break;
                case CodeTextbox.MarkStyleEnum.fill:
                    g.FillRectangle(new SolidBrush(Color.White), -charSizeX, 0, charSizeX * 2, charSizeY);
                    break;
            }

            System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix();
            cm.Matrix00 = 0;
            cm.Matrix11 = 0;
            cm.Matrix22 = 0;
            cm.Matrix33 = 0;
            cm.Matrix40 = (float)codeTextbox.Style.MarkColor[mark].R / 255;
            cm.Matrix41 = (float)codeTextbox.Style.MarkColor[mark].G / 255;
            cm.Matrix42 = (float)codeTextbox.Style.MarkColor[mark].B / 255;
            cm.Matrix03 = (float)codeTextbox.Style.MarkColor[mark].A / 255;
            cm.Matrix44 = 0;

            System.Drawing.Imaging.ImageAttributes ia = new System.Drawing.Imaging.ImageAttributes();
            ia.SetColorMatrix(cm);

            gc.DrawImage(canvas, new Rectangle(0, 0, canvas.Width, canvas.Height),
                0, 0, canvas.Width, canvas.Height, GraphicsUnit.Pixel, ia);

            canvas.Dispose();
            g.Dispose();
        }

        public enum MarkStyleEnum
        {
            underLine,
            wave,
            wave_inv,
            fill
        }

        SolidBrush lineNumberTextBrush = new SolidBrush(Color.Silver);
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public void Draw(Rectangle clipRect)
        {
            unsafe
            {
                Document document = codeTextbox.Document;
                StringBuilder sb = new StringBuilder();

                if (ReGenarateBuffer)
                {
                    createGraphicsCashe();
                    ReGenarateBuffer = false;
                }
                if (actualLineNumbers.Length != visibleLines + 2)
                {
                    actualLineNumbers = new int[visibleLines + 2];
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
                        xOffset = lines.ToString().Length + 4;
                    }
                    else
                    {
                        xOffset = 0;
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
            int scrollH = hScrollBar.Value * charSizeX;

            StringBuilder sb = new StringBuilder(256);

            IntPtr tabPen = Primitive.WinApi.CreatePen(0, 1, Primitive.WinApi.GetColor(codeTextbox.TabColor));
            IntPtr lfPen = Primitive.WinApi.CreatePen(0, 1, Primitive.WinApi.GetColor(codeTextbox.LfColor));
            IntPtr crPen = Primitive.WinApi.CreatePen(0, 1, Primitive.WinApi.GetColor(codeTextbox.CrColor));
            IntPtr hFont = codeTextbox.Font.ToHfont();
            IntPtr hOldFont = (IntPtr)Primitive.WinApi.SelectObject(graphics.DC, hFont);

            //IntPtr oldPen = (IntPtr)Primitive.WinApi.SelectObject(graphics.DC, tabPen);
            int lineStart = document.GetActialLineNo(vScrollBar.Value + 1);
            int drawLine = 0;
            int line = lineStart;
            if (!codeTextbox.MultiLine) drawLine = document.Lines;

            int x = -clipRect.X;
            int y = -clipRect.Y;

            while (y < 0)
            {
                if (document.Lines >= line) break;
                if (!document.IsVisibleLine(line)) // skip invisible lines
                {
                }
                else
                {
                    y = y + charSizeY;
                    drawLine++;
                }
                line++;
            }

            while (line <= document.Lines)
            {
                if (drawLine >= visibleLines + 2) break; // exit : out of visible area
                if (y > clipRect.Y + clipRect.Height + charSizeY) break;

                if (!document.IsVisibleLine(line)) // skip invisible lines
                {
                    Primitive.WinApi.MoveToEx(graphics.DC, (int)(xOffset * charSizeX), (int)(y - 1), IntPtr.Zero);
                    Primitive.WinApi.LineTo(graphics.DC, (int)(codeTextbox.Width), (int)(y - 1));
                    Primitive.WinApi.LineTo(graphics.DC, (int)(codeTextbox.Width), (int)(y));
                    Primitive.WinApi.LineTo(graphics.DC, (int)(xOffset * charSizeX), (int)(y));
                    Primitive.WinApi.SetPixel(graphics.DC, (int)(codeTextbox.Width), (int)(y - 1), Primitive.WinApi.GetColor(codeTextbox.BlockUnderlineColor));

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
                x = (xOffset - 3) * charSizeX - clipRect.X;
                if (codeTextbox.MultiLine)
                {
                    if (document.IsBlockHeadLine(line))
                    {
                        if (document.IsCollapsed(line))
                        {   // plus mark
                            IntPtr hsrc = Primitive.WinApi.CreateCompatibleDC(graphics.DC);
                            IntPtr hbmp = plusBitmap.GetHbitmap();
                            IntPtr porg = Primitive.WinApi.SelectObject(hsrc, hbmp);
                            Primitive.WinApi.BitBlt(graphics.DC, x, y, charSizeY, charSizeY, hsrc, 0, 0, (uint)Primitive.WinApi.TernaryRasterOperations.SRCCOPY);
                            Primitive.WinApi.DeleteObject(porg);
                            Primitive.WinApi.DeleteObject(hbmp);
                            Primitive.WinApi.DeleteDC(hsrc);
                        }
                        else
                        {   // minus mark
                            IntPtr hsrc = Primitive.WinApi.CreateCompatibleDC(graphics.DC);
                            IntPtr hbmp = minusBitmap.GetHbitmap();
                            IntPtr porg = Primitive.WinApi.SelectObject(hsrc, hbmp);
                            Primitive.WinApi.BitBlt(graphics.DC, x, y, charSizeY, charSizeY, hsrc, 0, 0, (uint)Primitive.WinApi.TernaryRasterOperations.SRCCOPY);
                            Primitive.WinApi.DeleteObject(porg);
                            Primitive.WinApi.DeleteObject(hbmp);
                            Primitive.WinApi.DeleteDC(hsrc);
                        }
                    }
                    x = x - charSizeX;

                    string lineString = line.ToString();
                    Primitive.WinApi.SetTextColor(graphics.DC, Primitive.WinApi.GetColor(Color.Silver));
                    Primitive.WinApi.ExtTextOut(graphics.DC, x - lineString.Length * charSizeX, y, 0, lineString);
                }

                sw.Start();

                Primitive.WinApi.SetBkMode(graphics.DC, 0);

                int lineX;
                int start;
                int end;

                // draw charactors
                x = xOffset * charSizeX - clipRect.X - scrollH;
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
                            Primitive.WinApi.SelectObject(graphics.DC, tabPen);
                            Primitive.WinApi.MoveToEx(graphics.DC, x + 2, y + charSizeY - 2, IntPtr.Zero);
                            Primitive.WinApi.LineTo(graphics.DC, x - 2 + xIncrement * charSizeX, y + charSizeY - 2);
                            Primitive.WinApi.MoveToEx(graphics.DC, x - 2 + xIncrement * charSizeX, y + charSizeY - 2, IntPtr.Zero);
                            Primitive.WinApi.LineTo(graphics.DC, x - 2 + xIncrement * charSizeX, y + charSizeY - 8);

                        }
                        else if (ch == '\r')
                        {
                            Primitive.WinApi.SelectObject(graphics.DC, crPen);
                            Primitive.WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.9), (int)(y + charSizeY * 0.2), IntPtr.Zero);
                            Primitive.WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.9), (int)(y + charSizeY * 0.6));
                            Primitive.WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.2), (int)(y + charSizeY * 0.6), IntPtr.Zero);
                            Primitive.WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.9), (int)(y + charSizeY * 0.6));
                            Primitive.WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.4), (int)(y + charSizeY * 0.4), IntPtr.Zero);
                            Primitive.WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.2), (int)(y + charSizeY * 0.6));
                            Primitive.WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.4), (int)(y + charSizeY * 0.8), IntPtr.Zero);
                            Primitive.WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.2), (int)(y + charSizeY * 0.6));
                        }
                        else if (ch == '\n')
                        {
                            if (i == 0 || document.GetCharAt(i - 1) != '\r')
                            {
                                Primitive.WinApi.SelectObject(graphics.DC, lfPen);
                                Primitive.WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.2), IntPtr.Zero);
                                Primitive.WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8));
                                Primitive.WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.4), (int)(y + charSizeY * 0.6), IntPtr.Zero);
                                Primitive.WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8));
                                Primitive.WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.8), (int)(y + charSizeY * 0.6), IntPtr.Zero);
                                Primitive.WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8));
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
                        x = x + charSizeX * xIncrement;
                    }
                }

                Primitive.WinApi.SetBkMode(graphics.DC, 1);

                // draw mark
                x = xOffset * charSizeX - scrollH;
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
                            {   // tab
                                xIncrement = codeTextbox.TabSize - (lineX % codeTextbox.TabSize);
                                IntPtr pSource = Primitive.WinApi.CreateCompatibleDC(graphics.DC);
                                IntPtr hbmp = selectionBitmap.GetHbitmap(Color.Black);
                                IntPtr pOrig = Primitive.WinApi.SelectObject(pSource, hbmp);
                                for (int j = 0; j < xIncrement; j++)
                                {
                                    Primitive.WinApi.AlphaBlend(
                                        graphics.DC, x + j * charSizeX, y, selectionBitmap.Width, selectionBitmap.Height,
                                        pSource, 0, 0, selectionBitmap.Width, selectionBitmap.Height,
                                        new Primitive.WinApi.BLENDFUNCTION(Primitive.WinApi.AC_SRC_OVER, 0, 0xff, Primitive.WinApi.AC_SRC_ALPHA)
                                        );
                                }
                                IntPtr pNew = Primitive.WinApi.SelectObject(pSource, pOrig);
                                Primitive.WinApi.DeleteObject(pNew);
                                Primitive.WinApi.DeleteObject(hbmp);
                                Primitive.WinApi.DeleteDC(pSource);
                            }
                            else
                            {
                                IntPtr pSource = Primitive.WinApi.CreateCompatibleDC(graphics.DC);
                                IntPtr hbmp = selectionBitmap.GetHbitmap(Color.Black);
                                IntPtr pOrig = Primitive.WinApi.SelectObject(pSource, hbmp);
                                Primitive.WinApi.AlphaBlend(
                                    graphics.DC, x, y, selectionBitmap.Width, selectionBitmap.Height,
                                    pSource, 0, 0, selectionBitmap.Width, selectionBitmap.Height,
                                    new Primitive.WinApi.BLENDFUNCTION(Primitive.WinApi.AC_SRC_OVER, 0, 0xff, Primitive.WinApi.AC_SRC_ALPHA)
                                    );
                                IntPtr pNew = Primitive.WinApi.SelectObject(pSource, pOrig);
                                Primitive.WinApi.DeleteObject(pNew);
                                Primitive.WinApi.DeleteObject(hbmp);
                                Primitive.WinApi.DeleteDC(pSource);
                            }
                        }


                        // mark
                        if (document.GetMarkAt(i) != 0)
                        {
                            for (int mark = 7; mark >= 0; mark--)
                            {
                                //if ((document.GetMarkAt(i) & (1 << mark)) != 0) drawMarkGdi(hDC, x, y, mark);
                                if ((document.GetMarkAt(i) & (1 << mark)) != 0)
                                {
                                    IntPtr pSource = Primitive.WinApi.CreateCompatibleDC(graphics.DC);
                                    IntPtr hbmp = markBitmap[mark].GetHbitmap(Color.Black);
                                    IntPtr pOrig = Primitive.WinApi.SelectObject(pSource, hbmp);
                                    Primitive.WinApi.AlphaBlend(
                                        graphics.DC, x, y, markBitmap[mark].Width, markBitmap[mark].Height,
                                        pSource, 0, 0, markBitmap[mark].Width, markBitmap[mark].Height,
                                        new Primitive.WinApi.BLENDFUNCTION(Primitive.WinApi.AC_SRC_OVER, 0, 0xff, Primitive.WinApi.AC_SRC_ALPHA)
                                        );
                                    IntPtr pNew = Primitive.WinApi.SelectObject(pSource, pOrig);
                                    Primitive.WinApi.DeleteObject(pNew);
                                    Primitive.WinApi.DeleteObject(hbmp);
                                    Primitive.WinApi.DeleteDC(pSource);
                                }

                            }
                        }

                        // caret
                        if (i == document.CaretIndex & codeTextbox.Editable)
                        {
                            IntPtr hrgn = Primitive.WinApi.CreateRectRgn(x, y + 2, x + 2, y + charSizeY - 2);
                            IntPtr hbrush = Primitive.WinApi.CreateSolidBrush(Primitive.WinApi.GetColor(codeTextbox.CarletColor));
                            Primitive.WinApi.FillRgn(graphics.DC, hrgn, hbrush);
                            Primitive.WinApi.DeleteObject(hbrush);
                            Primitive.WinApi.DeleteObject(hrgn);
                            caretX = x;
                            caretY = y;
                        }

                        lineX = lineX + xIncrement;
                        x = x + charSizeX * xIncrement;
                    }
                }


                // carlet at EOF
                if (line == document.Lines && document.Length == document.CaretIndex && codeTextbox.Editable)
                {
                    IntPtr hrgn = Primitive.WinApi.CreateRectRgn(x, y + 2, x + 2, y + charSizeY - 2);
                    IntPtr hbrush = Primitive.WinApi.CreateSolidBrush(Primitive.WinApi.GetColor(codeTextbox.CarletColor));
                    Primitive.WinApi.FillRgn(graphics.DC, hrgn, hbrush);
                    Primitive.WinApi.DeleteObject(hbrush);
                    Primitive.WinApi.DeleteObject(hrgn);
                    caretX = x;
                    caretY = y;
                }

                y = y + charSizeY;
                drawLine++;
                line++;
            }

            caretX += clipRect.X;
            caretY += clipRect.Y;

            // underline @ carlet
            if (codeTextbox.Editable)
            {
                Primitive.WinApi.SelectObject(graphics.DC, tabPen);
                Primitive.WinApi.MoveToEx(graphics.DC, xOffset * charSizeX - clipRect.X, caretY + charSizeY - clipRect.Y, IntPtr.Zero);
                Primitive.WinApi.LineTo(graphics.DC, codeTextbox.Width - clipRect.X, caretY + charSizeY - clipRect.Y);
            }

            Primitive.WinApi.SetBkMode(graphics.DC, 1);
            {
                // left column
                IntPtr hrgn = Primitive.WinApi.CreateRectRgn(-clipRect.X, -clipRect.Y, charSizeX * (xOffset - 1) + charSizeX / 2, codeTextbox.Height);
                IntPtr hbrush = Primitive.WinApi.CreateSolidBrush(Primitive.WinApi.GetColor(codeTextbox.LeftColumnColor));
                Primitive.WinApi.FillRgn(graphics.DC, hrgn, hbrush);
                Primitive.WinApi.DeleteObject(hbrush);
                Primitive.WinApi.DeleteObject(hrgn);
            }

            //Primitive.WinApi.SelectObject(hDC, oldPen);
            Primitive.WinApi.DeleteObject(tabPen);
            Primitive.WinApi.DeleteObject(crPen);
            Primitive.WinApi.DeleteObject(lfPen);
            Primitive.WinApi.DeleteObject((IntPtr)Primitive.WinApi.SelectObject(graphics.DC, hOldFont));
            //g.ReleaseHdc(hDC);

            //sw.Stop();
            //System.Diagnostics.Debug.Print("draw : " + sw.Elapsed.TotalMilliseconds.ToString() + "ms");

        }




        public int hitIndex(int x, int y)
        {
            Document document = codeTextbox.Document;
            //int line = y / charSizeY + vScrollBar.Value+1;
            if (y < 0) return 0;
            int line = codeTextbox.GetActualLineNo(y / charSizeY);
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