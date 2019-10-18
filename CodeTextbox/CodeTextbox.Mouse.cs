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


namespace ajkControls
{

    public partial class CodeTextbox : UserControl
    {
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
