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
        private List<int> highlightStarts = new List<int>();
        private List<int> highlighLasts = new List<int>();

        private void hilightUpdateWhenDocReplaced(int index, int replaceLength, byte colorIndex, string text)
        {
            if (highlightStarts.Count == 0) return;

            int change = text.Length - replaceLength;

            for(int i = 0; i < highlightStarts.Count; i++)
            {
                //     start    last
                //       +=======+

                // |---|                 a0
                // |---------|           a1
                // |-------------------| a2

                //           |---|       b0
                //           |---------| b1

                //                  |--| c0

                int start = highlightStarts[i];
                int last = highlighLasts[i];

                if (index <= start) // a0 | a1 | a2
                {
                    if (index+replaceLength < start)
                    { // a0
                        highlightStarts[i] += change;
                        highlighLasts[i] += change;
                    }
                    else if (index+replaceLength <= last) 
                    { // a1
                        highlightStarts[i] = index;
                        highlighLasts[i] = index+ change;
                    }
                    else
                    { // a2
                        highlighLasts[i] += change;
                    }
                }
                else if(index <= highlighLasts[i]+1) // b0 | b1
                {
                    if (index + replaceLength <= last+1)
                    { // b0
                        highlighLasts[i] += change;
                    }
                    else
                    { // b1
                        // none
                    }
                }
                else
                { // c0
                    // none
                }
            }
            ReDrawHighlight();
        }
        public void MoveToNextHighlight(out bool moved)
        {
            moved = false;
            int i = GetHighlightIndex(document.CaretIndex);
            if (i == -1) return;
            i++;
            if (i >= highlightStarts.Count) return;

            SelectHighlight(i);
            moved = true;
        }

        public void GetHighlightPosition(int highlightIndex,out int highlightStart,out int highlightLast)
        {
            if(highlightIndex > highlightStarts.Count)
            {
                highlightStart = -1;
                highlightLast = -1;
                return;
            }
            highlightStart = highlightStarts[highlightIndex];
            highlightLast = highlighLasts[highlightIndex];
        }

        public void SelectHighlight(int highlightIndex)
        {
            document.CaretIndex = highlightStarts[highlightIndex];
            document.SelectionStart = highlightStarts[highlightIndex];
            document.SelectionLast = highlighLasts[highlightIndex] + 1;
        }

        public int GetHighlightIndex(int index)
        {
            if (highlightStarts.Count == 0) return -1;
            for (int i = 0; i < highlightStarts.Count; i++)
            {
                if (highlightStarts[i] <= index && index <= highlighLasts[i]+1) return i;
            }
            return -1;
        }

        public void ClearHighlight()
        {
            if (highlightStarts.Count == 0) return;
            for (int i = 0; i < highlightStarts.Count; i++)
            {
                for (int index = highlightStarts[i]; index <= highlighLasts[i]; index++)
                {
                    if(index < document.Length) document.RemoveMarkAt(index, 7);
                }
            }
            highlightStarts.Clear();
            highlighLasts.Clear();

            dbDrawBox.Invalidate();
        }

        public void AppendHighlight(int highlightStart, int highlightLast)
        {
            for (int index = highlightStart; index <= highlightLast; index++)
            {
                document.SetMarkAt(index, 7);
            }
            highlightStarts.Add(highlightStart);
            highlighLasts.Add(highlightLast);

            dbDrawBox.Invalidate();
        }

        public void ReDrawHighlight()
        {
            for (int j = 0; j < highlightStarts.Count; j++)
            {
                for (int index = highlightStarts[j]; index <= highlighLasts[j]; index++)
                {
                    document.SetMarkAt(index, 7);
                }
            }
            dbDrawBox.Invalidate();
        }

    }
}
