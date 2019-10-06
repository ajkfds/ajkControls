using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ajkControls
{
    public class CodeDrawStyle
    {
        public virtual Color[] ColorPallet
        {
            get
            {
                return new System.Drawing.Color[16]
                    {
                        System.Drawing.Color.Black, // 0
                        System.Drawing.Color.LightGray, // 1
                        System.Drawing.Color.DarkGray, // 2
                        System.Drawing.Color.Red, // 3
                        System.Drawing.Color.Blue, // 4
                        System.Drawing.Color.Green, // 5
                        System.Drawing.Color.Turquoise, // 6
                        System.Drawing.Color.Purple, // 7
                        System.Drawing.Color.Orange, // 8
                        System.Drawing.Color.Pink, // 9
                        System.Drawing.Color.Black, // 10
                        System.Drawing.Color.Black, // 11
                        System.Drawing.Color.Black, // 12
                        System.Drawing.Color.Black, // 13
                        System.Drawing.Color.Black, // 14
                        System.Drawing.Color.Black  // 15
                    };
            }
        }

        public virtual Color[] MarkColor
        {
            get
            {
                return new System.Drawing.Color[8]
                    {
                        System.Drawing.Color.FromArgb(128,System.Drawing.Color.Red),    // 0
                        System.Drawing.Color.Orange, // 1
                        System.Drawing.Color.Red, // 2
                        System.Drawing.Color.Red, // 3
                        System.Drawing.Color.Red, // 4
                        System.Drawing.Color.Red, // 5
                        System.Drawing.Color.Red, // 6
                        System.Drawing.Color.FromArgb(50,100,10,100)  // 7
                    };
            }
        }

        public virtual CodeTextbox.MarkStyleEnum[] MarkStyle
        {
            get
            {
                return new CodeTextbox.MarkStyleEnum[8]
                    {
                        CodeTextbox.MarkStyleEnum.wave,    // 0
                        CodeTextbox.MarkStyleEnum.underLine,
                        CodeTextbox.MarkStyleEnum.underLine,
                        CodeTextbox.MarkStyleEnum.underLine,
                        CodeTextbox.MarkStyleEnum.underLine,
                        CodeTextbox.MarkStyleEnum.underLine,
                        CodeTextbox.MarkStyleEnum.underLine,
                        CodeTextbox.MarkStyleEnum.fill              // 7 for selection highlight
                    };
            }
        }
    }
}
