﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ajkControls.Primitive;

namespace ajkControls.TreeView
{
    public partial class TreeView : UserControl
    {
        public TreeView()
        {
            InitializeComponent();
            resize();
            this.dbDrawBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.dbDrawBox_MouseWheel);

            this.vScrollBar.Width = Global.ScrollBarWidth;
            this.hScrollBar.Height = Global.ScrollBarWidth;
        }

        public event EventHandler<TreeNode> SelectedNodeChanged;

        public class NodeClickedEventArgs : EventArgs
        {
            public NodeClickedEventArgs(MouseEventArgs e,TreeNode node)
            {
                this.TreeNode = node;
                this.Button = e.Button;
                this.Location = e.Location;
            }

            public System.Windows.Forms.MouseButtons Button;
            public Point Location;
            public TreeNode TreeNode;
        }
        public delegate void NodeClickedEventHandler(object sender, NodeClickedEventArgs e);
        public event NodeClickedEventHandler NodeClicked;


        protected virtual void OnNodeClicked(NodeClickedEventArgs e)
        {
            if (NodeClicked != null) NodeClicked(this, e);
        }

        private bool vScrollBarVisible = true;
        public bool VScrollBarVisible
        {
            get
            {
                return vScrollBarVisible;
            }
            set
            {
                if (vScrollBarVisible == value) return;
                vScrollBarVisible = value;
                vScrollBar.Visible = value;
            }
        }


        private bool hScrollBarVisible = true;
        public bool HScrollBarVisible
        {
            get
            {
                return hScrollBarVisible;
            }
            set
            {
                if (hScrollBarVisible == value) return;
                hScrollBarVisible = value;
                hScrollBar.Visible = value;
            }
        }


        public override Font Font
        {
            get { return base.Font; }
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
            if(grahics==null) grahics = dbDrawBox.CreateGraphics();
            Size fontSize = System.Windows.Forms.TextRenderer.MeasureText(grahics, "A", Font, new Size(100, 100), TextFormatFlags.NoPadding);
            lineHeight = fontSize.Height;

            visibleLines = (int)Math.Ceiling((double)(dbDrawBox.Height / lineHeight));
            vScrollBar.LargeChange = visibleLines;
            hScrollBar.LargeChange = dbDrawBox.Width;
        }

        public List<TreeNode> TreeNodes = new List<TreeNode>();
        private List<TreeNode> orderedNode = new List<TreeNode>();
        private Graphics grahics = null;

        private void indexNodes()
        {
            int index = 0;
            orderedNode.Clear();
            foreach (TreeNode node in TreeNodes)
            {
                indexHierarchy(node, ref index,0);
            }
            vScrollBar.Maximum = orderedNode.Count;
        }

        private void indexHierarchy(TreeNode node, ref int index,int depth)
        {
            node.Index = index;
            node.Depth = depth;
            orderedNode.Add(node);
            index++;
            if (node.IsExpanded == false) return;
            depth++;
            foreach (TreeNode subNode in node.TreeNodes)
            {
                indexHierarchy(subNode, ref index,depth);
            }
        }

        private static IconImage plusIcon = new IconImage(Properties.Resources.plus);
        private static IconImage minusIcon = new IconImage(Properties.Resources.minus);
        private static IconImage dotIcon = new IconImage(Properties.Resources.dot);

        private TreeNode selectedNode = null;
        public TreeNode SelectedNode
        {
            get
            {
                return selectedNode;
            }
        }

        public Color selectedColor = Color.FromArgb(0xd8, 0xe0, 0xe8);
        public Color SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                selectedColor = value;
            }
        }

        private void dBDrawBox_DoubleBufferedPaint(PaintEventArgs e)
        {
            indexNodes();
            width = 0;
            for(int i= vScrollBar.Value;i<vScrollBar.Value+visibleLines;i++)
            {
                if (i >= orderedNode.Count) break;
                TreeNode node = orderedNode[i];
                node.Width = node.MeasureWidth(grahics, Font, lineHeight);
                if (width < node.Width + lineHeight * node.Depth) width = node.Width + lineHeight * node.Depth;
            }
            hScrollBar.Maximum = width;

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
                    if (node.IsExpanded)
                    {
                        e.Graphics.DrawImage(minusIcon.GetImage(lineHeight, IconImage.ColorStyle.White), new Point(x, y));
                    }
                    else
                    {
                        e.Graphics.DrawImage(plusIcon.GetImage(lineHeight, IconImage.ColorStyle.White), new Point(x, y));
                    }
                }
                else
                {
                    e.Graphics.DrawImage(dotIcon.GetImage(lineHeight, IconImage.ColorStyle.Original), new Point(x, y));
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

        private float size = 8;
        private void dbDrawBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (e.Delta < 0) size--;
                if (e.Delta > 0) size++;
                if (size < 5) size = 5;
                if (size > 20) size = 20;
                this.Font = new Font(this.Font.FontFamily, size);
                dbDrawBox.Refresh();
            }
            else
            {
                int delta = (int)(Global.WheelSensitivity * e.Delta) * SystemInformation.MouseWheelScrollLines;
                if(delta == 0)
                {
                    if (e.Delta < 0) delta = -1;
                    if (e.Delta > 0) delta = 1;
                }
                int value = vScrollBar.Value - delta;
                if (value < vScrollBar.Minimum) value = vScrollBar.Minimum;
                if (value > vScrollBar.Maximum - vScrollBar.LargeChange) value = vScrollBar.Maximum - vScrollBar.LargeChange;
                if (value < 0) value = 0;
                vScrollBar.Value = value;
            }
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
                if (SelectedNodeChanged != null) SelectedNodeChanged(this, node);
            }
            if (leftArea && node.TreeNodes.Count != 0)
            {
                node.IsExpanded = !node.IsExpanded;
                if (node.IsExpanded)
                {
                    node.Expanded();
                }
                else
                {
                    node.Collapsed();
                }
            }
            node.Selected();
            OnNodeClicked(new NodeClickedEventArgs(e, node));
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

        private void dbDrawBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (selectedNode == null) return;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (selectedNode.IsExpanded)
                    {
                        selectedNode.IsExpanded = false;
                        selectedNode.Expanded();
                        Refresh();
                    }
                    break;
                case Keys.Right:
                    if (!selectedNode.IsExpanded && selectedNode.TreeNodes.Count != 0)
                    {
                        selectedNode.IsExpanded = true;
                        selectedNode.Collapsed();
                        Refresh();
                    }
                    break;
                case Keys.Up:
                    if(orderedNode.Contains(selectedNode) && orderedNode.IndexOf(selectedNode) != 0)
                    {
                        selectedNode = orderedNode[orderedNode.IndexOf(selectedNode) - 1];
                        Refresh();
                        SelectedNodeChanged(this, selectedNode);
                    }
                    break;
                case Keys.Down:
                    if (orderedNode.Contains(selectedNode) && orderedNode.IndexOf(selectedNode) != orderedNode.Count-1)
                    {
                        selectedNode = orderedNode[orderedNode.IndexOf(selectedNode) + 1];
                        Refresh();
                        SelectedNodeChanged(this, selectedNode);
                    }
                    break;
            }
        }

        private void dbDrawBox_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void dbDrawBox_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseClick(e);
        }
    }
}
