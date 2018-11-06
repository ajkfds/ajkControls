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
        }

        ResizableArray<char> chars = new ResizableArray<char>(1024, 256);
        ResizableArray<byte> colors = new ResizableArray<byte>(1024, 256);
        ResizableArray<byte> marks = new ResizableArray<byte>(1024, 256);
        ResizableArray<int> newLineIndex = new ResizableArray<int>(256, 256);

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

        public void CopyCharsFrom(Document document)
        {
            chars.CopyFrom(document.chars);
            colors.Resize(document.Length);
            marks.Resize(document.Length);
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

            int startLine = GetLineAt(index);
            int endLine = GetLineAt(index + replaceLength);
            int changedLine = lines.Count+startLine- endLine;

            if(changedLine > 0)
            {
                newLineIndex.Resize(newLineIndex.Length + changedLine);

                for (int i = newLineIndex.Length - 1; i >= startLine + lines.Count; i--)
                {
                    newLineIndex[i] = newLineIndex[i - changedLine] + text.Length - replaceLength;
                }
                for (int i = 0; i < lines.Count; i++)
                {
                    newLineIndex[startLine + i] = lines[i];
                }
            }
            else if (changedLine < 0)
            {
                for( int i = startLine + lines.Count; i < newLineIndex.Length + changedLine;i++)
                {
                    newLineIndex[i] = newLineIndex[i - changedLine] + text.Length - replaceLength;
                }
                for (int i = 0; i < lines.Count; i++)
                {
                    newLineIndex[startLine + i] = lines[i];
                }

                newLineIndex.Resize(newLineIndex.Length + changedLine);
            }
            else
            {
                for (int i = newLineIndex.Length + changedLine - 1; i >= startLine + lines.Count; i--)
                {
                    newLineIndex[i] = newLineIndex[i - changedLine] + text.Length - replaceLength;
                }
                for (int i = 0; i < lines.Count; i++)
                {
                    newLineIndex[startLine + i] = lines[i];
                }
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
            for (int line = 0; line < newLineIndex.Length; line++)
            {
                if (newLineIndex[line] >= index) return line;
            }
            return newLineIndex.Length;
        }


        public int GetLineStartIndex(int line)
        {
            System.Diagnostics.Debug.Assert(line <= newLineIndex.Length + 1);
            if(line == 0)
            {
                return 0;
            }
            else
            {
                return newLineIndex[line - 1]+1;
            }
        }

        public int GetLineLength(int line)
        {
//            System.Diagnostics.Debug.Assert(line < newLineIndex.Length + 1);
            if (line == 0)
            {
                if(newLineIndex.Length == 0)
                {
                    return chars.Length;
                }
                else
                {
                    return newLineIndex[line];
                }
            }
            else if ( line == newLineIndex.Length)
            {
                return chars.Length - newLineIndex[newLineIndex.Length - 1]-1;
            }
            else if (line == newLineIndex.Length + 1)
            {
                return 0;
            }
            else
            {
                return newLineIndex[line] - newLineIndex[line - 1];
            }
        }

        public int Lines
        {
            get {
                return newLineIndex.Length+1;
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
