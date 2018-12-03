using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ajkControls
{
    public class Log :IDisposable
    {
        public Log()
        {
//            filePath = System.Guid.NewGuid().ToString();
//            sw = new System.IO.StreamWriter(filePath);
        }

        public void Dispose()
        {
//            sw.Dispose();
 //           System.IO.File.Delete(filePath);
        }

        //        private string filePath;
        //        private System.IO.StreamWriter sw;
        private int startLine = 0;
        private List<string> lineCache = new List<string>();
        private const int casheLines = 100;

        public void AppendLogLine(string logLine)
        {
            lineCache.Add(logLine);
            if(lineCache.Count > casheLines)
            {
                lineCache.RemoveAt(0);
                startLine++;
            }
        }

        public string LineText(int line)
        {
            if (line < startLine)
            {
                return "";
            }else if(line - startLine >= lineCache.Count)
            {
                return "";
            }
            else
            {
                return lineCache[line - startLine];
            }
        }

        public int LineCount
        {
            get
            {
                return lineCache.Count + startLine;
            }
        }
    }
}
