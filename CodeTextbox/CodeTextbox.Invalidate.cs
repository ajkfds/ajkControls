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
		public void Invalidate(int beginIndex, int endIndex)
		{
            lock (document)
            {
				invalidateLines(document.GetLineAt(beginIndex), document.GetLineAt(endIndex));
			}
		}

		public void invalidateLines(int beginLine, int endLine)
		{
			int beginDrawLine = 0;
			int endDrawLine = actualLineNumbers.Length;

			for(int i = 0; i < actualLineNumbers.Length; i++)
            {
				if (actualLineNumbers[i] < beginLine) beginDrawLine = i;
				if(endLine < actualLineNumbers[i])
                {
					endDrawLine = i;
					break;
                }
            }

			Invalidate(new Rectangle(0,beginDrawLine*charSizeY,Width,(endDrawLine-beginDrawLine)*charSizeY+100));
		}

	}
}
