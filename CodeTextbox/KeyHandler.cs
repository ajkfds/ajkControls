//Copyright(c) 2018 ajkfds

//Permission is hereby granted, free of charge, to any person obtaining a codeTextbox.Copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, codeTextbox.Copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above codeTextbox.Copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR codeTextbox.CopyRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
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
    public class KeyHandler
    {
        public KeyHandler(CodeTextbox codeTextBox, Drawer drawer, HScrollBar hScrollBar, VScrollBar vScrollBar)
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
        private bool skipKeyPress = false;
        private int prevXPos = -1;


        public void PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            Document document = codeTextbox.Document;

            skipKeyPress = false;
            if (document == null || !codeTextbox.Editable) return;

            int lineBeforeEdit = document.GetLineAt(document.CaretIndex);

            codeTextbox.CallBeforeKeyDown(e);
            if (e.Handled)
            {
                skipKeyPress = true;
                codeTextbox.selectionChanged();
                codeTextbox.scrollToCaret();
                codeTextbox.Invoke(new Action(codeTextbox.Refresh));
                return;
            }

            if (e.KeyCode != Keys.Up && e.KeyCode != Keys.Down)
            {
                prevXPos = -1;
            }

            switch (e.KeyCode)
            {
                case Keys.C:
                    if (e.Modifiers == Keys.Control)
                    {
                        codeTextbox.Copy();
                        codeTextbox.scrollToCaret();
                        codeTextbox.selectionChanged();
                        codeTextbox.CallCarletLineChanged(EventArgs.Empty);
                        e.Handled = true;
                        codeTextbox.Invalidate();
                    }
                    break;
                case Keys.X:
                    if (e.Modifiers == Keys.Control)
                    {
                        codeTextbox.Cut();
                        codeTextbox.scrollToCaret();
                        codeTextbox.selectionChanged();
                        e.Handled = true;
                        codeTextbox.Invalidate();
                    }
                    break;
                case Keys.V:
                    if (e.Modifiers == Keys.Control)
                    {
                        codeTextbox.Paste();
                        codeTextbox.scrollToCaret();
                        codeTextbox.selectionChanged();
                        e.Handled = true;
                        codeTextbox.Invalidate();
                    }
                    break;
                case Keys.Z:
                    if (e.Modifiers == Keys.Control)
                    {
                        codeTextbox.Undo();
                        e.Handled = true;
                        codeTextbox.Invalidate();
                    }
                    break;
                case Keys.A:
                    if (e.Modifiers == Keys.Control)
                    {
                        codeTextbox.SelectAll();
                        e.Handled = true;
                        codeTextbox.Invalidate();
                    }
                    break;
                case Keys.Left:
                    {
                        int index = document.SelectionLast;
                        handleLeftKey(sender, e);
                        codeTextbox.Invalidate(document.SelectionStart, index);
                    }
                    break;
                case Keys.Right:
                    {
                        int index = document.SelectionStart;
                        handleRightKey(sender, e);
                        codeTextbox.Invalidate(index, document.SelectionLast);
                    }
                    break;
                case Keys.Up:
                    {
                        int index = document.SelectionLast;
                        handleUpKey(sender, e);
                        codeTextbox.Invalidate(document.SelectionStart, index);
                    }
                    break;
                case Keys.Down:
                    {
                        int index = document.SelectionStart;
                        handleDownKey(sender, e);
                        codeTextbox.Invalidate(index, document.SelectionLast);
                    }
                    break;
                case Keys.Delete:
                    {
                        bool redrawToLast = false;
                        if (document.SelectionStart == document.SelectionLast)
                        {
                            if (document.CaretIndex == document.Length) break;
                            if (document.CaretIndex != document.Length - 1 && document.GetCharAt(document.CaretIndex) == '\r' && document.GetCharAt(document.CaretIndex + 1) == '\n')
                            {
                                codeTextbox.documentReplace(document.CaretIndex, 2, 0, "");
                            }
                            else
                            {
                                codeTextbox.documentReplace(document.CaretIndex, 1, 0, "");
                            }
                            redrawToLast = true;
                        }
                        else
                        {
                            if (document.GetLineAt(document.SelectionStart) != document.GetLineAt(document.SelectionLast)) redrawToLast = true;
                            codeTextbox.documentReplace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, "");
                        }
                        codeTextbox.UpdateVScrollBarRange();
                        codeTextbox.caretChanged();
                        codeTextbox.selectionChanged();
                        e.Handled = true;
                        if (redrawToLast)
                        {
                            codeTextbox.Invalidate(document.SelectionStart, document.Length - 1);
                        }
                        else
                        {
                            codeTextbox.Invalidate(document.SelectionStart, document.SelectionLast);
                        }
                        e.Handled = true;
                        codeTextbox.Invalidate();
                    }
                    break;
                case Keys.Back:
                    if (document.SelectionStart == document.SelectionLast)
                    {
                        if (document.CaretIndex == 0) break;
                        if (document.CaretIndex > 1 && document.GetCharAt(document.CaretIndex - 2) == '\r' && document.GetCharAt(document.CaretIndex - 1) == '\n')
                        {
                            codeTextbox.documentReplace(document.CaretIndex - 2, 2, 0, "");
                        }
                        else
                        {
                            codeTextbox.documentReplace(document.CaretIndex - 1, 1, 0, "");
                        }
                    }
                    else
                    {
                        codeTextbox.documentReplace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, "");
                    }
                    codeTextbox.UpdateVScrollBarRange();
                    codeTextbox.caretChanged();
                    codeTextbox.selectionChanged();
                    e.Handled = true;
                    codeTextbox.Invalidate();
                    break;
                case Keys.Enter:
                    if (!codeTextbox.MultiLine) break;
                    if (e.Modifiers == Keys.Shift)
                    {
                        if (document.SelectionStart == document.SelectionLast)
                        {
                            codeTextbox.documentReplace(document.CaretIndex, 0, 0, "\n");
                        }
                        else
                        {
                            codeTextbox.documentReplace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, "\n");
                        }
                        if (document.CaretIndex >= document.SelectionStart) document.SelectionStart = document.SelectionStart + 1;
                        if (document.CaretIndex >= document.SelectionLast) document.SelectionStart = document.SelectionLast + 1;
                        document.CaretIndex = document.CaretIndex + 1;
                    }
                    else
                    {
                        if (document.SelectionStart == document.SelectionLast)
                        {
                            codeTextbox.documentReplace(document.CaretIndex, 0, 0, "\r\n");
                        }
                        else
                        {
                            codeTextbox.documentReplace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, "\r\n");
                        }
                        if (document.CaretIndex >= document.SelectionStart) document.SelectionStart = document.SelectionStart + 2;
                        if (document.CaretIndex >= document.SelectionLast) document.SelectionStart = document.SelectionLast + 2;
                        document.CaretIndex = document.CaretIndex + 2;
                    }
                    document.SelectionLast = document.SelectionStart;
                    codeTextbox.UpdateVScrollBarRange();
                    codeTextbox.caretChanged();
                    codeTextbox.scrollToCaret();
                    codeTextbox.selectionChanged();
                    e.Handled = true;
                    codeTextbox.Invalidate();
                    break;
                case Keys.Tab:
                    // multiple lines indent
                    if (document.SelectionStart == document.SelectionLast) break;
                    int lineStart = document.GetLineAt(document.SelectionStart);
                    int lineLast = document.GetLineAt(document.SelectionLast);
                    if (lineStart == lineLast) break;
                    if (e.Modifiers == Keys.Shift)
                    {
                        for (int i = lineStart; i < lineLast; i++)
                        {
                            if (document.GetCharAt(document.GetLineStartIndex(i)) == '\t' || document.GetCharAt(document.GetLineStartIndex(i)) == ' ')
                            {
                                codeTextbox.documentReplace(document.GetLineStartIndex(i), 1, 0, "");
                            }
                        }
                        document.SelectionStart = document.GetLineStartIndex(lineStart);
                        document.SelectionLast = document.GetLineStartIndex(lineLast);
                    }
                    else
                    {
                        for (int i = lineStart; i < lineLast; i++)
                        {
                            codeTextbox.documentReplace(document.GetLineStartIndex(i), 0, 0, "\t");
                        }
                        document.SelectionStart = document.GetLineStartIndex(lineStart);
                        document.SelectionLast = document.GetLineStartIndex(lineLast);
                    }
                    codeTextbox.UpdateVScrollBarRange();
                    codeTextbox.caretChanged();
                    codeTextbox.scrollToCaret();
                    codeTextbox.selectionChanged();
                    e.Handled = true;
                    codeTextbox.Invalidate();
                    break;
                default:
                    break;
            }

            codeTextbox.CallAfterKeyDown(e);
            int lineAfterEdit = document.GetLineAt(document.CaretIndex);
            if (lineBeforeEdit != lineAfterEdit)
            {
                codeTextbox.CallCarletLineChanged(EventArgs.Empty);
            }

            if (e.Handled)
            {
                skipKeyPress = true;
                //Invalidate();
                //if (InvokeRequired)
                //{
                //    Invoke(new Action(Refresh));
                //}
                //else
                //{
                //    Refresh();
                //}
            }
        }
        private void handleLeftKey(object sender, KeyEventArgs e)
        {
            Document document = codeTextbox.Document;
            if (document.CaretIndex < 1) return;
            bool onSelectionLast = false;
            if (document.SelectionLast == document.CaretIndex && document.SelectionStart != document.SelectionLast)
            {
                onSelectionLast = true;
            }

            if (document.CaretIndex > 0 && document.GetCharAt(document.CaretIndex) == '\n' && document.GetCharAt(document.CaretIndex - 1) == '\r')
            {
                document.CaretIndex = document.CaretIndex - 2;
            }
            else
            {
                document.CaretIndex--;
            }
            if ((e.Modifiers & Keys.Control) == Keys.Control)
            {
                int headIndex, length;
                document.GetWord(document.CaretIndex, out headIndex, out length);
                if (headIndex < document.CaretIndex)
                {
                    document.CaretIndex = headIndex;
                }
            }

            codeTextbox.caretChanged();
            if ((e.Modifiers & Keys.Shift) == Keys.Shift)
            {
                if (onSelectionLast)
                {
                    document.SelectionLast = document.CaretIndex;
                }
                else
                {
                    document.SelectionStart = document.CaretIndex;
                }
            }
            else
            {
                document.SelectionStart = document.CaretIndex;
                document.SelectionLast = document.CaretIndex;
            }
            codeTextbox.selectionChanged();
            //            Invoke(new Action(dbDrawBox.Refresh));
            e.Handled = true;
        }

        private void handleRightKey(object sender, KeyEventArgs e)
        {
            Document document = codeTextbox.Document;
            if (document.CaretIndex >= document.Length) return;
            bool onSelectionStart = false;
            if (document.SelectionStart == document.CaretIndex && document.SelectionStart != document.SelectionLast)
            {
                onSelectionStart = true;
            }
            if (document.CaretIndex != document.Length - 1 && document.GetCharAt(document.CaretIndex) == '\r' && document.GetCharAt(document.CaretIndex + 1) == '\n')
            {
                document.CaretIndex = document.CaretIndex + 2;
            }
            else
            {
                document.CaretIndex++;
            }
            if ((e.Modifiers & Keys.Control) == Keys.Control)
            {
                int headIndex, length;
                document.GetWord(document.CaretIndex, out headIndex, out length);
                if (document.CaretIndex < headIndex + length)
                {
                    document.CaretIndex = headIndex + length;
                }
            }

            codeTextbox.caretChanged();
            if ((e.Modifiers & Keys.Shift) == Keys.Shift)
            {
                if (onSelectionStart)
                {
                    document.SelectionStart = document.CaretIndex;
                }
                else
                {
                    document.SelectionLast = document.CaretIndex;
                }
            }
            else
            {
                document.SelectionStart = document.CaretIndex;
                document.SelectionLast = document.CaretIndex;
            }
            codeTextbox.selectionChanged();
            e.Handled = true;
        }

        public void handleUpKey(object sender, KeyEventArgs e)
        {
            Document document = codeTextbox.Document;
            if (!codeTextbox.MultiLine) return;
            bool onSelectionLast = false;
            if (document.SelectionLast == document.CaretIndex && document.SelectionStart != document.SelectionLast)
            {
                onSelectionLast = true;
            }
            int line = document.GetLineAt(document.CaretIndex);
            if (line == 1) return;


            int headindex = document.GetLineStartIndex(line);
            int xPosition = drawer.getXPos(document.CaretIndex, line);
            if (prevXPos > xPosition)
            {
                xPosition = prevXPos;
            }
            else
            {
                prevXPos = xPosition;
            }

            line--;

            // skip invisible lines
            while (line > 1 && !document.IsVisibleLine(line))
            {
                line--;
            }
            if (!document.IsVisibleLine(line)) line = document.GetLineAt(document.CaretIndex);


            headindex = document.GetLineStartIndex(line);
            document.CaretIndex = drawer.getIndex(xPosition, line);

            if (prevXPos == -1) prevXPos = xPosition;

            codeTextbox.caretChanged();
            if (e.Modifiers == Keys.Shift)
            {
                if (onSelectionLast)
                {
                    document.SelectionLast = document.CaretIndex;
                }
                else
                {
                    document.SelectionStart = document.CaretIndex;
                }
            }
            else
            {
                document.SelectionStart = document.CaretIndex;
                document.SelectionLast = document.CaretIndex;
            }
            codeTextbox.scrollToCaret();
            codeTextbox.selectionChanged();
            e.Handled = true;
        }
        private void handleDownKey(object sender, KeyEventArgs e)
        {
            Document document = codeTextbox.Document;
            if (!codeTextbox.MultiLine) return;
            bool onSelectionStart = false;
            if (document.SelectionStart == document.CaretIndex && document.SelectionStart != document.SelectionLast)
            {
                onSelectionStart = true;
            }
            int line = document.GetLineAt(document.CaretIndex);
            if (line == document.Lines) return; // last line

            int headindex = document.GetLineStartIndex(line);
            int xPosition = drawer.getXPos(document.CaretIndex, line);
            if (prevXPos > xPosition)
            {
                xPosition = prevXPos;
            }
            else
            {
                prevXPos = xPosition;
            }

            if (line == document.Lines) return;
            line++;

            // skip invisible lines
            while (line < document.Lines - 1 && !document.IsVisibleLine(line))
            {
                line++;
            }
            if (!document.IsVisibleLine(line)) line = document.GetLineAt(document.CaretIndex);

            document.CaretIndex = drawer.getIndex(xPosition, line);

            codeTextbox.caretChanged();
            if (e.Modifiers == Keys.Shift)
            {
                if (onSelectionStart)
                {
                    document.SelectionStart = document.CaretIndex;
                }
                else
                {
                    document.SelectionLast = document.CaretIndex;
                }
            }
            else
            {
                document.SelectionStart = document.CaretIndex;
                document.SelectionLast = document.CaretIndex;
            }
            codeTextbox.selectionChanged();
            codeTextbox.scrollToCaret();
            e.Handled = true;

            //            System.Diagnostics.Debug.Print("prevXPos",prevXPos.ToString());
        }



        public void KeyUp(object sender, KeyEventArgs e)
        {

        }

        public void KeyPress(object sender, KeyPressEventArgs e)
        {
            Document document = codeTextbox.Document;

            if (document == null || !codeTextbox.Editable) return;
            if (skipKeyPress)
            {
                skipKeyPress = false;
                e.Handled = true;
                codeTextbox.Update();
                return;
            }

            codeTextbox.CallBeforeKeyPressed(e);
            if (e.Handled)
            {
                codeTextbox.Invalidate();
                codeTextbox.Update();
                return;
            }

            char inChar = e.KeyChar;
            int prevIndex = document.CaretIndex;
            if (document.GetLineStartIndex(document.GetLineAt(prevIndex)) != prevIndex && prevIndex != 0)
            {
                prevIndex--;
            }

            bool redrawToLast = false;

            if ((inChar < 127 && inChar >= 0x20) || inChar == '\t' || inChar > 0xff)
            { // insert charactors
                if (document.SelectionStart == document.SelectionLast)
                {
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                    if (inChar == '\t' || inChar == ' ')
                    {   // use default color for tab and space
                        codeTextbox.documentReplace(document.CaretIndex, 0, 0, inChar.ToString());
                    }
                    else
                    {
                        codeTextbox.documentReplace(document.CaretIndex, 0, document.GetColorAt(prevIndex), inChar.ToString());
                    }
                    document.CaretIndex++;
                    codeTextbox.UpdateVScrollBarRange();
                    sw.Stop();
                    System.Diagnostics.Debug.Print("edit : " + sw.Elapsed.TotalMilliseconds.ToString() + "ms");
                }
                else
                {
                    redrawToLast = true;

                    codeTextbox.documentReplace(document.SelectionStart, document.SelectionLast - document.SelectionStart, document.GetColorAt(prevIndex), inChar.ToString());
                    document.CaretIndex = document.SelectionStart + 1;
                    document.SelectionStart = document.CaretIndex;
                    document.SelectionLast = document.CaretIndex;
                    codeTextbox.UpdateVScrollBarRange();
                }
            }
            codeTextbox.CallAfterKeyPressed(e);
            if (redrawToLast)
            {
                codeTextbox.Invalidate(document.SelectionStart, document.Length - 1);
            }
            else
            {
                codeTextbox.Invalidate(document.SelectionStart, document.SelectionLast);
            }
            //if (InvokeRequired)
            //{
            //    Invoke(new Action(Refresh));
            //}
            //else
            //{
            //    Refresh();
            //}

            codeTextbox.Update(); // redraw codeTextbox.Invalidate area
        }

    }
}
