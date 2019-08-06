using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ajkControls.Git
{
    public class BranchNode : TreeNode
    {
        public BranchNode(string text)
        {
            this.Text = text;
        }

        private static IconImage branchIcon = new IconImage(Properties.Resources.tree);
        public override void DrawNode(Graphics graphics, int x, int y, Font font, Color color, Color backgroundColor, Color selectedColor, int lineHeight, bool selected)
        {
            graphics.DrawImage(branchIcon.GetImage(lineHeight, IconImage.ColorStyle.Blue), new Point(x, y));

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
