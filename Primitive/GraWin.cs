using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Control = System.Windows.Forms.Control;
using SystemInformation = System.Windows.Forms.SystemInformation;
using Marshal = System.Runtime.InteropServices.Marshal;

namespace ajkControls.Primitive
{
	public class GraWin
	{
		#region Fields
		IntPtr _Window = IntPtr.Zero;
		IntPtr _DC = IntPtr.Zero;
		IntPtr _MemDC = IntPtr.Zero;
		Size _MemDcSize;
		Point _Offset = Point.Empty;
		IntPtr _MemBmp = IntPtr.Zero;
		IntPtr _OrgMemBmp;
		int _ForeColor;
		IntPtr _Pen = IntPtr.Zero;
		IntPtr _Brush = IntPtr.Zero;
		IntPtr _NullPen = IntPtr.Zero;
		IntPtr _Font = IntPtr.Zero;
		#endregion

		#region Init / Dispose
		public GraWin(IntPtr hWnd, FontInfo fontInfo)
		{
			_Window = hWnd;
			_DC = Primitive.WinApi.GetDC(_Window);
			FontInfo = fontInfo;
		}

		public void Dispose()
		{
			Primitive.WinApi.SelectObject(_MemDC, _OrgMemBmp);

			// release DC
			Primitive.WinApi.ReleaseDC(_Window, _DC);
			Primitive.WinApi.DeleteDC(_MemDC);

			// free objects lastly used
			Utl.SafeDeleteObject(_Pen);
			Utl.SafeDeleteObject(_Brush);
			Utl.SafeDeleteObject(_NullPen);
			Utl.SafeDeleteObject(_Font);
		}
		#endregion

		#region Off-screen Rendering
		/// <summary>
		/// Begin using off-screen buffer and cache drawing which will be done after.
		/// </summary>
		/// <param name="paintRect">painting area (used for creating off-screen buffer).</param>
		public void BeginPaint(Rectangle paintRect)
		{

			// create offscreen from the window DC
			_MemDC = Primitive.WinApi.CreateCompatibleDC(_DC);
			_MemBmp = Primitive.WinApi.CreateCompatibleBitmap(_DC, paintRect.Width, paintRect.Height);
			_Offset = paintRect.Location;
			_MemDcSize = paintRect.Size;
			_OrgMemBmp = Primitive.WinApi.SelectObject(_MemDC, _MemBmp);
		}

		/// <summary>
		/// End using off-screen buffer and flush all drawing results.
		/// </summary>
		public void EndPaint()
		{
			const uint SRCCOPY = 0x00CC0020;

			// flush cached graphic update
			Primitive.WinApi.BitBlt(_DC, _Offset.X, _Offset.Y, _MemDcSize.Width, _MemDcSize.Height,
				_MemDC, 0, 0, SRCCOPY);
			RemoveClipRect();

			// dispose resources used in off-screen rendering
			Primitive.WinApi.SelectObject(_MemDC, _OrgMemBmp);
			Primitive.WinApi.DeleteDC(_MemDC);
			_MemDC = IntPtr.Zero;
			Utl.SafeDeleteObject(_MemBmp);
			_MemBmp = IntPtr.Zero;

			// reset graphic coordinate offset
			_Offset.X = _Offset.Y = 0;
		}
		#endregion

		#region Graphic Setting
		/// <summary>
		/// Font used for drawing/measuring text.
		/// </summary>
		public FontInfo FontInfo
		{
			set
			{

				// delete old font
				Utl.SafeDeleteObject(_Font);

				// create font handle from Font instance of .NET
				unsafe
				{
					Primitive.WinApi.LOGFONTW logicalFont;

					Primitive.WinApi.CreateLogFont(IntPtr.Zero, value, out logicalFont);

					//// apply anti-alias method that user prefered
					//if (UserPref.Antialias == Antialias.None)
					//	logicalFont.quality = 3; // NONANTIALIASED_QUALITY
					//else if (UserPref.Antialias == Antialias.Gray)
					//	logicalFont.quality = 4; // ANTIALIASED_QUALITY
					//else if (UserPref.Antialias == Antialias.Subpixel)
					//	logicalFont.quality = 5; // CLEARTYPE_QUALITY

					_Font = Primitive.WinApi.CreateFontIndirectW(&logicalFont);
				}
			}
		}

		/// <summary>
		/// Font used for drawing/measuring text.
		/// </summary>
		public Font Font
		{
			set { FontInfo = new FontInfo(value); }
		}

		/// <summary>
		/// Foreground color used by drawing APIs.
		/// </summary>
		public Color ForeColor
		{
			set
			{
				Utl.SafeDeleteObject(_Pen);
				_ForeColor = (value.R) | (value.G << 8) | (value.B << 16);
				_Pen = Primitive.WinApi.CreatePen(0, 1, _ForeColor);
			}
		}

		/// <summary>
		/// Background color used by drawing APIs.
		/// </summary>
		public Color BackColor
		{
			set
			{
				Utl.SafeDeleteObject(_Brush);
				int colorRef = (value.R) | (value.G << 8) | (value.B << 16);
				_Brush = Primitive.WinApi.CreateSolidBrush(colorRef);
			}
		}

		/// <summary>
		/// Select specified rectangle as a clipping region.
		/// </summary>
		public void SetClipRect(Rectangle clipRect)
		{
			unsafe
			{
				// make RECT structure
				Primitive.WinApi.RECT r = new Primitive.WinApi.RECT();
				r.left = clipRect.X - _Offset.X;
				r.top = clipRect.Y - _Offset.Y;
				r.right = r.left + clipRect.Width;
				r.bottom = r.top + clipRect.Height;

				// create rectangle region and select it as a clipping region
				IntPtr clipRegion = Primitive.WinApi.CreateRectRgnIndirect(&r);
				Primitive.WinApi.SelectClipRgn(DC, clipRegion);
				Primitive.WinApi.DeleteObject(clipRegion); // SelectClipRgn copies given region thus we can delete this
			}
		}

		/// <summary>
		/// Remove currently selected clipping region from the offscreen buffer.
		/// </summary>
		public void RemoveClipRect()
		{
			Primitive.WinApi.SelectClipRgn(DC, IntPtr.Zero);
		}
		#endregion

		#region Text Rendering
		/// <summary>
		/// Draws a text.
		/// </summary>
		public void DrawText(string text, ref Point position, Color foreColor)
		{
			const int TRANSPARENT = 1;
			IntPtr oldFont, newFont;
			Int32 oldForeColor;
			int x, y;

			x = position.X - _Offset.X;
			y = position.Y - _Offset.Y;

			newFont = _Font;
			oldFont = Primitive.WinApi.SelectObject(DC, newFont);
			oldForeColor = Primitive.WinApi.SetTextColor(DC, foreColor);

			Primitive.WinApi.SetTextAlign(DC, false);
			Primitive.WinApi.SetBkMode(DC, TRANSPARENT);
			Primitive.WinApi.ExtTextOut(DC, x, y, 0, text);

			Primitive.WinApi.SetTextColor(DC, oldForeColor);
			Primitive.WinApi.SelectObject(DC, oldFont);
		}

		/// <summary>
		/// Measures graphical size of the a text.
		/// </summary>
		/// <param name="text">text to measure</param>
		/// <returns>size of the text in the graphic device context</returns>
		public Size MeasureText(string text)
		{
			int dummy;
			return MeasureText(text, Int32.MaxValue, out dummy);
		}

		/// <summary>
		/// Measures graphical size of the a text within the specified clipping width.
		/// </summary>
		/// <param name="text">text to measure</param>
		/// <param name="clipWidth">width of the clipping area for rendering text (in pixel unit if the context is screen)</param>
		/// <param name="drawableLength">count of characters which could be drawn within the clipping area width</param>
		/// <returns>size of the text in the graphic device context</returns>
		public Size MeasureText(string text, int clipWidth, out int drawableLength)
		{
			IntPtr oldFont;
			Size size;
			int[] extents = new int[text.Length];

			oldFont = Primitive.WinApi.SelectObject(DC, _Font); // measuring do not need to be done in offscreen buffer.

			// calculate width of given text and graphical distance from left end to where the each char is at
			size = Primitive.WinApi.GetTextExtent(DC, text, text.Length, clipWidth, out drawableLength, out extents);

			// calculate width of the drawable part
			if (drawableLength == 0)
			{
				// no chars can be written in clipping area.
				// so width of the drawable part is zero.
				size.Width = 0;
			}
			else
			{
				// there are chars which can be written in clipping area.
				// so get distance of the char at right most; this is the width of the drawable part of the text
				// (note: array of extents will always be filled by GetTextExtentExPoint API in WinCE.)
				if (drawableLength < extents.Length)
				{
					size.Width = extents[drawableLength - 1];
				}
			}

			// (MUST DO AFTER GETTING EXTENTS)
			// extend length if it ends with in a grapheme cluster
			//if (0 < drawableLength && Document.IsNotDividableIndex(text, drawableLength))
			//{
			//	do
			//	{
			//		drawableLength++;
			//	}
			//	while (Document.IsNotDividableIndex(text, drawableLength));
			//}

			Primitive.WinApi.SelectObject(DC, oldFont);

			return size;
		}
		#endregion

		#region Graphic Drawing
		/// <summary>
		/// Draws a line with foreground color.
		/// Note that the point where the line extends to will also be painted.
		/// </summary>
		public void DrawLine(int fromX, int fromY, int toX, int toY)
		{
			IntPtr oldPen;

			fromX -= _Offset.X;
			fromY -= _Offset.Y;
			toX -= _Offset.X;
			toY -= _Offset.Y;

			oldPen = Primitive.WinApi.SelectObject(DC, _Pen);

			Primitive.WinApi.MoveToEx(DC, fromX, fromY, IntPtr.Zero);
			Primitive.WinApi.LineTo(DC, toX, toY);
			Primitive.WinApi.SetPixel(DC, toX, toY, _ForeColor);

			Primitive.WinApi.SelectObject(DC, oldPen);
		}

		/// <summary>
		/// Draws a rectangle with foreground color.
		/// Note that right and bottom edge will also be painted.
		/// </summary>
		public void DrawRectangle(int x, int y, int width, int height)
		{
			IntPtr oldPen;

			x -= _Offset.X;
			y -= _Offset.Y;

			unsafe
			{
				Primitive.WinApi.POINT[] points = new Primitive.WinApi.POINT[5];
				points[0] = new Primitive.WinApi.POINT(x, y);
				points[1] = new Primitive.WinApi.POINT(x + width, y);
				points[2] = new Primitive.WinApi.POINT(x + width, y + height);
				points[3] = new Primitive.WinApi.POINT(x, y + height);
				points[4] = new Primitive.WinApi.POINT(x, y);

				oldPen = Primitive.WinApi.SelectObject(DC, _Pen);

				fixed (Primitive.WinApi.POINT* p = points)
				{
					Primitive.WinApi.Polyline(DC, p, 5);
				}

				Primitive.WinApi.SelectObject(DC, oldPen);
			}
		}

		/// <summary>
		/// Fills a rectangle with background color.
		/// Note that right and bottom edge will also be painted.
		/// </summary>
		public void FillRectangle(int x, int y, int width, int height)
		{
			IntPtr oldPen, oldBrush;

			x -= _Offset.X;
			y -= _Offset.Y;

			oldPen = Primitive.WinApi.SelectObject(DC, NullPen);
			oldBrush = Primitive.WinApi.SelectObject(DC, _Brush);

#if !PocketPC
			Primitive.WinApi.Rectangle(DC, x, y, x + width + 1, y + height + 1);
#else
			Primitive.WinApi.Rectangle( DC, x, y, x+width, y+height );
#endif

			Primitive.WinApi.SelectObject(DC, oldPen);
			Primitive.WinApi.SelectObject(DC, oldBrush);
		}
		#endregion

		#region Utilities
		IntPtr NullPen
		{
			get
			{
				const int PS_NULL = 5;
				if (_NullPen == IntPtr.Zero)
				{
					_NullPen = Primitive.WinApi.CreatePen(PS_NULL, 0, 0);
				}
				return _NullPen;
			}
		}

		public IntPtr DC
		{
			get
			{
				if (_MemDC != IntPtr.Zero)
					return _MemDC;
				else
					return _DC;
			}
		}

		class Utl
		{
			public static void SafeDeleteObject(IntPtr gdiObj)
			{
				if (gdiObj != IntPtr.Zero)
				{
					Primitive.WinApi.DeleteObject(gdiObj);
				}
			}
		}
		#endregion
	}
}
