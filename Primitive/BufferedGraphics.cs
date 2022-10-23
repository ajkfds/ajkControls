using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ajkControls
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
			_DC = WinApi.GetDC(_Window);
		}

		public virtual void Dispose()
		{
			WinApi.SelectObject(_MemDC, _OrgMemBmp);

			// release DC
			WinApi.ReleaseDC(_Window, _DC);
			WinApi.DeleteDC(_MemDC);

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
			_MemDC = WinApi.CreateCompatibleDC(_DC);
			_MemBmp = WinApi.CreateCompatibleBitmap(_DC, paintRect.Width, paintRect.Height);
			Offset = paintRect.Location;
			_MemDcSize = paintRect.Size;
			_OrgMemBmp = WinApi.SelectObject(_MemDC, _MemBmp);
		}

		/// <summary>
		/// End using off-screen buffer and flush all drawing results.
		/// </summary>
		public void EndPaint()
		{
			const uint SRCCOPY = 0x00CC0020;

			// flush cached graphic update
			WinApi.BitBlt(_DC, Offset.X, Offset.Y, _MemDcSize.Width, _MemDcSize.Height,
				_MemDC, 0, 0, SRCCOPY);
			WinApi.SelectClipRgn(DC, IntPtr.Zero); // RemoveClipRect

			// dispose resources used in off-screen rendering
			WinApi.SelectObject(_MemDC, _OrgMemBmp);
			WinApi.DeleteDC(_MemDC);
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
				WinApi.DeleteObject(gdiObj);
			}
		}
	}
}
