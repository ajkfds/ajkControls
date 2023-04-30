using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ajkControls
{
    public class FolderTreeNode : TreeNode
    {
        public FolderTreeNode(string text, IconImage.ColorStyle colorStyle)
        {
            Text = text;
            this.colorStyle = colorStyle;
        }
        IconImage.ColorStyle colorStyle;

        private static IconImage openFolderIcon = new IconImage(Properties.Resources.openFolder);
        private static IconImage closeFolderIcon = new IconImage(Properties.Resources.folder);
        public override void DrawNode(Graphics graphics, int x, int y, Font font, Color color, Color backgroundColor, Color selectedColor, int lineHeight, bool selected)
        {
            if(IsExpanded)
            {
                graphics.DrawImage(openFolderIcon.GetImage(lineHeight, IconImage.ColorStyle.Original), new Point(x, y));
            }
            else
            {
                graphics.DrawImage(closeFolderIcon.GetImage(lineHeight, IconImage.ColorStyle.Original), new Point(x, y));
            }

            Color bgColor = backgroundColor;
            if (selected) bgColor = selectedColor;
            System.Windows.Forms.TextRenderer.DrawText(
                graphics,
                Text,
                font,
                new Point(x + lineHeight + (lineHeight >> 2), y),
                color,
                bgColor,
                System.Windows.Forms.TextFormatFlags.NoPadding
                );
        }

    }
}
