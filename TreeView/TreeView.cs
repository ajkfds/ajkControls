using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ajkControls
{
    public partial class TreeView : UserControl
    {
        public TreeView()
        {
            InitializeComponent();
            grahics = dbDrawBox.CreateGraphics();
            resize();
        }

        public event EventHandler<TreeNode> SelectedNodeChanged;
        public event EventHandler<TreeNode> NodeClicked;

        //        public delegate void TreeNodeClickedHandler(TreeNode treeNode);
        //        public event TreeNodeClickedHandler TreeNodeClicked;

        //        public delegate void SelectedNodeChangedHandler(PaintEventArgs e);
        //        public event SelectedNodeChangedHandler SelectedNodeChanged;

        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                resize();
            }
        }

        public override void Refresh()
        {
            resize();
            dbDrawBox.Invalidate();
            base.Refresh();
        }

        private void TreeView_Resize(object sender, EventArgs e)
        {
            resize();
        }

        int lineHeight = 0;
        int visibleLines = 10;
        int width = 0;
        private void resize()
        {
            Size fontSize = System.Windows.Forms.TextRenderer.MeasureText(grahics, "A", Font, new Size(100, 100), TextFormatFlags.NoPadding);
            lineHeight = fontSize.Height;

            visibleLines = (int)Math.Ceiling((double)(dbDrawBox.Height / lineHeight));
            vScrollBar.LargeChange = visibleLines;
            hScrollBar.LargeChange = dbDrawBox.Width;
        }

        public List<TreeNode> TreeNodes = new List<TreeNode>();
        public List<TreeNode> orderedNode = new List<TreeNode>();
        private Graphics grahics = null;

        private void indexNodes()
        {
            width = 0;

            int index = 0;
            orderedNode.Clear();
            foreach (TreeNode node in TreeNodes)
            {
                indexHierarchy(node, ref index,0);
            }
            vScrollBar.Maximum = orderedNode.Count;
            hScrollBar.Maximum = width;
        }

        private void indexHierarchy(TreeNode node, ref int index,int depth)
        {
            node.Index = index;
            node.Depth = depth;
            orderedNode.Add(node);
            node.Width = node.MeasureWidth(grahics, Font, lineHeight);
            if (width < node.Width + lineHeight * node.Depth) width = node.Width + lineHeight * node.Depth;
            index++;
            if (node.Exanded == false) return;
            depth++;
            foreach (TreeNode subNode in node.TreeNodes)
            {
                indexHierarchy(subNode, ref index,depth);
            }
        }

        private static Icon plusIcon = new Icon(Properties.Resources.plus);
        private static Icon minusIcon = new Icon(Properties.Resources.minus);
        private static Icon dotIcon = new Icon(Properties.Resources.dot);

        private TreeNode selectedNode = null;

        private Color selectedColor = Color.SlateGray;

        private void dBDrawBox_DoubleBufferedPaint(PaintEventArgs e)
        {
            indexNodes();

            int index = vScrollBar.Value;
            int y = 0;
            e.Graphics.Clear(BackColor);
            for(int i= 0; i < visibleLines; i++)
            {
                if (index >= orderedNode.Count) break;
                TreeNode node = orderedNode[index];
                int x = node.Depth * lineHeight-hScrollBar.Value;
                if(node.TreeNodes.Count != 0)
                {
                    if (node.Exanded)
                    {
                        e.Graphics.DrawImage(minusIcon.GetImage(lineHeight, Icon.ColorStyle.White), new Point(x, y));
                    }
                    else
                    {
                        e.Graphics.DrawImage(plusIcon.GetImage(lineHeight, Icon.ColorStyle.White), new Point(x, y));
                    }
                }
                else
                {
                    e.Graphics.DrawImage(dotIcon.GetImage(lineHeight, Icon.ColorStyle.Original), new Point(x, y));
                }
                x = x + lineHeight;
                bool selected = false;
                if (selectedNode == node) selected = true;
                node.DrawNode(e.Graphics, x, y, Font, ForeColor, BackColor,selectedColor,lineHeight,selected);
                y = y + lineHeight;
                index++;
            }
        }

        private void hitTest(int x,int y, out TreeNode node,out bool leftArea)
        {
            leftArea = false;
            node = null;
            int line = y / lineHeight;
            int index = line + vScrollBar.Value;
            if (index >= orderedNode.Count) return;

            node = orderedNode[index];
            if (x < (node.Depth+1) * lineHeight) leftArea = true;
        }

        private void dbDrawBox_MouseDown(object sender, MouseEventArgs e)
        {
            bool leftArea;
            TreeNode node;
            hitTest(e.X+hScrollBar.Value, e.Y, out node, out leftArea);
            if (node == null) return;
            if (selectedNode != node)
            {
                selectedNode = node;
                if (SelectedNodeChanged != null) SelectedNodeChanged(this,node);
            }
            if(leftArea && node.TreeNodes.Count != 0)
            {
                node.Exanded = !node.Exanded;
            }
            if (NodeClicked != null) NodeClicked(this, node);
            Refresh();
        }

        private void vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void dbDrawBox_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void hScrollBar_ValueChanged(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
