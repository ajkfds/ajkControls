using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ajkControls
{
    public static class WinApi
    {

        #region Constants
        public const int WM_PAINT = 0x000F;
        public const int WM_HSCROLL = 0x0114;
        public const int WM_VSCROLL = 0x0115;

        public const int WM_KEYFIRST = 0x0100;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_CHAR = 0x0102;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SYSKEYUP = 0x0105;
        public const int WM_SYSCHAR = 0x0106;
        public const int WM_UNICHAR = 0x0109;
        public const int WM_KEYLAST = 0x0109;

        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_RBUTTONDBLCLK = 0x0206;
        public const int WM_MOUSEWHEEL = 0x020a;

        public const int WM_IME_STARTCOMPOSITION = 0x010D;
        public const int WM_IME_ENDCOMPOSITION = 0x010E;
        public const int WM_IME_COMPOSITION = 0x010F;
        public const int WM_IME_NOTIFY = 0x0282;
        public const int WM_IME_CHAR = 0x0286;
        public const int WM_IME_REQUEST = 0x0288;
        public const int IMR_RECONVERTSTRING = 0x0004;
        public const int SCS_QUERYRECONVERTSTRING = 0x00020000;
        public const int SCS_SETRECONVERTSTRING = 0x00010000;
        public const int SCS_CAP_SETRECONVERTSTRING = 0x00000004;
        public const int IME_PROP_UNICODE = 0x00080000;
        public const int IGP_PROPERTY = 0x00000004;
        public const int IGP_SETCOMPSTR = 0x00000014;

        public const long WS_HSCROLL = 0x00100000L;
        public const long WS_VSCROLL = 0x00200000L;
        public const long WS_BORDER = 0x00800000L;
        public const long WS_EX_CLIENTEDGE = 0x00000200L;
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int SWP_FRAMECHANGED = 0x0020;

        public const int SB_LINEUP = 0;
        public const int SB_LINEDOWN = 1;
        public const int SB_PAGEUP = 2;
        public const int SB_PAGEDOWN = 3;
        public const int SB_THUMBPOSITION = 4;
        public const int SB_THUMBTRACK = 5;
        public const int SB_TOP = 6;
        public const int SB_BOTTOM = 7;
        public const int SB_ENDSCROLL = 8;

        public const UInt32 CF_TEXT = 1;
        public const UInt32 CF_UNICODETEXT = 13;
        public const UInt32 CF_PRIVATEFIRST = 0x200;
        public const UInt32 CF_PRIVATELAST = 0x2ff;

        public const Int32 GCS_COMPREADSTR = 0x0001;
        public const Int32 GCS_COMPSTR = 0x0008;
        public const Int32 GCS_RESULTSTR = 0x0800;
        public const Int32 SCS_SETSTR = (GCS_COMPREADSTR | GCS_COMPSTR);

        const int SIF_RANGE = 0x01;
        const int SIF_PAGE = 0x02;
        const int SIF_POS = 0x04;
        const int SIF_DISABLENOSCROLL = 0x08;
        const int SIF_TRACKPOS = 0x10;
        const int SW_INVALIDATE = 0x0002;

        const uint TA_LEFT = 0;
        const uint TA_RIGHT = 2;
        const uint TA_CENTER = 6;
        const uint TA_NOUPDATECP = 0;
        const uint TA_TOP = 0;
        const int PATINVERT = 0x005A0049;
        #endregion

        #region Types
        public delegate IntPtr WNDPROC(IntPtr window, UInt32 message, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        struct SIZE
        {
            public Int32 width, height;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public POINT(int x, int y) { this.x = x; this.y = y; }
            public POINT(System.Drawing.Point pt) { x = pt.X; y = pt.Y; }
            public Int32 x, y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public RECT(System.Drawing.Rectangle rect)
            {
                left = rect.Left;
                top = rect.Top;
                right = rect.Right;
                bottom = rect.Bottom;
            }
            public Int32 left, top, right, bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public IntPtr hDC;
            public Int32 bErase;
            public RECT paint;
            public Int32 bRestore;
            public Int32 bIncUpdate;
            public Int64 reserved0, reserved1, reserved2, reserved3, reserved4;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SCROLLINFO
        {
            public UInt32 size;
            public UInt32 mask;
            public Int32 min;
            public Int32 max;
            public UInt32 page;
            public Int32 pos;
            public Int32 trackPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct COMPOSITIONFORM
        {
            public UInt32 style;
            public POINT currentPos;
            public RECT area;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct TEXTMETRIC
        {
            public Int32 height;
            public Int32 ascent;
            public Int32 descent;
            public Int32 internalLeading;
            public Int32 externalLeading;
            public Int32 aveCharWidth;
            public Int32 maxCharWidth;
            public Int32 weight;
            public Int32 overhang;
            public Int32 digitizedAspectX;
            public Int32 digitizedAspectY;
            public char firstChar;
            public char lastChar;
            public char defaultChar;
            public char breakChar;
            public byte italic;
            public byte underlined;
            public byte struckOut;
            public byte pitchAndFamily;
            public byte charSet;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct LOGFONTW
        {
            public Int32 height;
            public Int32 width;
            public Int32 escapement;
            public Int32 orientation;
            public Int32 weight;
            public byte italic;
            public byte underline;
            public byte strikeOut;
            public byte charSet;
            public byte outPrecision;
            public byte clipPrecision;
            public byte quality;
            public byte pitchAndFamily;
            public fixed char faceName[32];
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct RECONVERTSTRING
        {
            /// <summary>Size of this instance.</summary>
            public UInt32 dwSize;

            /// <summary>Version (must be 0).</summary>
            public UInt32 dwVersion;

            /// <summary>Length of the string given to IME.</summary>
            public UInt32 dwStrLen;

            /// <summary>
            /// Byte-offset of the string given to IME
            /// from the memory address of this structure.
            /// </summary>
            public UInt32 dwStrOffset;

            /// <summary>Length of the string that will be able to be reconverted.</summary>
            public UInt32 dwCompStrLen;

            /// <summary>
            /// Byte-offset of the string that will be able to be reconverted
            /// from the start position of where specified with dwStrOffset.
            /// </summary>
            public UInt32 dwCompStrOffset;

            /// <summary>Length of the exact string that will be reconverted.</summary>
            public UInt32 dwTargetStrLen;

            /// <summary>
            /// Byte-offset of the exact string that will be reconverted
            /// from the start position of where specified with dwStrOffset.
            /// </summary>
            public UInt32 dwTargetStrOffset;
        }
        #endregion
        /*
        IntPtr hDC = e.Graphics.GetHdc();
        IntPtr hFont = this.Font.ToHfont();
        IntPtr hOldFont = (IntPtr)SelectObject(hDC, hFont);

        SetTextColor(hDC,colorNo);
        TextOut(hDC, x, y, sb.ToString(), sb.Length);

        DeleteObject((IntPtr)SelectObject(hDC, hOldFont));
        e.Graphics.ReleaseHdc(hDC);
        */
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public extern static int TextOut(IntPtr hdc, int x, int y, string text, int length);

        [DllImport("gdi32.dll")]
        public static extern int SelectObject(IntPtr hdc, IntPtr hgdiObj);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern int SetTextColor(IntPtr hdc, int color);

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        public static extern bool FillRgn(IntPtr hdc, IntPtr hrgn, IntPtr hbr);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect,
            int nBottomRect);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(uint crColor);

        [DllImport("gdi32.dll")]
        public static extern uint SetBkColor(IntPtr hdc, int crColor);

        [DllImport("gdi32.dll")]
        public unsafe static extern UInt32 MoveToEx(IntPtr hdc, Int32 x, Int32 y, IntPtr nul);

        [DllImport("gdi32.dll")]
        public unsafe static extern Int32 LineTo(IntPtr hdc, Int32 x, Int32 y);

        [DllImport("gdi32.dll")]
        public unsafe static extern Int32 SetPixel(IntPtr hdc, Int32 x, Int32 y, Int32 color);

        [DllImport("gdi32.dll")]
        public unsafe static extern Int32 Rectangle(IntPtr hdc, Int32 left, Int32 top, Int32 right, Int32 bottom);

        [DllImport("gdi32.dll")]
        public unsafe static extern IntPtr CreatePen(Int32 style, Int32 width, Int32 color);

        [DllImport("gdi32.dll")]
        public unsafe static extern IntPtr CreateSolidBrush(Int32 color);
        /*
        Bitmap bmp = new Bitmap("ball.bmp");
     
        IntPtr hdc = e.Graphics.GetHdc();
        IntPtr hsrc = CreateCompatibleDC(hdc);
        IntPtr porg = SelectObject(hsrc, bmp.GetHbitmap());

        BitBlt(hdc, 80, 30, bmp.Width, bmp.Height, hsrc, 0, 0, TernaryRasterOperations.SRCCOPY);

        DeleteDC(hsrc);
        e.Graphics.ReleaseHdc(hdc);
        */
        public enum TernaryRasterOperations : uint
        {
            SRCCOPY = 0x00CC0020,
            SRCPAINT = 0x00EE0086,
            SRCAND = 0x008800C6,
            SRCINVERT = 0x00660046,
            SRCERASE = 0x00440328,
            NOTSRCCOPY = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            MERGECOPY = 0x00C000CA,
            MERGEPAINT = 0x00BB0226,
            PATCOPY = 0x00F00021,
            PATPAINT = 0x00FB0A09,
            PATINVERT = 0x005A0049,
            DSTINVERT = 0x00550009,
            BLACKNESS = 0x00000042,
            WHITENESS = 0x00FF0062,
            CAPTUREBLT = 0x40000000
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight,
            IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern uint SetPixel(IntPtr hdc, int X, int Y, uint crColor);



    }
}
