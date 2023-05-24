using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace ajkControls.CodeTextbox
{
    public class CodeTextboxGraphics : Primitive.BufferedGraphics
    {
        IntPtr tabPen = IntPtr.Zero;
        IntPtr lfPen = IntPtr.Zero;
        IntPtr crPen = IntPtr.Zero;
        IntPtr carletLinePen = IntPtr.Zero;
        IntPtr hFont = IntPtr.Zero;
        public CodeTextboxGraphics(IntPtr hWnd) : base(hWnd)
        {
            IntPtr lfPen = Primitive.WinApi.CreatePen(0, 1, Primitive.WinApi.GetColor(lfColor));
            IntPtr crPen = Primitive.WinApi.CreatePen(0, 1, Primitive.WinApi.GetColor(crColor));
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
                tabPen = Primitive.WinApi.CreatePen(0, 1, Primitive.WinApi.GetColor(tabColor));
            }
        }

        private Color lfColor;
        public Color LfColor
        {
            get { return lfColor; }
            set { 
                lfColor = value;
                deleteObject(lfPen);
                lfPen = Primitive.WinApi.CreatePen(0, 1, Primitive.WinApi.GetColor(lfColor));
            }
        }

        private Color crColor;
        public Color CrColor
        {
            get { return crColor; }
            set { 
                crColor = value;
                deleteObject(crPen);
                crPen = Primitive.WinApi.CreatePen(0, 1, Primitive.WinApi.GetColor(crColor));
            }
        }

        private Color carletLineColor;
        public Color CarletLineColor
        {
            get { return carletLineColor; }
            set { 
                carletLineColor = value;
                deleteObject(carletLinePen);
                carletLinePen = Primitive.WinApi.CreatePen(0, 1, Primitive.WinApi.GetColor(carletLineColor));
            }
        }

        private void deleteObject(IntPtr gdiObj)
        {
            if (gdiObj != IntPtr.Zero)
            {
                Primitive.WinApi.DeleteObject(gdiObj);
            }
        }

        public void DrawCr(int x,int y,int charSizeX,int charSizeY)
        {
            int ox = x - Offset.X;
            int oy = y - Offset.Y;

            Primitive.WinApi.SelectObject(DC, crPen);
            Primitive.WinApi.MoveToEx(DC, (int)(ox + charSizeX * 0.9), (int)(oy + charSizeY * 0.2), IntPtr.Zero);
            Primitive.WinApi.LineTo(DC, (int)(ox + charSizeX * 0.9), (int)(oy + charSizeY * 0.6));
            Primitive.WinApi.MoveToEx(DC, (int)(ox + charSizeX * 0.2), (int)(oy + charSizeY * 0.6), IntPtr.Zero);
            Primitive.WinApi.LineTo(DC, (int)(ox + charSizeX * 0.9), (int)(oy + charSizeY * 0.6));
            Primitive.WinApi.MoveToEx(DC, (int)(ox + charSizeX * 0.4), (int)(oy + charSizeY * 0.4), IntPtr.Zero);
            Primitive.WinApi.LineTo(DC, (int)(ox + charSizeX * 0.2), (int)(oy + charSizeY * 0.6));
            Primitive.WinApi.MoveToEx(DC, (int)(ox + charSizeX * 0.4), (int)(oy + charSizeY * 0.8), IntPtr.Zero);
            Primitive.WinApi.LineTo(DC, (int)(ox + charSizeX * 0.2), (int)(oy + charSizeY * 0.6));
        }

        public void DrawTab(int x, int y, int xIncrement,int charSizeX, int charSizeY)
        {
            int ox = x - Offset.X;
            int oy = y - Offset.Y;

            Primitive.WinApi.SelectObject(DC, tabPen);
            Primitive.WinApi.MoveToEx(DC, ox + 2, oy + charSizeY - 2, IntPtr.Zero);
            Primitive.WinApi.LineTo(DC, ox - 2 + xIncrement * charSizeX, oy + charSizeY - 2);
            Primitive.WinApi.MoveToEx(DC, ox - 2 + xIncrement * charSizeX, oy + charSizeY - 2, IntPtr.Zero);
            Primitive.WinApi.LineTo(DC, ox - 2 + xIncrement * charSizeX, oy + charSizeY - 8);
        }

        public void DrawLf(int x, int y, int charSizeX, int charSizeY)
        {
            int ox = x - Offset.X;
            int oy = y - Offset.Y;

            Primitive.WinApi.SelectObject(DC, lfPen);
            Primitive.WinApi.MoveToEx(DC, (int)(ox + charSizeX * 0.6), (int)(oy + charSizeY * 0.2), IntPtr.Zero);
            Primitive.WinApi.LineTo(DC, (int)(ox + charSizeX * 0.6), (int)(oy + charSizeY * 0.8));
            Primitive.WinApi.MoveToEx(DC, (int)(ox + charSizeX * 0.4), (int)(oy + charSizeY * 0.6), IntPtr.Zero);
            Primitive.WinApi.LineTo(DC, (int)(ox + charSizeX * 0.6), (int)(oy + charSizeY * 0.8));
            Primitive.WinApi.MoveToEx(DC, (int)(ox + charSizeX * 0.8), (int)(oy + charSizeY * 0.6), IntPtr.Zero);
            Primitive.WinApi.LineTo(DC, (int)(ox + charSizeX * 0.6), (int)(oy + charSizeY * 0.8));

        }

        public void DrawText(int x,int y,string text,Color color)
        {
            int colorNo = Primitive.WinApi.GetColor(color);
            Primitive.WinApi.SetTextColor(DC, colorNo);
            Primitive.WinApi.ExtTextOut(DC, x - Offset.X, y - Offset.Y, 0, text);
        }
    }
}
