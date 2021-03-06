﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;


namespace ajkControls
{

    public partial class CodeTextbox : UserControl
    {
        private void dbDrawBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (document == null || !Editable) return;

            if (e.Button == MouseButtons.Left)
            {
                if (e.X < xOffset * charSizeX) // block folding control
                {
                    int drawLine = e.Y / charSizeY;
                    int line = GetActualLineNo(drawLine);
                    if (document.IsCollapsed(line))
                    {
                        document.ExpandBlock(line);
                    }
                    else
                    {
                        document.CollapseBlock(line);
                    }
                    UpdateVScrollBarRange();
                    Invoke(new Action(dbDrawBox.Refresh));
                    return;
                }

                int index = hitIndex(e.X, e.Y);


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
                        state = uiState.selecting;
                        selectionChanged();
                        Invoke(new Action(dbDrawBox.Refresh));
                    }
                    else
                    {
                        document.CaretIndex = index;
                        document.SelectionStart = index;
                        document.SelectionLast = index;
                        state = uiState.selecting;
                        selectionChanged();
                        Invoke(new Action(dbDrawBox.Refresh));
                    }
                }
                else
                {
                    state = uiState.selecting;
                }
            }
        }

        private void dbDrawBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (document == null || !Editable) return;

            if (state == uiState.selecting)
            {
                int index = hitIndex(e.X, e.Y);
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
                selectionChanged();

                Invoke(new Action(dbDrawBox.Refresh));
            }
            this.OnMouseMove(e);
        }

        private void dbDrawBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (document == null || !Editable) return;

            if (e.Button == MouseButtons.Left)
            {
                if (state == uiState.selecting)
                {
                    int index = hitIndex(e.X, e.Y);
                    if (document.SelectionStart == document.CaretIndex)
                    {
                        document.SelectionStart = index;
                        document.CaretIndex = index;
                        state = uiState.idle;
                        selectionChanged();
                        Invoke(new Action(dbDrawBox.Refresh));
                    }
                    else if (document.SelectionLast == document.CaretIndex)
                    {
                        document.SelectionLast = index;
                        document.CaretIndex = index;
                        state = uiState.idle;
                        selectionChanged();
                        Invoke(new Action(dbDrawBox.Refresh));
                    }
                    else
                    {
                        state = uiState.idle;
                    }
                }
            }
        }
        private void dbDrawBox_MouseLeave(object sender, EventArgs e)
        {
            if (document == null || !Editable) return;

            if (state == uiState.selecting)
            {
                document.CaretIndex = document.SelectionStart;
                document.SelectionLast = document.SelectionStart;
                state = uiState.idle;
                selectionChanged();
                Invoke(new Action(dbDrawBox.Refresh));
            }
            this.OnMouseLeave(e);
        }

        private void dbDrawBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (document == null || !Editable) return;

            if (state == uiState.selecting)
            {
                if (document.CaretIndex != document.SelectionStart || document.SelectionLast != document.SelectionStart)
                {
                    document.CaretIndex = document.SelectionStart;
                    document.SelectionLast = document.SelectionStart;
                    state = uiState.idle;
                    selectionChanged();
                    Invoke(new Action(dbDrawBox.Refresh));
                }
                else
                {
                    state = uiState.idle;
                }
            }
            this.OnMouseDoubleClick(e);
        }
    }
}
