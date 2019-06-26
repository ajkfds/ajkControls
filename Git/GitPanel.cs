using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ajkControls.Git
{
    public partial class GitPanel : UserControl,IDisposable
    {
        public GitPanel(string repositoryPath)
        {
            path = repositoryPath;
            InitializeComponent();
            shell.Start();

            shell.LineReceived += shell_Received;

            mustRefresh = true;
        }
        private string path;

        public new void Dispose()
        {
            shell.Dispose();
            base.Dispose();
        }

        public void RefreshLog()
        {
            refreshLog();
        }

        public void Fetch()
        {
            fetch();
        }


        private ajkControls.CommandShell shell = new ajkControls.CommandShell();

        volatile bool mustRefresh = false;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mustRefresh)
            {
                refreshLog();
                mustRefresh = false;
            }
        }

        public class Connection
        {
            public Connection(int from,int to,System.Drawing.Color color)
            {
                From = from;
                To = to;
                Color = color;
            }
            public int From;
            public int To;
            public System.Drawing.Color Color;
        }

        private void fetch()
        {
            shell.Execute("cd " + path);
            shell.Execute("");
            shell.StartLogging();
            shell.Execute(@"git fetch");
            shell.Execute("echo <complete>");
            while (!shell.GetLastLine().EndsWith("echo <complete>"))
            {
                System.Threading.Thread.Sleep(10);
            }
            shell.EndLogging();
        }
        private void getStatus()
        {
            shell.Execute("cd " + path);
            shell.Execute("");
            shell.StartLogging();
            shell.Execute(@"git branch");
            shell.Execute("echo <complete>");
            while (!shell.GetLastLine().EndsWith("echo <complete>"))
            {
                System.Threading.Thread.Sleep(10);
            }
            List<string> lines = shell.GetLogs();

            shell.EndLogging();
        }

        private void refreshLog()
        {
            shell.Execute("cd " + path);
            shell.Execute("");
            shell.StartLogging();
            shell.Execute(@"git log --pretty=""format:<%H> <%cd> <%an> %s %d "" --date=format:""%Y/%m/%d %H:%M:%S"" --all --graph --color");
            shell.Execute("echo <complete>");
            while (!shell.GetLastLine().EndsWith("echo <complete>"))
            {
                System.Threading.Thread.Sleep(10);
            }
            List<string> lines = shell.GetLogs();


            List<Connection> connections = new List<Connection>();

            shell.EndLogging();
            tableView.TableItems.Clear();

            foreach (string line in lines)
            {
                Primitive.ColoredString cs = new Primitive.ColoredString(line);

                GitCommit commit = GitCommit.Create(cs.Text,Color.AliceBlue);
                if (commit == null)
                {
                    commit = GitCommit.CreateBlank(cs.Text);
                    if (commit == null) continue;

                    List<Connection> newconnections = new List<Connection>();
                    for (int i = 0; i < commit.Tree.Length; i++)
                    {
                        switch (commit.Tree[i])
                        {
                            case '|':
                                for (int j = 0; j < connections.Count; j++)
                                {
                                    if (connections[j].To == i)
                                    {
                                        newconnections.Add(new Connection(connections[j].From, i,cs.Colors[i]));
                                    }
                                }
                                break;
                            case '\\':
                                for (int j = 0; j < connections.Count; j++)
                                {
                                    if(connections[j].To == i - 1)
                                    {
                                        newconnections.Add(new Connection(connections[j].From, i + 1, cs.Colors[i]));
                                    }
                                }
                                break;
                            case '/':
                                for (int j = 0; j< connections.Count;j++)
                                {
                                    if (connections[j].To == i + 1)
                                    {
                                        newconnections.Add(new Connection(connections[j].From, i - 1, cs.Colors[i]));
                                    }
                                }
                                break;
                            case ' ':
                                break;
                        }
                    }
                    connections = newconnections;
                } else {
                    // normal commit
                    tableView.TableItems.Add(commit);

                    commit.Connections = connections;
                    var newconnections = new List<Connection>();

                    for(int i=0;i<commit.Tree.Length;i++)
                    {
                        if (commit.Tree[i] == '|')
                        {
                            for (int j = 0; j < connections.Count; j++)
                            {
                                if (connections[j].To == i)
                                {
                                    newconnections.Add(new Connection(i, i, cs.Colors[i]));
                                }
                            }
                        }else 
                        if (commit.Tree[i] != ' ')
                        {
                            newconnections.Add(new Connection(i,i,Color.White));
                        }
                    }
                    connections = newconnections;
                }
            }
            tableView.Refresh();
        }

        private static List<Color> colorTable = new List<Color> { Color.AliceBlue, Color.Red, Color.Green, Color.Yellow, Color.Black };

        private void shell_Received(string line)
        {
            logView.AppendLogLine(line);
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void PullBtn_Click(object sender, EventArgs e)
        {
            refreshLog();
        }

        private void FetchBtn_Click(object sender, EventArgs e)
        {
            fetch();
        }


        //private void PullButton_Click(object sender, EventArgs e)
        //{
        //    string path = project.GetAbsolutePath("");
        //    shell.Execute("cd " + path);
        //    shell.Execute("");
        //    shell.StartLogging();
        //    shell.Execute(@"git pull origin master");
        //    shell.Execute("echo <complete>");
        //    while (!shell.GetLastLine().EndsWith("echo <complete>"))
        //    {
        //        System.Threading.Thread.Sleep(10);
        //    }
        //    List<string> lines = shell.GetLogs();
        //    shell.EndLogging();

        //    //            refreshLog();
        //}
    }
}
