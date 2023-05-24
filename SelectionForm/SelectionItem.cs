using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ajkControls.SelectionForm
{
    public class SelectionItem : TreeView.TreeNode
    {
        public SelectionItem(string text, Color color)
        {
            this.Text = text;
            this.Color = color;
        }
        public SelectionItem(string text, Color color, ajkControls.Primitive.IconImage icon, ajkControls.Primitive.IconImage.ColorStyle iconColorStyle)
        {
            this.Text = text;
            this.Color = color;
            this.icon = icon;
            this.iconColorStyle = iconColorStyle;
        }

        private ajkControls.Primitive.IconImage icon = null;
        private ajkControls.Primitive.IconImage.ColorStyle iconColorStyle;

        private Color Color;

        public virtual void Draw(Graphics graphics, int x, int y, Font font, Color backgroundColor, out int height)
        {
            Size tsize = System.Windows.Forms.TextRenderer.MeasureText(graphics, Text, font);
            if (icon != null) graphics.DrawImage(icon.GetImage(tsize.Height, iconColorStyle), new Point(x, y));
            Color bgColor = backgroundColor;
            System.Windows.Forms.TextRenderer.DrawText(
                graphics,
                Text,
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
