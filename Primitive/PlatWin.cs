// file: PlatWin.cs
// brief: Platform API caller for Windows.
// author: YAMAMOTO Suguru
// encoding: UTF-8
// update: 2011-02-20
//=========================================================
using System;
using System.Drawing;
using System.Text;
using Control = System.Windows.Forms.Control;
using SystemInformation = System.Windows.Forms.SystemInformation;
using Marshal = System.Runtime.InteropServices.Marshal;

namespace ajkControls.Primitive
{
	/// <summary>
	/// Platform API for Windows.
	/// </summary>
	public class PlatWin
	{
		#region Fields
		const string LineSelectClipFormatName = "MSDEVLineSelect";
		const string RectSelectClipFormatName = "MSDEVColumnSelect";
		UInt32 _CF_LINEOBJECT = Primitive.WinApi.CF_PRIVATEFIRST + 1;
		UInt32 _CF_RECTSELECT = Primitive.WinApi.CF_PRIVATEFIRST + 2;
		#endregion

		#region Init / Dispose
		public PlatWin()
		{
			_CF_LINEOBJECT = Primitive.WinApi.RegisterClipboardFormatW(LineSelectClipFormatName);
			_CF_RECTSELECT = Primitive.WinApi.RegisterClipboardFormatW(RectSelectClipFormatName);
		}
		#endregion

		#region UI Notification
		public void MessageBeep()
		{
			Primitive.WinApi.MessageBeep(0);
		}
		#endregion

		#region Clipboard
		/// <summary>
		/// Gets content of the system clipboard.
		/// </summary>
		/// <param name="dataType">The type of the text data in the clipboard</param>
		/// <returns>Text content retrieved from the clipboard if available. Otherwise null.</returns>
		/// <remarks>
		/// <para>
		/// This method gets text from the system clipboard.
		/// If stored text data is a special format (line or rectangle,)
		/// its data type will be set to <paramref name="dataType"/> parameter.
		/// </para>
		/// </remarks>
		/// <seealso cref="Sgry.Azuki.TextDataType">TextDataType enum</seealso>
		public string GetClipboardText(out TextDataType dataType)
		{
			Int32 rc; // result code
			bool clipboardOpened = false;
			IntPtr dataHandle = IntPtr.Zero;
			IntPtr dataPtr = IntPtr.Zero;
			uint formatID = UInt32.MaxValue;
			string data = null;

			dataType = TextDataType.Normal;

			try
			{
				// open clipboard
				rc = Primitive.WinApi.OpenClipboard(IntPtr.Zero);
				if (rc == 0)
				{
					return null;
				}
				clipboardOpened = true;

				// distinguish type of data in the clipboard
				if (Primitive.WinApi.IsClipboardFormatAvailable(_CF_LINEOBJECT) != 0)
				{
					formatID = Primitive.WinApi.CF_UNICODETEXT;
					dataType = TextDataType.Line;
				}
				else if (Primitive.WinApi.IsClipboardFormatAvailable(_CF_RECTSELECT) != 0)
				{
					formatID = Primitive.WinApi.CF_UNICODETEXT;
					dataType = TextDataType.Rectangle;
				}
				else if (Primitive.WinApi.IsClipboardFormatAvailable(Primitive.WinApi.CF_UNICODETEXT) != 0)
				{
					formatID = Primitive.WinApi.CF_UNICODETEXT;
				}
				else if (Primitive.WinApi.IsClipboardFormatAvailable(Primitive.WinApi.CF_TEXT) != 0)
				{
					formatID = Primitive.WinApi.CF_TEXT;
				}
				if (formatID == UInt32.MaxValue)
				{
					return null; // no text data was in clipboard
				}

				// get handle of the clipboard data
				dataHandle = Primitive.WinApi.GetClipboardData(formatID);
				if (dataHandle == IntPtr.Zero)
				{
					return null;
				}

				// get data pointer by locking the handle
				dataPtr = Utl.MyGlobalLock(dataHandle);
				if (dataPtr == IntPtr.Zero)
				{
					return null;
				}

				// retrieve data
				if (formatID == Primitive.WinApi.CF_TEXT)
					data = Utl.MyPtrToStringAnsi(dataPtr);
				else
					data = Marshal.PtrToStringUni(dataPtr);
			}
			finally
			{
				// unlock handle
				if (dataPtr != IntPtr.Zero)
				{
					Utl.MyGlobalUnlock(dataHandle);
				}
				if (clipboardOpened)
				{
					Primitive.WinApi.CloseClipboard();
				}
			}

			return data;
		}

		/// <summary>
		/// Sets content of the system clipboard.
		/// </summary>
		/// <param name="text">Text data to set.</param>
		/// <param name="dataType">Type of the data to set.</param>
		/// <remarks>
		/// <para>
		/// This method set content of the system clipboard.
		/// If <paramref name="dataType"/> is TextDataType.Normal,
		/// the text data will be just a character sequence.
		/// If <paramref name="dataType"/> is TextDataType.Line or TextDataType.Rectangle,
		/// stored text data would be special format that is compatible with Microsoft Visual Studio.
		/// </para>
		/// </remarks>
		public void SetClipboardText(string text, TextDataType dataType)
		{
			Int32 rc; // result code
			IntPtr dataHdl;
			bool clipboardOpened = false;

			try
			{
				// open clipboard
				rc = Primitive.WinApi.OpenClipboard(IntPtr.Zero);
				if (rc == 0)
				{
					return;
				}
				clipboardOpened = true;

				// clear clipboard first
				Primitive.WinApi.EmptyClipboard();

				// set normal text data
				dataHdl = Utl.MyStringToHGlobalUni(text);
				Primitive.WinApi.SetClipboardData(Primitive.WinApi.CF_UNICODETEXT, dataHdl);

				// set addional text data
				if (dataType == TextDataType.Line)
				{
					// allocate dummy text (this is needed for PocketPC)
					dataHdl = Utl.MyStringToHGlobalUni("");
					Primitive.WinApi.SetClipboardData(_CF_LINEOBJECT, dataHdl);
				}
				else if (dataType == TextDataType.Rectangle)
				{
					// allocate dummy text (this is needed for PocketPC)
					dataHdl = Utl.MyStringToHGlobalUni("");
					Primitive.WinApi.SetClipboardData(_CF_RECTSELECT, dataHdl);
				}
			}
			finally
			{
				if (clipboardOpened)
				{
					Primitive.WinApi.CloseClipboard();
				}
			}
		}
		#endregion

		#region UI parameters
		/// <summary>
		/// It will be regarded as a drag operation by the system
		/// if mouse cursor moved beyond this rectangle.
		/// </summary>
		public Size DragSize
		{
			get
			{
#if !PocketPC
				return SystemInformation.DragSize;
#else
				return new Size( 4, 4 );
#endif
			}
		}
		#endregion

		//#region Graphic Interface
		///// <summary>
		///// Gets a graphic device context from a window.
		///// </summary>
		//public IGraphics GetGraphics(object window)
		//{
		//	AzukiControl azuki = window as AzukiControl;
		//	if (azuki != null)
		//	{
		//		return new GraWin(azuki.Handle, azuki.FontInfo);
		//	}

		//	Control control = window as Control;
		//	if (control != null)
		//	{
		//		if (control.Font == null)
		//			return new GraWin(control.Handle, new FontInfo());
		//		else
		//			return new GraWin(control.Handle, new FontInfo(control.Font));
		//	}

		//	throw new ArgumentException("an object of unexpected type (" + window.GetType() + ") was given to PlatWin.GetGraphics.", "window");
		//}
		//#endregion

		#region Utilities
		class Utl
		{
			#region Handle Allocation
			public static IntPtr MyGlobalLock(IntPtr handle)
			{
#if !PocketPC
				return Primitive.WinApi.GlobalLock(handle);
#else
				return handle;
#endif
			}

			public static void MyGlobalUnlock(IntPtr handle)
			{
#if !PocketPC
				Primitive.WinApi.GlobalUnlock(handle);
#else
				// do nothing
#endif
			}
			#endregion

			#region String Conversion
			public static string MyPtrToStringAnsi(IntPtr dataPtr)
			{
				unsafe
				{
					byte* p = (byte*)dataPtr;
					int byteCount = 0;

					// count length
					for (int i = 0; *(p + i) != 0; i++)
					{
						byteCount++;
					}

					// copy data
					byte[] data = new byte[byteCount];
					for (int i = 0; i < byteCount; i++)
					{
						data[i] = *(p + i);
					}

					return new String(Encoding.Default.GetChars(data));
				}
			}

			/// <exception cref="ArgumentOutOfRangeException">Too long text was given.</exception>
			/// <exception cref="OutOfMemoryException">No enough memory.</exception>
			public static IntPtr MyStringToHGlobalUni(string text)
			{
#if !PocketPC
				return Marshal.StringToHGlobalUni(text);
#else
				unsafe {
					IntPtr handle = Marshal.AllocHGlobal( sizeof(char)*(text.Length + 1) );
					for( int i=0; i<text.Length; i++ )
					{
						Marshal.WriteInt16( handle, i*sizeof(char), (short)text[i] ); // handle[i] = text[i];
					}
					Marshal.WriteInt16( handle, text.Length*sizeof(char), 0 ); // buf[text.Length] = '\0';
					
					return handle;
				}
#endif
			}
			#endregion
		}
		#endregion
	}

}
