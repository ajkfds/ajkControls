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
        int charSizeX = 0;
        int charSizeY = 0;
        int visibleLines = 10;
        private void resizeCharSize()
        {
            Graphics g = this.CreateGraphics();
            Size fontSize = System.Windows.Forms.TextRenderer.MeasureText(g, "A", Font, new Size(100, 100), TextFormatFlags.NoPadding);
            charSizeX = fontSize.Width;
            charSizeY = fontSize.Height;
            resizeDrawBuffer();
            reGenarateBuffer = true;
        }

        private void resizeDrawBuffer()
        {
            visibleLines = (int)Math.Ceiling((double)(Height / charSizeY));
            vScrollBar.LargeChange = visibleLines;
            UpdateVScrollBarRange();
        }


        // character & mark graphics buffer
        volatile bool reGenarateBuffer = true;
        private Bitmap[] markBitmap = new Bitmap[8];
        private Bitmap plusBitmap;
        private Bitmap minusBitmap;
        private Bitmap selectionBitmap;

        private void createGraphicsBuffer()
        {
            unsafe
            {
                //                Bitmap bmp = new Bitmap(charSizeX, charSizeY);
                for (int mark = 0; mark < 8; mark++)
                {
                    if (markBitmap[mark] != null) markBitmap[mark].Dispose();
                    markBitmap[mark] = new Bitmap(charSizeX, charSizeY,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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
                g.Clear(System.Drawing.Color.FromArgb(150, SelectionColor.R, SelectionColor.G, SelectionColor.B));
            }

            {
                plusBitmap = new Bitmap(charSizeY, charSizeY);
                Graphics g = Graphics.FromImage(plusBitmap);
                g.Clear(leftColumnColor);
                g.DrawImage(plusIcon.GetImage(charSizeY, IconImage.ColorStyle.Blue), 0, 0);
            }
            {
                minusBitmap = new Bitmap(charSizeY, charSizeY);
                Graphics g = Graphics.FromImage(minusBitmap);
                g.Clear(leftColumnColor);
                g.DrawImage(minusIcon.GetImage(charSizeY, IconImage.ColorStyle.Blue), 0, 0);
            }

        }

        private void drawMark(Graphics gc, int mark)
        {
            Bitmap canvas = new Bitmap(charSizeX, charSizeY, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(canvas);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(Color.Black);
            Pen pen = new Pen(Color.White, 2);

            switch (Style.MarkStyle[mark])
            {
                case MarkStyleEnum.underLine:
                    for (int i = 0; i < 2; i++)
                    {
                        g.DrawLine(pen,
                            new Point(0, (int)(charSizeY * 0.8) + i),
                            new Point((int)charSizeX, (int)(charSizeY * 0.8) + i)
                        );
                    }
                    break;
                case MarkStyleEnum.wave:
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
                case MarkStyleEnum.wave_inv:
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
                case MarkStyleEnum.fill:
                    g.FillRectangle(new SolidBrush(Color.White), -charSizeX, 0, charSizeX*2, charSizeY);
                    break;
            }

            System.Drawing.Imaging.ColorMatrix cm = new System.Drawing.Imaging.ColorMatrix();
            cm.Matrix00 = 0;
            cm.Matrix11 = 0;
            cm.Matrix22 = 0;
            cm.Matrix33 = 0;
            cm.Matrix40 = (float)Style.MarkColor[mark].R / 255;
            cm.Matrix41 = (float)Style.MarkColor[mark].G / 255;
            cm.Matrix42 = (float)Style.MarkColor[mark].B / 255;
            cm.Matrix03 = (float)Style.MarkColor[mark].A / 255;
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

        private void draw(Rectangle clipRect)
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

                GraWin graphics = new GraWin(this.Handle,new FontInfo(Font));

                graphics.BeginPaint(clipRect);

                //                Graphics g = this.CreateGraphics();
                //                g.Clear(BackColor);
                graphics.BackColor = BackColor;

                if (document == null)
                {
                    graphics.EndPaint();
                    return;
                }
                lock (document)
                {
                    if (multiLine)
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
                    if (!multiLine) drawLine = document.Lines;

                    drawChars(graphics,clipRect);

                    drawLine = 0;
                    line = lineStart;
                    if (!multiLine) drawLine = document.Lines;
                }

                graphics.EndPaint();
            }
        }


        private void drawChars(GraWin graphics, Rectangle clipRect)
        {
            //sw.Reset();

            StringBuilder sb = new StringBuilder(256);
            WinApi.SetBkMode(graphics.DC, 1);

            {
                // left column
                IntPtr hrgn = WinApi.CreateRectRgn(-clipRect.X,-clipRect.Y, charSizeX * (xOffset - 1) + charSizeX / 2, Height);
                IntPtr hbrush = WinApi.CreateSolidBrush(WinApi.GetColor(leftColumnColor));
                WinApi.FillRgn(graphics.DC, hrgn, hbrush);
                WinApi.DeleteObject(hbrush);
                WinApi.DeleteObject(hrgn);
            }

            IntPtr tabPen = WinApi.CreatePen(0, 1, WinApi.GetColor(tabColor));
            IntPtr lfPen = WinApi.CreatePen(0, 1, WinApi.GetColor(lfColor));
            IntPtr crPen = WinApi.CreatePen(0, 1, WinApi.GetColor(crColor));
            IntPtr hFont = this.Font.ToHfont();
            IntPtr hOldFont = (IntPtr)WinApi.SelectObject(graphics.DC, hFont);

            //IntPtr oldPen = (IntPtr)WinApi.SelectObject(graphics.DC, tabPen);
            int lineStart = document.GetActialLineNo(vScrollBar.Value + 1);
            int drawLine = 0;
            int line = lineStart;
            if (!multiLine) drawLine = document.Lines;

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
                if (y > clipRect.Y + clipRect.Height+charSizeY) break;

                if (!document.IsVisibleLine(line)) // skip invisible lines
                {
                    WinApi.MoveToEx(graphics.DC, (int)(xOffset * charSizeX), (int)(y-1), IntPtr.Zero);
                    WinApi.LineTo(graphics.DC, (int)(Width), (int)(y - 1));
                    WinApi.LineTo(graphics.DC, (int)(Width), (int)(y));
                    WinApi.LineTo(graphics.DC, (int)(xOffset * charSizeX), (int)(y));
                    WinApi.SetPixel(graphics.DC, (int)(Width), (int)(y-1), WinApi.GetColor(blockUnderlineColor));

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
                if (multiLine)
                {
                    if (document.IsBlockHeadLine(line))
                    {
                        if (document.IsCollapsed(line))
                        {   // plus mark
                            IntPtr hsrc = WinApi.CreateCompatibleDC(graphics.DC);
                            IntPtr hbmp = plusBitmap.GetHbitmap();
                            IntPtr porg = WinApi.SelectObject(hsrc, hbmp);
                            WinApi.BitBlt(graphics.DC, x, y, charSizeY, charSizeY, hsrc, 0, 0, (uint)WinApi.TernaryRasterOperations.SRCCOPY);
                            WinApi.DeleteObject(porg);
                            WinApi.DeleteObject(hbmp);
                            WinApi.DeleteDC(hsrc);
                        }
                        else
                        {   // minus mark
                            IntPtr hsrc = WinApi.CreateCompatibleDC(graphics.DC);
                            IntPtr hbmp = minusBitmap.GetHbitmap();
                            IntPtr porg = WinApi.SelectObject(hsrc, hbmp);
                            WinApi.BitBlt(graphics.DC, x, y, charSizeY, charSizeY, hsrc, 0, 0, (uint)WinApi.TernaryRasterOperations.SRCCOPY);
                            WinApi.DeleteObject(porg);
                            WinApi.DeleteObject(hbmp);
                            WinApi.DeleteDC(hsrc);
                        }
                    }
                    x = x - charSizeX;

                    string lineString = line.ToString();
                    WinApi.SetTextColor(graphics.DC, WinApi.GetColor(Color.Silver));
//                    WinApi.TextOut(graphics.DC, x - lineString.Length * charSizeX, y , lineString, lineString.Length);
                    WinApi.ExtTextOut(graphics.DC, x - lineString.Length * charSizeX, y, 0, lineString);

                    //                    Point point = new Point(x - lineString.Length * charSizeX, y);
                    //                    graphics.DrawText(lineString, ref point, Color.Silver);
                }

                sw.Start();

                WinApi.SetBkMode(graphics.DC, 0);

                int lineX;
                int start;
                int end;
                
                // draw charactors
                x = xOffset * charSizeX - clipRect.X;
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
                            xIncrement = tabSize - (lineX % tabSize);
                            WinApi.SelectObject(graphics.DC, tabPen);
                            WinApi.MoveToEx(graphics.DC, x + 2, y + charSizeY - 2, IntPtr.Zero);
                            WinApi.LineTo(graphics.DC, x - 2 + xIncrement * charSizeX, y + charSizeY - 2);
                            WinApi.MoveToEx(graphics.DC, x - 2 + xIncrement * charSizeX, y + charSizeY - 2, IntPtr.Zero);
                            WinApi.LineTo(graphics.DC, x - 2 + xIncrement * charSizeX, y + charSizeY - 8);

                        }
                        else if (ch == '\r')
                        {
                            WinApi.SelectObject(graphics.DC, crPen);
                            WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.9), (int)(y + charSizeY * 0.2), IntPtr.Zero);
                            WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.9), (int)(y + charSizeY * 0.6));
                            WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.2), (int)(y + charSizeY * 0.6), IntPtr.Zero);
                            WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.9), (int)(y + charSizeY * 0.6));
                            WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.4), (int)(y + charSizeY * 0.4), IntPtr.Zero);
                            WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.2), (int)(y + charSizeY * 0.6));
                            WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.4), (int)(y + charSizeY * 0.8), IntPtr.Zero);
                            WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.2), (int)(y + charSizeY * 0.6));
                        }
                        else if (ch == '\n')
                        {
                            if (i == 0 || document.GetCharAt(i - 1) != '\r')
                            {
                                WinApi.SelectObject(graphics.DC, lfPen);
                                WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.2), IntPtr.Zero);
                                WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8));
                                WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.4), (int)(y + charSizeY * 0.6), IntPtr.Zero);
                                WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8));
                                WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.8), (int)(y + charSizeY * 0.6), IntPtr.Zero);
                                WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8));
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

                            int colorNo = WinApi.GetColor(Style.ColorPallet[color]);

                            WinApi.SetTextColor(graphics.DC, colorNo);
                            //WinApi.TextOut(graphics.DC, x, y, sb.ToString(), sb.Length);
                            WinApi.ExtTextOut(graphics.DC, x, y, 0, sb.ToString());
//                            Point point = new Point(x, y);
//                            graphics.DrawText(sb.ToString(), ref point, Style.ColorPallet[color]);

                            xIncrement = sb.Length;
                            sb.Clear();
                            i--;
                        }
                        lineX = lineX + xIncrement;
                        x = x + charSizeX * xIncrement;
                    }
                }

                WinApi.SetBkMode(graphics.DC, 1);


                x = xOffset * charSizeX;
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
                            xIncrement = tabSize - (lineX % tabSize);
                        }

                        // selection
                        if (i >= document.SelectionStart && i < document.SelectionLast)
                        {
                            if (ch == '\t')
                            {   // tab
                                xIncrement = tabSize - (lineX % tabSize);
                                IntPtr pSource = WinApi.CreateCompatibleDC(graphics.DC);
                                IntPtr hbmp = selectionBitmap.GetHbitmap(Color.Black);
                                IntPtr pOrig = WinApi.SelectObject(pSource, hbmp);
                                for (int j = 0; j < xIncrement; j++)
                                {
                                    WinApi.AlphaBlend(
                                        graphics.DC, x + j * charSizeX, y, selectionBitmap.Width, selectionBitmap.Height,
                                        pSource, 0, 0, selectionBitmap.Width, selectionBitmap.Height,
                                        new WinApi.BLENDFUNCTION(WinApi.AC_SRC_OVER, 0, 0xff, WinApi.AC_SRC_ALPHA)
                                        );
                                }
                                IntPtr pNew = WinApi.SelectObject(pSource, pOrig);
                                WinApi.DeleteObject(pNew);
                                WinApi.DeleteObject(hbmp);
                                WinApi.DeleteDC(pSource);
                            }
                            else
                            {
                                IntPtr pSource = WinApi.CreateCompatibleDC(graphics.DC);
                                IntPtr hbmp = selectionBitmap.GetHbitmap(Color.Black);
                                IntPtr pOrig = WinApi.SelectObject(pSource, hbmp);
                                WinApi.AlphaBlend(
                                    graphics.DC, x, y, selectionBitmap.Width, selectionBitmap.Height,
                                    pSource, 0, 0, selectionBitmap.Width, selectionBitmap.Height,
                                    new WinApi.BLENDFUNCTION(WinApi.AC_SRC_OVER, 0, 0xff, WinApi.AC_SRC_ALPHA)
                                    );
                                IntPtr pNew = WinApi.SelectObject(pSource, pOrig);
                                WinApi.DeleteObject(pNew);
                                WinApi.DeleteObject(hbmp);
                                WinApi.DeleteDC(pSource);
                            }
                        }


                        // mark
                        if (document.GetMarkAt(i) != 0)
                        {
                            for (int mark = 7; mark >= 0 ; mark--)
                            {
                                //if ((document.GetMarkAt(i) & (1 << mark)) != 0) drawMarkGdi(hDC, x, y, mark);
                                if ((document.GetMarkAt(i) & (1 << mark)) != 0)
                                {
                                    IntPtr pSource = WinApi.CreateCompatibleDC(graphics.DC);
                                    IntPtr hbmp = markBitmap[mark].GetHbitmap(Color.Black);
                                    IntPtr pOrig = WinApi.SelectObject(pSource, hbmp);
                                    WinApi.AlphaBlend(
                                        graphics.DC, x, y, markBitmap[mark].Width, markBitmap[mark].Height,
                                        pSource, 0, 0, markBitmap[mark].Width, markBitmap[mark].Height,
                                        new WinApi.BLENDFUNCTION(WinApi.AC_SRC_OVER, 0, 0xff, WinApi.AC_SRC_ALPHA)
                                        );
                                    IntPtr pNew = WinApi.SelectObject(pSource, pOrig);
                                    WinApi.DeleteObject(pNew);
                                    WinApi.DeleteObject(hbmp);
                                    WinApi.DeleteDC(pSource);
                                }

                            }
                        }

                        // caret
                        if (i == document.CaretIndex & Editable)
                        {
                            IntPtr hrgn = WinApi.CreateRectRgn(x, y + 2, x + 2, y + charSizeY - 2);
                            IntPtr hbrush = WinApi.CreateSolidBrush(WinApi.GetColor(CarletColor));
                            WinApi.FillRgn(graphics.DC, hrgn, hbrush);
                            WinApi.DeleteObject(hbrush);
                            WinApi.DeleteObject(hrgn);
                            caretX = x;
                            caretY = y;
                        }

                        lineX = lineX + xIncrement;
                        x = x + charSizeX * xIncrement;
                    }
                }


                // carlet at EOF
                if (line == document.Lines && document.Length == document.CaretIndex && Editable)
                {
                    IntPtr hrgn = WinApi.CreateRectRgn(x, y + 2, x + 2, y + charSizeY - 2);
                    IntPtr hbrush = WinApi.CreateSolidBrush(WinApi.GetColor(CarletColor));
                    WinApi.FillRgn(graphics.DC, hrgn, hbrush);
                    WinApi.DeleteObject(hbrush);
                    WinApi.DeleteObject(hrgn);
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
            if (Editable)
            {
                WinApi.SelectObject(graphics.DC, tabPen);
                WinApi.MoveToEx(graphics.DC, xOffset*charSizeX - clipRect.X, caretY + charSizeY - clipRect.Y, IntPtr.Zero);
                WinApi.LineTo(graphics.DC, Width - clipRect.X, caretY + charSizeY - clipRect.Y);
            }

            //WinApi.SelectObject(hDC, oldPen);
            WinApi.DeleteObject(tabPen);
            WinApi.DeleteObject(crPen);
            WinApi.DeleteObject(lfPen);
            WinApi.DeleteObject((IntPtr)WinApi.SelectObject(graphics.DC, hOldFont));
            //g.ReleaseHdc(hDC);

            //sw.Stop();
            //System.Diagnostics.Debug.Print("draw : " + sw.Elapsed.TotalMilliseconds.ToString() + "ms");

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
