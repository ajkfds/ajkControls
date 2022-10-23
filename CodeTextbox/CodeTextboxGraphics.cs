using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace ajkControls.CodeTextbox
{
    public class CodeTextboxGraphics : BufferedGraphics
    {
        IntPtr tabPen = IntPtr.Zero;
        IntPtr lfPen = IntPtr.Zero;
        IntPtr crPen = IntPtr.Zero;
        IntPtr carletLinePen = IntPtr.Zero;
        IntPtr hFont = IntPtr.Zero;
        public CodeTextboxGraphics(IntPtr hWnd) : base(hWnd)
        {
            IntPtr lfPen = WinApi.CreatePen(0, 1, WinApi.GetColor(lfColor));
            IntPtr crPen = WinApi.CreatePen(0, 1, WinApi.GetColor(crColor));
        }

        public override void Dispose()
        {
            deleteObject(tabPen);
            deleteObject(lfPen);
            deleteObject(crPen);
            deleteObject(carletLinePen);
            deleteObject(hFont);
            base.Dispose();
        }

        private Font font;
        public Font Font
        {
            get { return font; }
            set { 
                font = value;
                deleteObject(hFont); 
                hFont = font.ToHfont();
            }
        }

        private Color tabColor;
        public Color TabColor
        {
            get { return tabColor; }
            set {
                tabColor = value;
                deleteObject(tabPen);
                tabPen = WinApi.CreatePen(0, 1, WinApi.GetColor(tabColor));
            }
        }

        private Color lfColor;
        public Color LfColor
        {
            get { return lfColor; }
            set { 
                lfColor = value;
                deleteObject(lfPen);
                lfPen = WinApi.CreatePen(0, 1, WinApi.GetColor(lfColor));
            }
        }

        private Color crColor;
        public Color CrColor
        {
            get { return crColor; }
            set { 
                crColor = value;
                deleteObject(crPen);
                crPen = WinApi.CreatePen(0, 1, WinApi.GetColor(crColor));
            }
        }

        private Color carletLineColor;
        public Color CarletLineColor
        {
            get { return carletLineColor; }
            set { 
                carletLineColor = value;
                deleteObject(carletLinePen);
                carletLinePen = WinApi.CreatePen(0, 1, WinApi.GetColor(carletLineColor));
            }
        }

        private void deleteObject(IntPtr gdiObj)
        {
            if (gdiObj != IntPtr.Zero)
            {
                WinApi.DeleteObject(gdiObj);
            }
        }

        public void DrawCr(int x,int y,int charSizeX,int charSizeY)
        {
            int ox = x - Offset.X;
            int oy = y - Offset.Y;

            WinApi.SelectObject(DC, crPen);
            WinApi.MoveToEx(DC, (int)(ox + charSizeX * 0.9), (int)(oy + charSizeY * 0.2), IntPtr.Zero);
            WinApi.LineTo(DC, (int)(ox + charSizeX * 0.9), (int)(oy + charSizeY * 0.6));
            WinApi.MoveToEx(DC, (int)(ox + charSizeX * 0.2), (int)(oy + charSizeY * 0.6), IntPtr.Zero);
            WinApi.LineTo(DC, (int)(ox + charSizeX * 0.9), (int)(oy + charSizeY * 0.6));
            WinApi.MoveToEx(DC, (int)(ox + charSizeX * 0.4), (int)(oy + charSizeY * 0.4), IntPtr.Zero);
            WinApi.LineTo(DC, (int)(ox + charSizeX * 0.2), (int)(oy + charSizeY * 0.6));
            WinApi.MoveToEx(DC, (int)(ox + charSizeX * 0.4), (int)(oy + charSizeY * 0.8), IntPtr.Zero);
            WinApi.LineTo(DC, (int)(ox + charSizeX * 0.2), (int)(oy + charSizeY * 0.6));
        }

        public void DrawTab(int x, int y, int xIncrement,int charSizeX, int charSizeY)
        {
            int ox = x - Offset.X;
            int oy = y - Offset.Y;

            WinApi.SelectObject(DC, tabPen);
            WinApi.MoveToEx(DC, ox + 2, oy + charSizeY - 2, IntPtr.Zero);
            WinApi.LineTo(DC, ox - 2 + xIncrement * charSizeX, oy + charSizeY - 2);
            WinApi.MoveToEx(DC, ox - 2 + xIncrement * charSizeX, oy + charSizeY - 2, IntPtr.Zero);
            WinApi.LineTo(DC, ox - 2 + xIncrement * charSizeX, oy + charSizeY - 8);
        }

        public void DrawLf(int x, int y, int charSizeX, int charSizeY)
        {
            int ox = x - Offset.X;
            int oy = y - Offset.Y;

            WinApi.SelectObject(DC, lfPen);
            WinApi.MoveToEx(DC, (int)(ox + charSizeX * 0.6), (int)(oy + charSizeY * 0.2), IntPtr.Zero);
            WinApi.LineTo(DC, (int)(ox + charSizeX * 0.6), (int)(oy + charSizeY * 0.8));
            WinApi.MoveToEx(DC, (int)(ox + charSizeX * 0.4), (int)(oy + charSizeY * 0.6), IntPtr.Zero);
            WinApi.LineTo(DC, (int)(ox + charSizeX * 0.6), (int)(oy + charSizeY * 0.8));
            WinApi.MoveToEx(DC, (int)(ox + charSizeX * 0.8), (int)(oy + charSizeY * 0.6), IntPtr.Zero);
            WinApi.LineTo(DC, (int)(ox + charSizeX * 0.6), (int)(oy + charSizeY * 0.8));

        }

        public void DrawText(int x,int y,string text,Color color)
        {
            int colorNo = WinApi.GetColor(color);
            WinApi.SetTextColor(DC, colorNo);
            WinApi.ExtTextOut(DC, x - Offset.X, y - Offset.Y, 0, text);
        }
    }
}
