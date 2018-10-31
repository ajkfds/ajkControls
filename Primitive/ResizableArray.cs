using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ajkControls
{
    public class ResizableArray<T>
    {
        public ResizableArray(int initialBufferSize,int minBufferExpandSize)
        {
            bufferSize = initialBufferSize;
            buffer = new T[initialBufferSize];
            this.minBufferExpandSize = minBufferExpandSize;

            if(typeof(T) == typeof(char))
            {
                tSize = 2;
            }
            else
            {
                tSize = System.Runtime.InteropServices.Marshal.SizeOf(buffer.GetType().GetElementType());
            }
        }

        private int tSize;
        private int minBufferExpandSize;

        private int bufferSize;
        private T[] buffer;
        private int usedLength = 0;

        public int Length
        {
            get
            {
                return usedLength;
            }
        }

        public void Replace(int index, int replaceLength, T[] items)
        {
            int changedSize = items.Length - replaceLength;
            if (changedSize > 0)
            {
                expandBufferSize(changedSize);
                shiftBuffer(index, changedSize);
                usedLength = usedLength + changedSize;
            }
            else if (changedSize < 0)
            {
                shiftBuffer(index+replaceLength, changedSize);
                usedLength = usedLength + changedSize;
            }
            Array.Copy(items, 0, buffer, index, items.Length);
        }

        public T this[int index]
        {
            get
            {
                if(index > usedLength)
                { // unused buffer access
                    System.Diagnostics.Debugger.Break();
                }
                return buffer[index];
            }
            set
            {
                buffer[index] = value;
            }
        }

        public T[] CreateArray()
        {
            T[] array = new T[usedLength];
            Array.Copy(buffer, 0 ,array, 0, usedLength);
            return array;
        }

        public T[] CreateArray(int index,int length)
        {
            T[] array = new T[length];
            Array.Copy(buffer, index, array, 0, length);
            return array;
        }


        private void expandBufferSize(int expandLength)
        {
            if (usedLength+expandLength > bufferSize)
            {
                if(expandLength < minBufferExpandSize)
                {
                    bufferSize = bufferSize + minBufferExpandSize;
                }
                else
                {
                    bufferSize = bufferSize + expandLength;
                }
                Array.Resize<T>(ref buffer, bufferSize);
            }
        }

        public void Resize(int size)
        {
            if (size <= bufferSize)
            {
                usedLength = size;
            }
            else
            {
                usedLength = size;
                bufferSize = usedLength;
                Array.Resize<T>(ref buffer, bufferSize);
            }
        }

        private void shiftBuffer(int startIndex, int shiftLength)
        {
            //  almost same speed (array.copy vs buffer.blockcopy)
//           Array.Copy(buffer, startIndex, buffer, startIndex + shiftLength, usedLength - startIndex);
            Buffer.BlockCopy(buffer, startIndex*tSize, buffer, (startIndex + shiftLength)*tSize, (usedLength - startIndex)*tSize);
        }

        public void CopyFrom(ResizableArray<T> resizableArray)
        {
            if(Length < resizableArray.Length)
            {
                expandBufferSize(resizableArray.Length-Length);
            }
            usedLength = resizableArray.Length;
            Buffer.BlockCopy(resizableArray.buffer, 0, buffer, 0, usedLength * tSize);
        }

    }
}
