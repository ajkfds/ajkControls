using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ajkControls
{
    public partial class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();
        }

        private Log log = new Log();

        private void dbDrawBox_Resize(object sender, EventArgs e)
        {
            dbDrawBox.Invalidate();
        }

        private List<string> logsFromAnotherThread = new List<string>();
        public void AppendLogLineFromThread(string logLine)
        {
            lock (logsFromAnotherThread)
            {
                logsFromAnotherThread.Add(logLine);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lock (logsFromAnotherThread)
            {
                if(logsFromAnotherThread.Count == 0)
                {
                    return;
                }

                foreach(string line in logsFromAnotherThread)
                {
                    log.AppendLogLine(line);
                }
                logsFromAnotherThread.Clear();
            }
            dbDrawBox.Invalidate();
        }

        public void AppendLogLine(string logLine)
        {
            log.AppendLogLine(logLine);
            dbDrawBox.Invalidate();
        }


        int startLine = 0;

        private void dbDrawBox_DoubleBufferedPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);

            TextFormatFlags flags = TextFormatFlags.Top | TextFormatFlags.WordBreak;

            int i = 0;
            int y = 0;
            while (i<log.LineCount && y<dbDrawBox.Height)
            {
                string line = log.LineText(i+startLine);
                if (line == "") line = " ";
                Size size = System.Windows.Forms.TextRenderer.MeasureText(e.Graphics,line, Font,new Size(dbDrawBox.Width,dbDrawBox.Height),flags);
                Rectangle rect = new Rectangle(0, y, dbDrawBox.Width, dbDrawBox.Height);
                System.Windows.Forms.TextRenderer.DrawText(e.Graphics, line, Font, rect, Color.Gray,flags);
                y = y + size.Height;
                i++;
            }
            if (y >= dbDrawBox.Height)
            {
                startLine++;
                Invalidate();
            }
        }


    }
}
