using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ajkControls
{
	/// <summary>
	/// Specifies type of text data.
	/// </summary>
	public enum TextDataType
	{
		/// <summary>
		/// Normal text data; a stream of characters.
		/// </summary>
		Normal,

		/// <summary>
		/// Word text data; a stream of characters which starts and ends at word boundaries.
		/// </summary>
		Words,

		/// <summary>
		/// Line text data; not a stream but a line.
		/// </summary>
		Line,

		/// <summary>
		/// Rectangle text data; graphically layouted text.
		/// </summary>
		Rectangle
	}

	/// <summary>
	/// Information about font.
	/// </summary>
	public class FontInfo
	{
		string _Name;
		int _Size;
		FontStyle _Style;

		#region Properties
		/// <summary>
		/// Font face name of this font.
		/// </summary>
		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		/// <summary>
		/// Size of this font in pt (point).
		/// </summary>
		public int Size
		{
			get { return _Size; }
			set { _Size = value; }
		}

		/// <summary>
		/// Style of this font.
		/// </summary>
		public FontStyle Style
		{
			get { return _Style; }
			set { _Style = value; }
		}
		#endregion

		#region Init / Dispose
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public FontInfo()
		{
#if PocketPC
			_Name = FontFamily.GenericSansSerif.Name;
			_Size = 10;
			_Style = FontStyle.Regular;
#else
			_Name = SystemFonts.DefaultFont.Name;
			_Size = (int)SystemFonts.DefaultFont.Size;
			_Style = SystemFonts.DefaultFont.Style;
#endif
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public FontInfo(string name, int size, FontStyle style)
		{
			_Name = name;
			_Size = size;
			_Style = style;
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public FontInfo(FontInfo fontInfo)
		{
			if (fontInfo == null)
				throw new ArgumentNullException("fontInfo");

			_Name = fontInfo.Name;
			_Size = fontInfo.Size;
			_Style = fontInfo.Style;
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public FontInfo(Font font)
		{
			if (font == null)
				throw new ArgumentNullException("font");

			_Name = font.Name;
			_Size = (int)font.Size;
			_Style = font.Style;
		}
		#endregion

		#region Utilities
		/// <summary>
		/// Gets user readable text of this font information.
		/// </summary>
		public override string ToString()
		{
			return String.Format("\"{0}\", {1}, {2}", _Name, _Size, _Style);
		}

		/// <summary>
		/// Creates new instance of System.Drawing.Font with same information.
		/// </summary>
		/// <exception cref="System.ArgumentException">Failed to create System.Font object.</exception>
		public Font ToFont()
		{
			try
			{
				return new Font(Name, Size, Style);
			}
			catch (ArgumentException ex)
			{
				// ArgumentException will be thrown
				// if the font specified the name does not support
				// specified font style.
				// try to find available font style for the font.
				FontStyle[] styles = new FontStyle[5];
				styles[0] = FontStyle.Regular;
				styles[1] = FontStyle.Bold;
				styles[2] = FontStyle.Italic;
				styles[3] = FontStyle.Underline;
				styles[4] = FontStyle.Strikeout;
				foreach (FontStyle s in styles)
				{
					try
					{
						return new Font(Name, Size, s);
					}
					catch
					{ }
				}

				// there is nothing Azuki can do...
				throw ex;
			}
		}

		/// <summary>
		/// Creates new instance of System.Drawing.Font with same information.
		/// </summary>
		public static implicit operator Font(FontInfo other)
		{
			return other.ToFont();
		}
		#endregion
	}

}
