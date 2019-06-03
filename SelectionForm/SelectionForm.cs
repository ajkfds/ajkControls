using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ajkControls
{
    public partial class SelectionForm : Form
    {
        public SelectionForm()
        {
            InitializeComponent();
            Document document = new Document();
            codeTextbox.Document = document;

            this.Font = Global.UiDefaultFont;
            this.ShowInTaskbar = false;
        }

        public event EventHandler Selected;


        public void SetSelectionItems(List<SelectionItem> items)
        {
            this.items = items;
            UpdateVisibleItems("");
        }

        public override Font Font
        {
            get
            {
                return base.Font;
            }

            set
            {
                base.Font = value;
                codeTextbox.Font = Font;
                codeTextbox.Refresh();
            }
        }


        private List<SelectionItem> items = new List<SelectionItem>();
        private List<SelectionItem> visibleItems = new List<SelectionItem>();

        private SelectionItem selectedItem = null;

        private string inputText = "";
        public void UpdateVisibleItems(string inputText)
        {
            this.inputText = inputText;
            UpdateVisibleItems();
        }

        public void UpdateVisibleItems()
        {
            lock (visibleItems)
            {
                List<SelectionItem> partialMatch = new List<SelectionItem>();
                visibleItems.Clear();
                foreach (SelectionItem item in items)
                {
                    if (item.Text.ToLower().StartsWith(inputText.ToLower()))
                    {
                        visibleItems.Add(item);
                        continue;
                    }
                    if (item.Text.ToLower().Contains(inputText.ToLower()))
                    {
                        partialMatch.Add(item);
                        continue;
                    }
                }

                foreach (SelectionItem item in partialMatch)
                {
                    visibleItems.Add(item);
                }

                if (selectedItem != null && !visibleItems.Contains(selectedItem))
                {
                    selectedItem = null;
                }
                if (selectedItem == null && visibleItems.Count != 0)
                {
                    selectedItem = visibleItems[0];
                }
            }
            if (selectedItem == null)
            {
                Visible = false;
                return;
            }
            doubleBufferedDrawBox.Refresh();
        }

        public SelectionItem SelectItem()
        {
            if (!visibleItems.Contains(selectedItem))
            {
                return null;
            }
            Visible = false;
            return selectedItem;
        }

        public void moveDown()
        {
            if (!visibleItems.Contains(selectedItem)) return;
            int index = visibleItems.IndexOf(selectedItem);
            index++;
            if (index < visibleItems.Count)
            {
                selectedItem = visibleItems[index];
            }
            UpdateVisibleItems();
        }

        public void moveUp()
        {
            if (!visibleItems.Contains(selectedItem)) return;
            int index = visibleItems.IndexOf(selectedItem);
            index--;
            if (index >= 0)
            {
                selectedItem = visibleItems[index];
            }
            UpdateVisibleItems();
        }

        private int leftMargin = 4;
        private int rightMargin = 4;
        private int topMargin = 4;
        private int bottomMargin = 4;
        private int maxVisibleLines = 8;

        private SolidBrush selectionBrush = new SolidBrush(Color.FromArgb(100, 0, 50, 100));
        private void doubleBufferedDrawBox_DoubleBufferedPaint(PaintEventArgs e)
        {
            lock (visibleItems)
            {
                if (items.Count == 0) return;
                // update size
                int visibleLines = maxVisibleLines;
                if (visibleLines > visibleItems.Count)
                {
                    visibleLines = visibleItems.Count;
                }
                // update scrollbar
                vScrollBar.Minimum = 0;
                vScrollBar.Maximum = items.Count;
                vScrollBar.LargeChange = visibleLines;
                int selectedIndex = visibleItems.IndexOf(selectedItem);
                if (selectedIndex < vScrollBar.Value)
                {
                    vScrollBar.Value = selectedIndex;
                }
                else if (selectedIndex >= vScrollBar.Value + visibleLines)
                {
                    if ((selectedIndex - visibleLines + 1) < 0)
                    {
                        vScrollBar.Value = 0;
                    }
                    else
                    {
                        vScrollBar.Value = selectedIndex - visibleLines + 1;
                    }
                }

                int y = topMargin;
                int height;
                bool setHeight = true;
                for (int i = vScrollBar.Value; i < vScrollBar.Value + visibleLines; i++)
                {
                    SelectionItem item = visibleItems[i];
                    item.Draw(e.Graphics, leftMargin, y, Font, BackColor, out height);
                    if (setHeight)
                    {
                        Height = doubleBufferedDrawBox.Top+ height * visibleLines + topMargin + bottomMargin;
                        setHeight = false;
                    }
                    if (item == selectedItem)
                    {
                        e.Graphics.FillRectangle(selectionBrush, new Rectangle(leftMargin, y, Width - leftMargin - rightMargin, height));
                    }
                    y += height;
                }
           }
        }

        private void codeTextbox_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible) codeTextbox.Focus();
            if (codeTextbox.Document != null) codeTextbox.Document.Replace(0, codeTextbox.Document.Length, 0, "");
            SelectedItem = null;
        }

        private void SelectionForm_Deactivate(object sender, EventArgs e)
        {
            Visible = false;
        }

        private void codeTextbox_AfterKeyPressed(object sender, KeyPressEventArgs e)
        {
            UpdateVisibleItems(codeTextbox.Document.CreateLineString(1));
        }

        private void codeTextbox_BeforeKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Visible = false;
                    break;
                case Keys.Return:
                    if (selectedItem == null) return;
                    SelectedItem = selectedItem;
                    if (Selected != null) Selected(this, EventArgs.Empty);
                    Visible = false;
                    break;
                case Keys.Up:
                    moveUp();
                    break;
                case Keys.Down:
                    moveDown();
                    break;
            }
        }

        public SelectionItem SelectedItem { get; set; }


    }
}
