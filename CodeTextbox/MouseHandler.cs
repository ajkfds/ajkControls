//Copyright(c) 2018 ajkfds

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ajkControls.CodeTextbox
{
    public class MouseHandler
    {
        public MouseHandler(CodeTextbox codeTextBox,Drawer drawer, HScrollBar hScrollBar, VScrollBar vScrollBar)
        {
            this.codeTextbox = codeTextBox;
            this.vScrollBar = vScrollBar;
            this.hScrollBar = hScrollBar;
            this.drawer = drawer;
        }
        CodeTextbox codeTextbox;
        HScrollBar hScrollBar;
        VScrollBar vScrollBar;
        Drawer drawer;
        public void CodeTextbox_MouseEnter(object sender, EventArgs e)
        {

        }

        public void CodeTextbox_MouseHover(object sender, EventArgs e)
        {

        }




        public void CodeTextbox_MouseDown(object sender, MouseEventArgs e)
        {
            Document document = codeTextbox.Document;
            if (document == null || !codeTextbox.Editable) return;

            if (e.Button == MouseButtons.Left)
            {
                if (e.X < drawer.xOffset * drawer.charSizeX) // block folding control
                {
                    int drawLine = e.Y / drawer.charSizeY;
                    int line = codeTextbox.GetActualLineNo(drawLine);
                    if (document.IsCollapsed(line))
                    {
                        document.ExpandBlock(line);
                    }
                    else
                    {
                        document.CollapseBlock(line);
                    }
                    codeTextbox.UpdateVScrollBarRange();
                    codeTextbox.Invoke(new Action(codeTextbox.Refresh));
                    return;
                }

                int index = drawer.hitIndex(e.X, e.Y);


                if (document.CaretIndex != index || document.SelectionStart != index || document.SelectionLast != index)
                {
                    if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                    {
                        if (index < document.CaretIndex)
                        {
                            document.SelectionStart = index;
                            //                            document.SelectionLast = document.Carlet;
                            document.CaretIndex = index;
                        }
                        else
                        {
                            //                           document.SelectionStart = document.CaretIndex;
                            document.SelectionLast = index;
                            document.CaretIndex = index;
                        }
                        codeTextbox.state = CodeTextbox.uiState.selecting;
                        codeTextbox.selectionChanged();
                        codeTextbox.Invoke(new Action(codeTextbox.Refresh));
                    }
                    else
                    {
                        document.CaretIndex = index;
                        document.SelectionStart = index;
                        document.SelectionLast = index;
                        codeTextbox.state = CodeTextbox.uiState.selecting;
                        codeTextbox.selectionChanged();
                        codeTextbox.Invoke(new Action(codeTextbox.Refresh));
                    }
                }
                else
                {
                    codeTextbox.state = CodeTextbox.uiState.selecting;
                }
            }
        }

        public void CodeTextbox_MouseMove(object sender, MouseEventArgs e)
        {
            Document document = codeTextbox.Document;
            if (document == null || !codeTextbox.Editable) return;

            if (codeTextbox.state == CodeTextbox.uiState.selecting)
            {
                int index = drawer.hitIndex(e.X, e.Y);
                if (document.SelectionStart == document.SelectionLast)
                {
                    if (document.SelectionStart < index)
                    {
                        document.SelectionLast = index;
                    }
                    else
                    {
                        document.SelectionStart = index;
                    }
                }
                else if (document.SelectionStart == document.CaretIndex)
                {
                    document.SelectionStart = index;
                }
                else if (document.SelectionLast == document.CaretIndex)
                {
                    document.SelectionLast = index;
                }
                document.CaretIndex = index;
                codeTextbox.selectionChanged();

                codeTextbox.Invoke(new Action(codeTextbox.Refresh));
            }
            //            this.OnMouseMove(e);
        }

        public void CodeTextbox_MouseUp(object sender, MouseEventArgs e)
        {
            Document document = codeTextbox.Document;
            if (document == null || !codeTextbox.Editable) return;

            if (e.Button == MouseButtons.Left)
            {
                if (codeTextbox.state == CodeTextbox.uiState.selecting)
                {
                    int index = drawer.hitIndex(e.X, e.Y);
                    if (document.SelectionStart == document.CaretIndex)
                    {
                        document.SelectionStart = index;
                        document.CaretIndex = index;
                        codeTextbox.state = CodeTextbox.uiState.idle;
                        codeTextbox.selectionChanged();
                        codeTextbox.Invoke(new Action(codeTextbox.Refresh));
                    }
                    else if (document.SelectionLast == document.CaretIndex)
                    {
                        document.SelectionLast = index;
                        document.CaretIndex = index;
                        codeTextbox.state = CodeTextbox.uiState.idle;
                        codeTextbox.selectionChanged();
                        codeTextbox.Invoke(new Action(codeTextbox.Refresh));
                    }
                    else
                    {
                        codeTextbox.state = CodeTextbox.uiState.idle;
                    }
                }
            }
        }
        public void CodeTextbox_MouseLeave(object sender, EventArgs e)
        {
            Document document = codeTextbox.Document;
            if (document == null || !codeTextbox.Editable) return;

            if (codeTextbox.state == CodeTextbox.uiState.selecting)
            {
                document.CaretIndex = document.SelectionStart;
                document.SelectionLast = document.SelectionStart;
                codeTextbox.state = CodeTextbox.uiState.idle;
                codeTextbox.selectionChanged();
                codeTextbox.Invoke(new Action(codeTextbox.Refresh));
            }
            //            this.OnMouseLeave(e);
        }

        public void CodeTextbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Document document = codeTextbox.Document;
            if (document == null || !codeTextbox.Editable) return;

            if (codeTextbox.state == CodeTextbox.uiState.selecting)
            {
                if (document.CaretIndex != document.SelectionStart || document.SelectionLast != document.SelectionStart)
                {
                    document.CaretIndex = document.SelectionStart;
                    document.SelectionLast = document.SelectionStart;
                    codeTextbox.state = CodeTextbox.uiState.idle;
                    codeTextbox.selectionChanged();
                    codeTextbox.Invoke(new Action(codeTextbox.Refresh));
                }
                else
                {
                    codeTextbox.state = CodeTextbox.uiState.idle;
                }
            }
            //            this.OnMouseDoubleClick(e);
        }
    }
}
