using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ajkControls.CodeTextbox
{
    public class Document
    {
        public Document()
        {
            lock (this)
            {
                newLineIndex.Replace(0, 0, new int[] { 0, 0 });
                lineVisible.Replace(0, 0, new bool[] { true, true });
                visibleLines = 1;
            }
        }

        public Document(string text)
        {
            lock (this)
            {
                newLineIndex.Replace(0, 0, new int[] { 0, 0 });
                lineVisible.Replace(0, 0, new bool[] { true, true });
                visibleLines = 1;

                Replace(0, 0, 0, text);
                ClearHistory();
                Version = 0;
                Clean();
            }

            if (chars.Length != colors.Length)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        public void Clean()
        {
            CleanVersion = Version;
        }

        public bool IsDirty
        {
            get
            {
                if (CleanVersion == Version) return false;
                return true;
            }
        }

        public Action<int, int, byte, string> Replaced;


        ResizableArray<char> chars = new ResizableArray<char>(1024, 256);
        ResizableArray<byte> colors = new ResizableArray<byte>(1024, 256);
        ResizableArray<byte> marks = new ResizableArray<byte>(1024, 256);
        ResizableArray<int> newLineIndex = new ResizableArray<int>(256, 256);
        ResizableArray<bool> lineVisible = new ResizableArray<bool>(256, 256);
        private int visibleLines = 0;
        List<int> collapsedLines = new List<int>();

        public ulong Version { get; private set; } = 0;

        public ulong CleanVersion { get; private set; } = 0;

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


        // block handling /////////////////////////////

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
            blockCashActive = true;
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
                    for (int k = blockStartLines[j] ; k < blockEndLines[j]-1; k++)
                    {
                        if (lineVisible[k])
                        {
                            lineVisible[k] = false;
                            visibleLines--;
                        }
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

        public int GetVisibleLineNo(int lineNo)
        {
            if (!blockCashActive) createBlockCash();
            if (collapsedLines.Count == 0) return lineNo;
            int vline = 0;
            for (int i = 0; i < lineNo; i++)
            {
                if (lineVisible[i]) vline++;
            }
            return vline;
        }

        public int GetActialLineNo(int visibleLineNo)
        {
            if (!blockCashActive) createBlockCash();
            if (collapsedLines.Count == 0) return visibleLineNo;
            int lineNo = 0;
            int vLine = 0;
            for (lineNo = 0; lineNo < Lines; lineNo++)
            {
                if (lineVisible[lineNo]) vLine++;
                if (visibleLineNo == vLine) break;
            }
            if (lineNo == 0) lineNo = 1;
            return lineNo;
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
            return lineVisible[lineNo-1];
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

        /////////////////////////////////////////

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

        public void CopyColorMarkFrom(Document document)
        {
            copyFrom(document, false, true, true);
        }
        public void CopyFrom(Document document)
        {
            copyFrom(document, true,true, true);
        }
        public void CopyTextOnlyFrom(Document document)
        {
            copyFrom(document, true, false, false);
        }

        private void copyFrom(Document document, bool copyText,bool copyMark,bool copyColor)
        {
            if (this == document)
            {
                System.Diagnostics.Debugger.Break();
            }
            lock (this)
            {
                lock (document)
                {
                    if (copyText)
                    {
                        chars.CopyFrom(document.chars);
                        newLineIndex.CopyFrom(document.newLineIndex);
                        marks.Resize(document.Length);
                    }
                    else
                    {
                        if (chars.Length != colors.Length || chars.Length != marks.Length)
                        {
                            System.Diagnostics.Debugger.Break();
                        }
                    }

                    if (copyColor)
                    {
                        colors.CopyFrom(document.colors);
                    }
                    else
                    {
                        colors.Resize(document.Length);
                    }

                    if (copyMark)
                    {
                        if (copyMark) marks.CopyFrom(document.marks);
                    }
                    else
                    {
                        marks.Resize(document.Length);
                    }

                    blockCashActive = false;
                    blockStartIndexs = document.blockStartIndexs;
                    blockEndIndexs = document.blockEndIndexs;
                }
            }
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
            lock (this)
            {
                if (histories.Count == 0) return;
                History history = histories.Last();
                histories.RemoveAt(histories.Count - 1);
                Version--;
                replace(history.Index, history.Length, 0, history.ChangedFrom);
            }
        }

        public void ClearHistory()
        {
            histories.Clear();
        }

        public void Replace(int index, int replaceLength, byte colorIndex, string text)
        {
            lock (this)
            {
                histories.Add(new History(index, text.Length, CreateString(index, replaceLength)));
                Version++;
                if (histories.Count > HistoryMaxLimit)
                {
                    histories.RemoveAt(0);
                }
                replace(index, replaceLength, colorIndex, text);
            }
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
                lines.Add(i + index+1);
            }

            int startLine = GetLineAt(index);
            int endLine = GetLineAt(index + replaceLength);
            int changedLine = lines.Count + startLine - endLine;

            if(changedLine > 0)
            {
                newLineIndex.Resize(newLineIndex.Length + changedLine);
                lineVisible.Resize(lineVisible.Length + changedLine);

                for (int i = newLineIndex.Length-1; i > startLine + lines.Count-1; i--)
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
                for (int i = startLine + lines.Count-1 +1; i < newLineIndex.Length + changedLine; i++)
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
                for (int i = newLineIndex.Length - 1; i >= startLine + lines.Count; i--)
                {
                    newLineIndex[i] = newLineIndex[i] + text.Length - replaceLength;
                    lineVisible[i] = lineVisible[i];
                }
                for (int i = 0; i < lines.Count; i++)
                {
                    newLineIndex[startLine + i] = lines[i];
                    lineVisible[startLine + i] = lineVisible[i];
                }
            }

            for (int i = collapsedLines.Count - 1; i >= 0; i--)
            {
                if (collapsedLines[i] > endLine && changedLine != 0)
                {
                    collapsedLines[i] = collapsedLines[i] + changedLine;
                    if (collapsedLines[i] < startLine) collapsedLines.RemoveAt(i);
                }
            }

            
            for (int i =　blockStartIndexs.Count - 1; i >= 0; i--)
            {
                if (blockStartIndexs[i] > index + replaceLength)
                {
                    blockStartIndexs[i] = blockStartIndexs[i] - replaceLength + array.Length;
                }
            }
            visibleLines = visibleLines + changedLine;
            if (Replaced != null) Replaced(index, replaceLength, colorIndex,text);

            if(chars.Length != colors.Length)
            {
                System.Diagnostics.Debugger.Break();
            }
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
            lock (this)
            {
                int lineStart = 1;
                int lineLast = newLineIndex.Length;

                int l = (lineLast + lineStart) >> 1;

                while (lineStart < lineLast - 1)
                {
                    if (index == newLineIndex[l - 1])
                    {
                        return l;
                    }
                    else if (index < newLineIndex[l - 1])
                    {
                        lineLast = l;
                    }
                    else if (newLineIndex[l - 1] < index)
                    {
                        lineStart = l;
                    }
                    l = (lineLast + lineStart) >> 1;
                }
                l = lineStart;
                if (newLineIndex[l - 1 + 1] < index) l++;

                return l;
            }
        }

        public int GetVisibleLine(int line)
        {
            lock (this)
            {
                int visibleLine = 0;
                for (int l = 0; l < line; l++)
                {
                    if (lineVisible[l]) visibleLine++;
                }
                return visibleLine;
            }
        }


        public int GetLineStartIndex(int line)
        {
            lock (this)
            {
                System.Diagnostics.Debug.Assert(line <= newLineIndex.Length + 1);
                if (line == 1)
                {
                    return 0;
                }
                else
                {
                    return newLineIndex[line - 1];
                }
            }
        }

        public int GetLineLength(int line)
        {
            lock (this)
            {
                return newLineIndex[line] - newLineIndex[line - 1];
            }
        }

        public int Lines
        {
            get {
                return newLineIndex.Length-1;
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
            lock (this)
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
        }

        public string CreateString()
        {
            unsafe
            {
                char[] array = chars.CreateArray();
                return new string(array);
            }
        }

        public string CreateString(int index,int length)
        {
            unsafe
            {
                char[] array = chars.CreateArray(index, length);
                return new string(array);
            }
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
            unsafe
            {
                char[] array = chars.CreateArray(GetLineStartIndex(line), GetLineLength(line));
                return array;
            }
        }

        public virtual void GetWord(int index, out int headIndex, out int length)
        {
            lock (this)
            {
                headIndex = index;
                length = 0;
                char ch = GetCharAt(index);
                if (ch == ' ' || ch == '\r' || ch == '\n' || ch == '\t') return;

                while (headIndex > 0)
                {
                    ch = GetCharAt(headIndex);
                    if (ch == ' ' || ch == '\r' || ch == '\n' || ch == '\t')
                    {
                        break;
                    }
                    headIndex--;
                }
                headIndex++;

                while (headIndex + length < Length)
                {
                    ch = GetCharAt(headIndex + length);
                    if (ch == ' ' || ch == '\r' || ch == '\n' || ch == '\t')
                    {
                        break;
                    }
                    length++;
                }
            }
        }
    }
}
