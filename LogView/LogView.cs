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
            vScrollBar.Width = Global.ScrollBarWidth;
        }

        private List<string> logs = new List<string>();
        private List<System.Drawing.Color> logColors = new List<Color>();
        private List<int> logHeights = new List<int>();

        private int maxLogs = 200;
        public int MaxLogs {
            get
            {
                return maxLogs;
            }
            set
            {
                maxLogs = value;
            }
        }

        private void appendLog(string log,Color color)
        {
            logs.Add(log);
            logColors.Add(color);
            logHeights.Add(-1);
            while (logs.Count > MaxLogs)
            {
                logs.RemoveAt(0);
                logColors.RemoveAt(0);
                logHeights.RemoveAt(0);
            }
        }


        private void dbDrawBox_Resize(object sender, EventArgs e)
        {
            dbDrawBox.Invalidate();
            for (int i = 0; i < logHeights.Count; i++)
            {
                logHeights[i] = -1;
            }
        }

        private List<string> logStock = new List<string>();
        private List<Color> logColorStock = new List<Color>();
        public void AppendLogLine(string logLine)
        {
            lock (logStock)
            {
                logStock.Add(logLine);
                logColorStock.Add(ForeColor);
            }
        }
        public void AppendLogLine(string logLine,Color color)
        {
            lock (logStock)
            {
                logStock.Add(logLine);
                logColorStock.Add(color);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lock (logStock)
            {
                if(logStock.Count == 0)
                {
                    return;
                }

                for(int i = 0; i < logStock.Count; i++)
                {
                    appendLog(logStock[i], logColorStock[i]);
                }

                logStock.Clear();
                logColorStock.Clear();
            }
            dbDrawBox.Invalidate();
        }

        private void dbDrawBox_DoubleBufferedPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);

            TextFormatFlags flags = TextFormatFlags.Top | TextFormatFlags.WordBreak;

            for(int i = 0; i < logs.Count; i++)
            {
                if (logHeights[i] != -1) continue;
                string line = logs[i];
                if (line == "") line = " ";
                Size size = System.Windows.Forms.TextRenderer.MeasureText(e.Graphics, line, Font, new Size(dbDrawBox.Width, dbDrawBox.Height), flags);
                logHeights[i] = size.Height;
            }

            if (logHeights.Count == 0) return;
            int startline = logHeights.Count - 1;

            int y = 0;
            while (startline > 0)
            {
                y = y + logHeights[startline];
                if (y >= dbDrawBox.Height)
                {
                    startline++;
                    break;
                }
                startline--;
            }

            y = 0;
            int j = startline;
            while (j < logs.Count && y<dbDrawBox.Height)
            {
                string line = logs[j];
                Rectangle rect = new Rectangle(0, y, dbDrawBox.Width, dbDrawBox.Height);
                System.Windows.Forms.TextRenderer.DrawText(e.Graphics, line, Font, rect, logColors[j],flags);
                y = y + logHeights[j];
                j++;
            }
        }


    }
}
