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
    public class DrawCache
    {
        public DrawCache(CodeTextbox codeTextBox)
        {
            this.codeTextbox = codeTextBox;
        }

        CodeTextbox codeTextbox;

        private static Primitive.IconImage plusIcon = new Primitive.IconImage(Properties.Resources.plus);
        private static Primitive.IconImage minusIcon = new Primitive.IconImage(Properties.Resources.minus);

        public int[] actualLineNumbers = new int[] { };
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

        public IntPtr tabPen;
        public IntPtr lfPen;
        public IntPtr crPen;
        public void ResizeDrawBuffer()
        {
            tabPen = Primitive.WinApi.CreatePen(0, 1, Primitive.WinApi.GetColor(codeTextbox.TabColor));
            lfPen = Primitive.WinApi.CreatePen(0, 1, Primitive.WinApi.GetColor(codeTextbox.LfColor));
            crPen = Primitive.WinApi.CreatePen(0, 1, Primitive.WinApi.GetColor(codeTextbox.CrColor));
            visibleLines = (int)Math.Ceiling((double)(codeTextbox.Height / charSizeY));
        }


        // character & mark graphics Cache
        public volatile bool ReGenarateBuffer = true;
        public Bitmap[] markBitmap = new Bitmap[8];
        public Bitmap plusBitmap;
        public Bitmap minusBitmap;
        public Bitmap selectionBitmap;

        public void CreateGraphicsCashe()
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

        public void DrawCr(Primitive.GraWin graphics,int x,int y)
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

        public void DrawLf(Primitive.GraWin graphics, int x, int y)
        {
            Primitive.WinApi.SelectObject(graphics.DC, lfPen);
            Primitive.WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.2), IntPtr.Zero);
            Primitive.WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8));
            Primitive.WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.4), (int)(y + charSizeY * 0.6), IntPtr.Zero);
            Primitive.WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8));
            Primitive.WinApi.MoveToEx(graphics.DC, (int)(x + charSizeX * 0.8), (int)(y + charSizeY * 0.6), IntPtr.Zero);
            Primitive.WinApi.LineTo(graphics.DC, (int)(x + charSizeX * 0.6), (int)(y + charSizeY * 0.8));
        }

        public void DrawTab(Primitive.GraWin graphics,int x,int y, int xIncrement)
        {
            Primitive.WinApi.SelectObject(graphics.DC, tabPen);
            Primitive.WinApi.MoveToEx(graphics.DC, x + 2, y + charSizeY - 2, IntPtr.Zero);
            Primitive.WinApi.LineTo(graphics.DC, x - 2 + xIncrement * charSizeX, y + charSizeY - 2);
            Primitive.WinApi.MoveToEx(graphics.DC, x - 2 + xIncrement * charSizeX, y + charSizeY - 2, IntPtr.Zero);
            Primitive.WinApi.LineTo(graphics.DC, x - 2 + xIncrement * charSizeX, y + charSizeY - 8);
        }

        public void DrawCarlet(Primitive.GraWin graphics, int x, int y)
        {
            IntPtr hrgn = Primitive.WinApi.CreateRectRgn(x, y + 2, x + 2, y + charSizeY - 2);
            IntPtr hbrush = Primitive.WinApi.CreateSolidBrush(Primitive.WinApi.GetColor(codeTextbox.CarletColor));
            Primitive.WinApi.FillRgn(graphics.DC, hrgn, hbrush);
            Primitive.WinApi.DeleteObject(hbrush);
            Primitive.WinApi.DeleteObject(hrgn);
        }

        public void DrawMark(Primitive.GraWin graphics, int x, int y,int mark)
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

        public void DrawSelection(Primitive.GraWin graphics, int x, int y)
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

        public void DrawSelection(Primitive.GraWin graphics, int x, int y,int xIncrement)
        {
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

        public void DrawPuls(Primitive.GraWin graphics, int x, int y)
        {
            IntPtr hsrc = Primitive.WinApi.CreateCompatibleDC(graphics.DC);
            IntPtr hbmp = plusBitmap.GetHbitmap();
            IntPtr porg = Primitive.WinApi.SelectObject(hsrc, hbmp);
            Primitive.WinApi.BitBlt(graphics.DC, x, y, charSizeY, charSizeY, hsrc, 0, 0, (uint)Primitive.WinApi.TernaryRasterOperations.SRCCOPY);
            Primitive.WinApi.DeleteObject(porg);
            Primitive.WinApi.DeleteObject(hbmp);
            Primitive.WinApi.DeleteDC(hsrc);
        }

        public void DrawMinus(Primitive.GraWin graphics, int x, int y)
        {
            IntPtr hsrc = Primitive.WinApi.CreateCompatibleDC(graphics.DC);
            IntPtr hbmp = minusBitmap.GetHbitmap();
            IntPtr porg = Primitive.WinApi.SelectObject(hsrc, hbmp);
            Primitive.WinApi.BitBlt(graphics.DC, x, y, charSizeY, charSizeY, hsrc, 0, 0, (uint)Primitive.WinApi.TernaryRasterOperations.SRCCOPY);
            Primitive.WinApi.DeleteObject(porg);
            Primitive.WinApi.DeleteObject(hbmp);
            Primitive.WinApi.DeleteDC(hsrc);
        }

    }
}
