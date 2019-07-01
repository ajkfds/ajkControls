using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ajkControls.Git
{
    public class GitCommit : ajkControls.TableView.TableItem
    {
        protected GitCommit() { }

        //public static GitCommit CreateBlank(string line)
        //{
        //    if (!line.StartsWith("|")) return null;
        //    GitCommit commit = new GitCommit();
        //    commit.Tree = line;
        //    return commit;
        //}

        public static GitCommit Create(string line,Color color,int fromIndent)
        {
            GitCommit commit = Create(line, color);
            if (commit == null) return null;
            commit.FromIndent = fromIndent;

            return commit;
        }

        public static GitCommit Create(string line, Color color)
        {
            GitCommit commit = new GitCommit();
            if (!line.Contains(" <")) return null;
            if (!line.Contains("*")) return null;

            string s = line;
            int i = s.IndexOf(" <");

            commit.Tree = s.Substring(0, s.IndexOf(" <"));
            commit.Indent = s.IndexOf("*");

            s = s.Substring(s.IndexOf(" <") + 2);
            commit.CommitHash = s.Substring(0, 40);

            s = s.Substring(s.IndexOf(" <") + 2);
            commit.CommitDate = DateTime.Parse(s.Substring(0, 19));

            s = s.Substring(s.IndexOf(" <") + 2);
            commit.AuthorName = s.Substring(0, s.IndexOf(">"));

            s = s.Substring(s.IndexOf(">") + 2);
            commit.Subject = s;
            commit.Color = color;

            return commit;
        }

        public DateTime CommitDate;
        public string Subject;
        public string CommitHash;
        public string AuthorName;
        public int Indent = 0;
        public int FromIndent = -1;
        public string Tree;
        public Color Color = Color.AliceBlue;
        public List<GitPanel.Connection> Connections;

        public override void Draw(Graphics g, Font font, List<Rectangle> rectangles)
        {
            SolidBrush stringBrush = new SolidBrush(Color.DarkGray);
            SolidBrush dotBrush = new SolidBrush(Color.White);
            Pen treePen = new Pen(Color.FromArgb(100,Color.White));

            int size = rectangles[0].Height;
            //int r = size / 4;
            int x = rectangles[0].Left;
            int y = rectangles[0].Top;


            if (CommitHash == null) return;

            if (Connections != null)
            {
                foreach (GitPanel.Connection connection in Connections)
                {
                    treePen = new Pen(Color.FromArgb(100, connection.Color),4);
                    g.DrawLine(treePen, 
                        rectangles[0].Left + size / 2 + size/2 * connection.From, 
                        y - size/2,
                        rectangles[0].Left + size / 2 + size/2 * connection.To,
                        y + size/2+2
                        );
                }
            }

            g.DrawString(CommitHash.Substring(0, 6), font, stringBrush, rectangles[1]);
            g.DrawString(CommitDate.ToString(), font, stringBrush, rectangles[2]);
            g.DrawString(AuthorName, font, stringBrush, rectangles[3]);
            g.DrawString(Subject, font, stringBrush, rectangles[4]);
        }

        public override void PostDraw(Graphics g, Font font, List<Rectangle> rectangles)
        {
            SolidBrush stringBrush = new SolidBrush(Color.DarkGray);
            SolidBrush dotBrush = new SolidBrush(Color.White);
            Pen treePen = new Pen(Color.FromArgb(100, Color.White));

            int size = rectangles[0].Height;
            //int r = size / 4;
            int x = rectangles[0].Left;
            int y = rectangles[0].Top;

            if (CommitHash == null) return;

            g.FillEllipse(
                dotBrush,
                rectangles[0].Left + size / 2 + size / 2 * Indent - size / 4,
                y + size / 4,
                size / 2,
                size / 2
                );
        }
    }
}
