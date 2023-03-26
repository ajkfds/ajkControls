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

    public partial class CodeTextbox : UserControl
    {
        private bool skipKeyPress = false;
        private int prevXPos = -1;


        private void CodeTextbox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void CodeTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            skipKeyPress = false;
            if (document == null || !Editable) return;

            int lineBeforeEdit = document.GetLineAt(document.CaretIndex);
            if (BeforeKeyDown != null) BeforeKeyDown(this, e);
            if (e.Handled)
            {
                skipKeyPress = true;
                selectionChanged();
                scrollToCaret();
                Invoke(new Action(Refresh));
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
                        Copy();
                        scrollToCaret();
                        selectionChanged();
                        if (CarletLineChanged != null) CarletLineChanged(this, EventArgs.Empty);
                        e.Handled = true;
                        Invalidate();
                    }
                    break;
                case Keys.X:
                    if (e.Modifiers == Keys.Control)
                    {
                        Cut();
                        scrollToCaret();
                        selectionChanged();
                        e.Handled = true;
                        Invalidate();
                    }
                    break;
                case Keys.V:
                    if (e.Modifiers == Keys.Control)
                    {
                        Paste();
                        scrollToCaret();
                        selectionChanged();
                        e.Handled = true;
                        Invalidate();
                    }
                    break;
                case Keys.Z:
                    if (e.Modifiers == Keys.Control)
                    {
                        Undo();
                        e.Handled = true;
                        Invalidate();
                    }
                    break;
                case Keys.A:
                    if(e.Modifiers == Keys.Control)
                    {
                        SelectAll();
                        e.Handled = true;
                        Invalidate();
                    }
                    break;
                case Keys.Left:
                    {
                        int index = document.SelectionLast;
                        keyLeft(sender, e);
                        Invalidate(document.SelectionStart, index);
                    }
                    break;
                case Keys.Right:
                    {
                        int index = document.SelectionStart;
                        keyRight(sender, e);
                        Invalidate(index, document.SelectionLast);
                    }
                    break;
                case Keys.Up:
                    {
                        int index = document.SelectionLast;
                        keyUp(sender, e);
                        Invalidate(document.SelectionStart, index);
                    }
                    break;
                case Keys.Down:
                    {
                        int index = document.SelectionStart;
                        keyDown(sender, e);
                        Invalidate(index, document.SelectionLast);
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
                                documentReplace(document.CaretIndex, 2, 0, "");
                            }
                            else
                            {
                                documentReplace(document.CaretIndex, 1, 0, "");
                            }
                            redrawToLast = true;
                        }
                        else
                        {
                            if (document.GetLineAt(document.SelectionStart) != document.GetLineAt(document.SelectionLast)) redrawToLast = true;
                            documentReplace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, "");
                        }
                        UpdateVScrollBarRange();
                        caretChanged();
                        selectionChanged();
                        e.Handled = true;
                        if (redrawToLast)
                        {
                            Invalidate(document.SelectionStart, document.Length - 1);
                        }
                        else
                        {
                            Invalidate(document.SelectionStart, document.SelectionLast);
                        }
                    }
                    break;
                case Keys.Back:
                    if (document.SelectionStart == document.SelectionLast)
                    {
                        if (document.CaretIndex == 0) break;
                        if (document.CaretIndex > 1 && document.GetCharAt(document.CaretIndex - 2) == '\r' && document.GetCharAt(document.CaretIndex - 1) == '\n')
                        {
                            documentReplace(document.CaretIndex - 2, 2, 0, "");
                        }
                        else
                        {
                            documentReplace(document.CaretIndex - 1, 1, 0, "");
                        }
                    }
                    else
                    {
                        documentReplace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, "");
                    }
                    UpdateVScrollBarRange();
                    caretChanged();
                    selectionChanged();
                    e.Handled = true;
                    Invalidate();
                    break;
                case Keys.Enter:
                    if (!multiLine) break;
                    if (e.Modifiers == Keys.Shift)
                    {
                        if (document.SelectionStart == document.SelectionLast)
                        {
                            documentReplace(document.CaretIndex, 0, 0, "\n");
                        }
                        else
                        {
                            documentReplace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, "\n");
                        }
                        if (document.CaretIndex >= document.SelectionStart) document.SelectionStart = document.SelectionStart + 1;
                        if (document.CaretIndex >= document.SelectionLast) document.SelectionStart = document.SelectionLast + 1;
                        document.CaretIndex = document.CaretIndex + 1;
                    }
                    else
                    {
                        if (document.SelectionStart == document.SelectionLast)
                        {
                            documentReplace(document.CaretIndex, 0, 0, "\r\n");
                        }
                        else
                        {
                            documentReplace(document.SelectionStart, document.SelectionLast - document.SelectionStart, 0, "\r\n");
                        }
                        if (document.CaretIndex >= document.SelectionStart) document.SelectionStart = document.SelectionStart + 2;
                        if (document.CaretIndex >= document.SelectionLast) document.SelectionStart = document.SelectionLast + 2;
                        document.CaretIndex = document.CaretIndex + 2;
                    }
                    document.SelectionLast = document.SelectionStart;
                    UpdateVScrollBarRange();
                    caretChanged();
                    scrollToCaret();
                    selectionChanged();
                    e.Handled = true;
                    Invalidate();
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
                                documentReplace(document.GetLineStartIndex(i), 1, 0, "");
                            }
                        }
                        document.SelectionStart = document.GetLineStartIndex(lineStart);
                        document.SelectionLast = document.GetLineStartIndex(lineLast);
                    }
                    else
                    {
                        for (int i = lineStart; i < lineLast; i++)
                        {
                            documentReplace(document.GetLineStartIndex(i), 0, 0, "\t");
                        }
                        document.SelectionStart = document.GetLineStartIndex(lineStart);
                        document.SelectionLast = document.GetLineStartIndex(lineLast);
                    }
                    UpdateVScrollBarRange();
                    caretChanged();
                    scrollToCaret();
                    selectionChanged();
                    e.Handled = true;
                    Invalidate();
                    break;
                default:
                    break;
            }

            if (AfterKeyDown != null) AfterKeyDown(this, e);
            int lineAfterEdit = document.GetLineAt(document.CaretIndex);
            if (lineBeforeEdit != lineAfterEdit)
            {
                if (CarletLineChanged != null) CarletLineChanged(this, EventArgs.Empty);
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
        private void keyLeft(object sender, KeyEventArgs e)
        {
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

            caretChanged();
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
            selectionChanged();
//            Invoke(new Action(dbDrawBox.Refresh));
            e.Handled = true;
        }

        private void keyRight(object sender, KeyEventArgs e)
        {
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

            caretChanged();
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
            selectionChanged();
            e.Handled = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            if (!multiLine) return;
            bool onSelectionLast = false;
            if (document.SelectionLast == document.CaretIndex && document.SelectionStart != document.SelectionLast)
            {
                onSelectionLast = true;
            }
            int line = document.GetLineAt(document.CaretIndex);
            if (line == 1) return;


            int headindex = document.GetLineStartIndex(line);
            int xPosition = getXPos(document.CaretIndex, line);
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
            document.CaretIndex = getIndex(xPosition, line);

            if (prevXPos == -1) prevXPos = xPosition;
 
            caretChanged();
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
            scrollToCaret();
            selectionChanged();
            e.Handled = true;
        }
        private void keyDown(object sender, KeyEventArgs e)
        {
            if (!multiLine) return;
            bool onSelectionStart = false;
            if (document.SelectionStart == document.CaretIndex && document.SelectionStart != document.SelectionLast)
            {
                onSelectionStart = true;
            }
            int line = document.GetLineAt(document.CaretIndex);
            if (line == document.Lines) return; // last line

            int headindex = document.GetLineStartIndex(line);
            int xPosition = getXPos(document.CaretIndex, line);
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
            while (line < document.Lines-1 && !document.IsVisibleLine(line))
            {
                line++;
            }
            if (!document.IsVisibleLine(line)) line = document.GetLineAt(document.CaretIndex);

            document.CaretIndex = getIndex(xPosition, line);

            caretChanged();
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
            selectionChanged();
            scrollToCaret();
            e.Handled = true;

//            System.Diagnostics.Debug.Print("prevXPos",prevXPos.ToString());
        }



        private void CodeTextbox_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void CodeTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (document == null || !Editable) return;
            if (skipKeyPress)
            {
                skipKeyPress = false;
                e.Handled = true;
                return;
            }
            if (BeforeKeyPressed != null) BeforeKeyPressed(this, e);
            if (e.Handled)
            {
                Invalidate();
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
                        documentReplace(document.CaretIndex, 0, 0, inChar.ToString());
                    }
                    else
                    {
                        documentReplace(document.CaretIndex, 0, document.GetColorAt(prevIndex), inChar.ToString());
                    }
                    document.CaretIndex++;
                    UpdateVScrollBarRange();
                    sw.Stop();
                    System.Diagnostics.Debug.Print("edit : " + sw.Elapsed.TotalMilliseconds.ToString()+"ms");
                }
                else
                {
                    redrawToLast = true;

                    documentReplace(document.SelectionStart, document.SelectionLast - document.SelectionStart, document.GetColorAt(prevIndex), inChar.ToString());
                    document.CaretIndex = document.SelectionStart + 1;
                    document.SelectionStart = document.CaretIndex;
                    document.SelectionLast = document.CaretIndex;
                    UpdateVScrollBarRange();
                }
            }
            if (AfterKeyPressed != null) AfterKeyPressed(this, e);
            if (redrawToLast)
            {
                Invalidate(document.SelectionStart, document.Length - 1);
            }
            else
            {
                Invalidate(document.SelectionStart, document.SelectionLast);
            }
            //if (InvokeRequired)
            //{
            //    Invoke(new Action(Refresh));
            //}
            //else
            //{
            //    Refresh();
            //}

            Update(); // redraw invalidate area
        }

    }
}
