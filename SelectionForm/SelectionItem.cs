using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ajkControls
{
    public class SelectionItem : TreeNode
    {
        public SelectionItem(string text,System.Drawing.Color color, Icon icon)
        {
            this.Text = text;
        }
    }
}
