using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ajkControls
{
    public class SelectionItem : TreeNode
    {
        public SelectionItem(string text, Color color)
        {
            this.text = text;
            this.Color = color;
        }
        public SelectionItem(string text, Color color, ajkControls.IconImage icon, ajkControls.IconImage.ColorStyle iconColorStyle)
        {
            this.text = text;
            this.Color = color;
            this.icon = icon;
            this.iconColorStyle = iconColorStyle;
        }

        private ajkControls.IconImage icon = null;
        private ajkControls.IconImage.ColorStyle iconColorStyle;

        private string text;

        private Color Color;

        public virtual void Draw(Graphics graphics, int x, int y, Font font, Color backgroundColor, out int height)
        {
            Size tsize = System.Windows.Forms.TextRenderer.MeasureText(graphics, text, font);
            if (icon != null) graphics.DrawImage(icon.GetImage(tsize.Height, iconColorStyle), new Point(x, y));
            Color bgColor = backgroundColor;
            System.Windows.Forms.TextRenderer.DrawText(
                graphics,
                text,
                font,
                new Point(x + tsize.Height + (tsize.Height >> 2), y),
                Color,
                bgColor,
                System.Windows.Forms.TextFormatFlags.NoPadding
                );
            height = tsize.Height;
        }

    }
}
