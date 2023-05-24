using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ajkControls.Primitive
{
    public class BufferedGraphics
    {
		#region Fields
		IntPtr _Window = IntPtr.Zero;
		IntPtr _DC = IntPtr.Zero;
		IntPtr _MemDC = IntPtr.Zero;
		Size _MemDcSize;
		public Point Offset = Point.Empty;
		IntPtr _MemBmp = IntPtr.Zero;
		IntPtr _OrgMemBmp;
		#endregion

		#region Init / Dispose
		public BufferedGraphics(IntPtr hWnd)
		{
			_Window = hWnd;
			_DC = Primitive.WinApi.GetDC(_Window);
		}

		public virtual void Dispose()
		{
			Primitive.WinApi.SelectObject(_MemDC, _OrgMemBmp);

			// release DC
			Primitive.WinApi.ReleaseDC(_Window, _DC);
			Primitive.WinApi.DeleteDC(_MemDC);

			// free objects lastly used
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
			Offset = paintRect.Location;
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
			Primitive.WinApi.BitBlt(_DC, Offset.X, Offset.Y, _MemDcSize.Width, _MemDcSize.Height,
				_MemDC, 0, 0, SRCCOPY);
			Primitive.WinApi.SelectClipRgn(DC, IntPtr.Zero); // RemoveClipRect

			// dispose resources used in off-screen rendering
			Primitive.WinApi.SelectObject(_MemDC, _OrgMemBmp);
			Primitive.WinApi.DeleteDC(_MemDC);
			_MemDC = IntPtr.Zero;
			safeDeleteObject(_MemBmp);
			_MemBmp = IntPtr.Zero;

			// reset graphic coordinate offset
			Offset.X = Offset.Y = 0;
		}
		#endregion

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
		private void safeDeleteObject(IntPtr gdiObj)
		{
			if (gdiObj != IntPtr.Zero)
			{
				Primitive.WinApi.DeleteObject(gdiObj);
			}
		}
	}
}
