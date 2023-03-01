using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ajkControls.ToolStrip
{
    public class CustomRenderer : ToolStripSystemRenderer
    {
        public CustomRenderer(Color foreColor,Color backColor, Color selectedForeColor, Color selectedBackColor)
        {
            this.foreColor = foreColor;
            this.backColor = backColor;
            this.selectedForeColor = selectedForeColor;
            this.selectBackColor = selectedBackColor;
        }

        private Color foreColor;
        private Color backColor;
        private Color selectedForeColor;
        private Color selectBackColor;

        protected override void InitializeItem(ToolStripItem item)
        {
            base.InitializeItem(item);
            item.ForeColor = foreColor;
        }

        protected override void Initialize(System.Windows.Forms.ToolStrip toolStrip)
        {
            base.Initialize(toolStrip);
            toolStrip.ForeColor = foreColor;
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBorder(e);
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBackground(e);
            e.Graphics.FillRectangle(new SolidBrush(backColor), e.AffectedBounds);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            base.OnRenderImageMargin(e);
            e.Graphics.FillRectangle(new SolidBrush(backColor), e.AffectedBounds);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            base.OnRenderArrow(e);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderMenuItemBackground(e);

            var rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(selectBackColor), rect);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(backColor), rect);
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                e.Item.ForeColor = selectedForeColor;
            }
            else
            {
                e.Item.ForeColor = foreColor;
            }
            base.OnRenderItemText(e);
        }

    }
}
