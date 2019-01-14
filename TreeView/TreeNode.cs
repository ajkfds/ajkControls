using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ajkControls
{
    public class TreeNode
    {
        public List<TreeNode> TreeNodes = new List<TreeNode>();
        public bool Exanded { get; set; }

        public virtual string Text { get; set; }

        internal int Index { get; set; }
        internal int Depth { get; set; }
        internal int Width { get; set; } = -1;

        private static IconImage questionIcon = new IconImage(Properties.Resources.question);
        public virtual void DrawNode(Graphics graphics,int x,int y,Font font,Color color,Color backgroundColor, Color selectedColor, int lineHeight,bool selected)
        {
            graphics.DrawImage(questionIcon.GetImage(lineHeight, IconImage.ColorStyle.Original),new Point(x,y));
            Color bgColor = backgroundColor;
            if (selected) bgColor = selectedColor;
            System.Windows.Forms.TextRenderer.DrawText(
                graphics, 
                Text, 
                font, 
                new Point(x+lineHeight + (lineHeight>>2), y), 
                color,
                bgColor , 
                System.Windows.Forms.TextFormatFlags.NoPadding
                );
        }

        public virtual int MeasureWidth(Graphics graphics, Font font, int lineHeight)
        {
            Size size = System.Windows.Forms.TextRenderer.MeasureText(graphics, Text, font,new Size(1000,lineHeight<<2), System.Windows.Forms.TextFormatFlags.NoPadding);
            return size.Width + lineHeight + (lineHeight >> 2);
        }
    }
}
