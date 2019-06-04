using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ajkControls
{
    public class Document
    {
        public Document()
        {
            newLineIndex.Replace(0, 0, new int[] { 0 });
            lineVisible.Replace(0, 0, new bool[] { true });
            visibleLines = 1;
        }

        ResizableArray<char> chars = new ResizableArray<char>(1024, 256);
        ResizableArray<byte> colors = new ResizableArray<byte>(1024, 256);
        ResizableArray<byte> marks = new ResizableArray<byte>(1024, 256);
        ResizableArray<int> newLineIndex = new ResizableArray<int>(256, 256);
        ResizableArray<bool> lineVisible = new ResizableArray<bool>(256, 256);
        private int visibleLines = 0;
        List<int> collapsedLines = new List<int>();

        public int EditID { get; private set; } = 0;

        List<History> histories = new List<History>();
        public int HistoryMaxLimit = 100;

        public class History
        {
            public History(int index,int length,string changedFrom)
            {
                Index = index;
                Length = length;
                ChangedFrom = changedFrom;
            }
            public readonly int Index;
            public readonly int Length;
            public readonly string ChangedFrom;
        }

        public int Length
        {
            get
            {
                return chars.Length;
            }
        }

        List<int> blockStartIndexs = new List<int>();
        List<int> blockEndIndexs = new List<int>();

        // block infomation cash
        bool blockCashActive = false;
        List<int> blockStartLines = new List<int>();
        List<int> blockEndLines = new List<int>();
        private void createBlockCash()
        {
            blockStartLines.Clear();
            blockEndLines.Clear();
            for(int i = 0; i < blockStartIndexs.Count; i++)
            {
                blockStartLines.Add(GetLineAt(blockStartIndexs[i]));
                blockEndLines.Add(GetLineAt(blockEndIndexs[i]));
            }
        }

        private void refreshVisibleLines()
        {
            for(int i = 0; i < lineVisible.Length; i++)
            {
                lineVisible[i] = true;
            }
            visibleLines = Lines;
            for(int i = collapsedLines.Count-1; i >= 0; i--)
            {
                int j = blockStartLines.IndexOf(collapsedLines[i]);
                if (j == -1)
                {
                    collapsedLines.RemoveAt(i);
                }
                else
                {
                    for (int k = blockStartLines[j] + 1; k < blockEndLines[j]; k++)
                    {
                        lineVisible[k] = false;
                        visibleLines--;
                    }
                }
            }
        }


        public int VisibleLines
        {
            get
            {
                return visibleLines;
            }
        }


        public void ClearBlock()
        {
            blockCashActive = false;
            blockStartIndexs.Clear();
            blockEndIndexs.Clear();
        }
        public void AppendBlock(int startIndex,int endIndex)
        {
            blockCashActive = false;
            blockStartIndexs.Add(startIndex);
            blockEndIndexs.Add(endIndex);
        }

        public bool IsVisibleLine(int lineNo)
        {
            if (!blockCashActive) createBlockCash();
            return lineVisible[lineNo];
        }
        public bool IsBlockHeadLine(int lineNo)
        {
            if (!blockCashActive) createBlockCash();
            return blockStartLines.Contains(lineNo) ;
        }

        public void CollapseBlock(int lineNo)
        {
            if (!blockCashActive) createBlockCash();
            if (!blockStartLines.Contains(lineNo)) return;
            if(!collapsedLines.Contains(lineNo))
            {
                collapsedLines.Add(lineNo);
                refreshVisibleLines();
            }
        }

        public void ExpandBlock(int lineNo)
        {
            if (!blockCashActive) createBlockCash();
            if (!blockStartLines.Contains(lineNo)) return;
            if (collapsedLines.Contains(lineNo))
            {
                collapsedLines.Remove(lineNo);
                refreshVisibleLines();
            }
        }

        public bool IsCollapsed(int lineNo)
        {
            if (!blockStartLines.Contains(lineNo)) return false;
            if (collapsedLines.Contains(lineNo)) return true;
            return false;
        }

        int selectionStart;
        public int SelectionStart {
            get
            {
                return selectionStart;
            }
            set
            {
                selectionStart = value;
            }
        }

        int selectionLast;
        public int SelectionLast
        {
            get
            {
                return selectionLast;
            }
            set
            {
                selectionLast = value;
            }
        }

        int caretIndex;
        public int CaretIndex
        {
            get
            {
                return caretIndex;
            }
            set
            {
                caretIndex = value;
            }
        }

        public char GetCharAt(int index)
        {
            return chars[index];
        }

        public void SetCharAt(int index,char value)
        {
            chars[index] = value;
        }

        public void CopyColorsFrom(Document document)
        {
            colors.CopyFrom(document.colors);
        }

        public void CopyMarksFrom(Document document)
        {
            marks.CopyFrom(document.marks);
        }

        public void CopyLineIndexFrom(Document document)
        {
            newLineIndex.CopyFrom(document.newLineIndex);
        }

        public void CopyCharsFrom(Document document)
        {
            chars.CopyFrom(document.chars);
            colors.Resize(document.Length);
            marks.Resize(document.Length);
        }

        public void CopyBlocksFrom(Document document)
        {
            blockCashActive = false;
            blockStartIndexs = document.blockStartIndexs;
            blockEndIndexs = document.blockEndIndexs;
        }

        public byte GetMarkAt(int index)
        {
            return marks[index];
        }

        public void SetMarkAt(int index, byte value)
        {
            marks[index] |= (byte)(1<<value);
        }

        public void RemoveMarkAt(int index, byte value)
        {
            marks[index] &= (byte)((1 << value)^0xff);
        }

        public byte GetColorAt(int index)
        {
            return colors[index];
        }

        public void SetColorAt(int index, byte value)
        {
            colors[index] = value;
        }

        public void Undo()
        {
            if (histories.Count == 0) return;
            History history = histories.Last();
            histories.RemoveAt(histories.Count - 1);
            EditID--;
            replace(history.Index,history.Length,0,history.ChangedFrom);
        }

        public void ClearHistory()
        {
            histories.Clear();
        }

        public void Replace(int index, int replaceLength, byte colorIndex, string text)
        {
            histories.Add(new History(index, text.Length, CreateString(index, replaceLength)));
           EditID++;
            if (histories.Count > HistoryMaxLimit)
            {
                histories.RemoveAt(0);
            }
            replace(index,replaceLength,colorIndex,text);
        }


        private void replace(int index,int replaceLength, byte colorIndex, string text)
        {
            // replace text
            char[] array = text.ToArray();
            byte[] color = new byte[text.Length];
            byte[] mark = new byte[text.Length];

            if(colorIndex != 0)
            {
                for(int i = 0; i < text.Length; i++)
                {
                    color[i] = colorIndex;
                }
            }

            chars.Replace(index, replaceLength, array);
            colors.Replace(index, replaceLength, color);
            marks.Replace(index, replaceLength, mark);

            // update Selection
            updateIndex(ref caretIndex, index, replaceLength, text.Length);
            updateIndex(ref selectionStart, index, replaceLength, text.Length);
            updateIndex(ref selectionLast, index, replaceLength, text.Length);

            // update new line index array
            List<int> lines = new List<int>();
            for(int i=0;i<array.Length;i++)
            {
                if (array[i] != '\n') continue;
                lines.Add(i + index);
            }

            int startLine = GetLineAt(index)-1;
            int endLine = GetLineAt(index + replaceLength)-1;
            int changedLine = lines.Count+startLine- endLine;

            if(changedLine > 0)
            {
                newLineIndex.Resize(newLineIndex.Length + changedLine);
                lineVisible.Resize(lineVisible.Length + changedLine);

                for (int i = newLineIndex.Length - 1; i >= startLine + lines.Count; i--)
                {
                    newLineIndex[i] = newLineIndex[i - changedLine] + text.Length - replaceLength;
                    lineVisible[i] = lineVisible[i - changedLine];
                }
                for (int i = 0; i < lines.Count; i++)
                {
                    newLineIndex[startLine + i] = lines[i];
                    lineVisible[startLine + i] = true;
                }
            }
            else if (changedLine < 0)
            {
                for( int i = startLine + lines.Count; i < newLineIndex.Length + changedLine;i++)
                {
                    newLineIndex[i] = newLineIndex[i - changedLine] + text.Length - replaceLength;
                    lineVisible[i] = lineVisible[i - changedLine];
                }
                for (int i = 0; i < lines.Count; i++)
                {
                    newLineIndex[startLine + i] = lines[i];
                    lineVisible[startLine + i] = lineVisible[i];
                }

                newLineIndex.Resize(newLineIndex.Length + changedLine);
                lineVisible.Resize(lineVisible.Length + changedLine);
            }
            else
            {
                for (int i = newLineIndex.Length + changedLine - 1; i >= startLine + lines.Count; i--)
                {
                    newLineIndex[i] = newLineIndex[i - changedLine] + text.Length - replaceLength;
                    lineVisible[i] = lineVisible[i - changedLine];
                }
                for (int i = 0; i < lines.Count; i++)
                {
                    newLineIndex[startLine + i] = lines[i];
                    lineVisible[startLine + i] = lineVisible[i];
                }
            }

            for(int i = 0; i < collapsedLines.Count - 1; i++)
            {
                if (collapsedLines[i] > endLine) collapsedLines[i] = collapsedLines[i] + changedLine;
            }
            visibleLines = visibleLines + changedLine;

//            System.Diagnostics.Debug.Print("line,lineVisibles "+ newLineIndex.Length.ToString()+","+ lineVisible.Length.ToString()+","+ VisibleLines.ToString());
        }

        private void updateIndex(ref int index,int modifyIndex,int modifyLength,int modifiedToLength)
        {
            if (index < modifyIndex) return; // before modified area
            if (index <= modifyIndex + modifyLength) // in mofdified area
            {
                if (index <= modifyIndex + modifiedToLength)
                {
                    return;
                }
                else
                {
                    index = modifyIndex + modifiedToLength;
                }
            }
            else
            { // after modified area
                index = modifyIndex + modifiedToLength - modifyLength;
            }
            if (index < 0) index = 0;
            if (index > Length) index = Length;
        }

        public int GetLineAt(int index)
        {
/*          simple implementation
            for (int line = 0; line < newLineIndex.Length; line++)
            {
                if (newLineIndex[line] >= index) return line;
            }
            exception
*/
            int lineAfter = 0;
            int lineBefore = newLineIndex.Length-1;

            int l = lineBefore >> 1;

            while (lineBefore-1 > lineAfter)
            {
                if (newLineIndex[l] >= index)
                {
                    lineBefore = l;
                }
                else if (newLineIndex[l] < index)
                {
                    lineAfter = l;
                }
                l = (lineBefore + lineAfter) >> 1;
            }
            if (newLineIndex[l] < index) l++;

            return l+1;
         }

        public int GetVisibleLine(int line)
        {
            int visibleLine = 1;
            for(int l = 0; l < line; l++)
            {
                if (lineVisible[l]) visibleLine++;   
            }
            return visibleLine;
        }


        public int GetLineStartIndex(int line)
        {
            System.Diagnostics.Debug.Assert(line <= newLineIndex.Length + 1);
            if(line == 1)
            {
                return 0;
            }
            else
            {
                return newLineIndex[line - 2]+1;
            }
        }

        public int GetLineLength(int line)
        {
//            System.Diagnostics.Debug.Assert(line < newLineIndex.Length + 1);
            if (line == 1)
            {
                if(newLineIndex.Length == 0)
                {
                    return chars.Length;
                }
                else
                {
                    return newLineIndex[0];
                }
            }
            else if (line == newLineIndex.Length)
            {
                return 0;
            }
            else
            {
                return newLineIndex[line-1] - newLineIndex[line - 2] - 1;
            }
        }

        public int Lines
        {
            get {
                return newLineIndex.Length;
            }
        }

        public int FindIndexOf(string targetString,int startIndex)
        {
            if (targetString.Length == 0) return -1;
            for (int i = startIndex; i < Length-targetString.Length; i++)
            {
                if (targetString[0] != chars[i]) continue;
                bool match = true;
                for(int j = 1; j < targetString.Length; j++)
                {
                    if (targetString[j] != chars[i+j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match) return i;
            }
            return -1;
        }

        public int FindPreviousIndexOf(string targetString, int startIndex)
        {
            if (targetString.Length == 0) return -1;
            if (startIndex > Length - targetString.Length) startIndex = Length - targetString.Length;

            for (int i = startIndex; i >=0; i--)
            {
                if (targetString[0] != chars[i]) continue;
                bool match = true;
                for (int j = 1; j < targetString.Length; j++)
                {
                    if (targetString[j] != chars[i + j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match) return i;
            }
            return -1;
        }

        public string CreateString()
        {
            char[] array = chars.CreateArray();
            return new string(array);
        }

        public string CreateString(int index,int length)
        {
            char[] array = chars.CreateArray(index, length);
            return new string(array);
        }

        public char[] CreateCharArray()
        {
            return chars.CreateArray();
        }

        public string CreateLineString(int line)
        {
            return new string(CreateLineArray(line));
        }

        public char[] CreateLineArray(int line)
        {
            char[] array = chars.CreateArray( GetLineStartIndex(line),GetLineLength(line) );
            return array;
        }
    }
}
