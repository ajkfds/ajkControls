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

            treeView.TreeNodes.Add(branchesNode);
            treeView.TreeNodes.Add(remotesNode);
            treeView.TreeNodes.Add(stashesNode);

            shell.Start();
            shell.Execute("prompt GitPanel$G");
            shell.LineReceived += shell_Received;

            mustRefresh = true;
        }
        private string path;
        private FolderTreeNode branchesNode = new FolderTreeNode("Branch", IconImage.ColorStyle.Gray);
        private FolderTreeNode remotesNode = new FolderTreeNode("Remote", IconImage.ColorStyle.Gray);
        private FolderTreeNode stashesNode = new FolderTreeNode("Stash", IconImage.ColorStyle.Gray);

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
            shell.StartLogging();
            shell.Execute("cd " + path);
            shell.Execute("");
            while (!shell.GetLastLine().StartsWith("GitPanel>"))
            {
                System.Threading.Thread.Sleep(10);
            }
            shell.ClearLogs();
            shell.Execute(@"git branch");
            shell.Execute("echo <complete>");
            while (!shell.GetLastLine().EndsWith("echo <complete>"))
            {
                System.Threading.Thread.Sleep(10);
            }
            List<string> lines = shell.GetLogs();

            branchesNode.TreeNodes.Clear();
            foreach(string line  in lines)
            {
                if(line.StartsWith("GitPanel>")) continue;
                branchesNode.TreeNodes.Add(new BranchNode(line));
            }
            


            shell.EndLogging();
        }

        private void refreshLog()
        {
            getStatus();

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
            Color defaultColor = Color.FromArgb(80, 80, 80);

            List<Connection> connections = new List<Connection>();

            shell.EndLogging();
            tableView.TableItems.Clear();

            int treeDepth = 1;

            foreach (string line in lines)
            {
                Primitive.ColoredString cs = new Primitive.ColoredString(line);

                GitCommit commit = GitCommit.Create(cs.Text,Color.AliceBlue);
                if (commit == null)
                {
                    if (!cs.Text.StartsWith("|")) continue;

                    List<Connection> newconnections = new List<Connection>();
                    for (int i = 0; i < cs.Text.Length; i++)
                    {
                        switch (cs.Text[i])
                        {
                            case '|':
                                for (int j = 0; j < connections.Count; j++)
                                {
                                    if (connections[j].To == i && (connections[j].Color == defaultColor | connections[j].Color == cs.Colors[i]))
                                    {
                                        newconnections.Add(new Connection(connections[j].From, i,cs.Colors[i]));
                                    }
                                }
                                break;
                            case '\\':
                                for (int j = 0; j < connections.Count; j++)
                                {
                                    if(connections[j].To == i - 1 && (connections[j].Color == defaultColor | connections[j].Color == cs.Colors[i]))
                                    {
                                        newconnections.Add(new Connection(connections[j].From, i + 1, cs.Colors[i]));
                                    }
                                }
                                break;
                            case '/':
                                for (int j = 0; j< connections.Count;j++)
                                {
                                    if (connections[j].To == i + 1 && (connections[j].Color == defaultColor | connections[j].Color == cs.Colors[i]))
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
                            newconnections.Add(new Connection(i,i, defaultColor));
                        }
                    }
                    connections = newconnections;
                    if (treeDepth < connections.Count) treeDepth = connections.Count;
                }
            }
            // color conversion

            for (int i = tableView.TableItems.Count - 1; i >= 0; i--)
            {
                if (i == tableView.TableItems.Count - 1) continue;
                foreach (Connection connection in (tableView.TableItems[i] as GitCommit).Connections)
                {
                    if (connection.Color != defaultColor)
                    {
                        connection.Color = colorTable[connection.Color];
                        continue;
                    }
                    int con = connection.To;
                    foreach (Connection nextConnection in (tableView.TableItems[i + 1] as GitCommit).Connections)
                    {
                        if (nextConnection.Color == defaultColor) continue;
                        if (nextConnection.From == con) connection.Color = nextConnection.Color;
                    }
                }
            }
            tableView.Widths[0] = (treeDepth+1) * tableView.LineHeight;
            //            tableView.Refresh();
        }

        private Dictionary<System.Drawing.Color, System.Drawing.Color> colorTable
            = new Dictionary<Color, Color> {
                {Color.Black,   Color.FromArgb(100,100,100)},
                {Color.Red,     Color.FromArgb(200,100,100)},
                {Color.Green,   Color.FromArgb(100,200,100)},
                {Color.Yellow,  Color.FromArgb(200,200,50)},
                {Color.Blue,    Color.FromArgb(100,100,200)},
                {Color.Magenta, Color.FromArgb(200,100,200)},
                {Color.Cyan,    Color.FromArgb(100,200,200)},
                {Color.White,   Color.FromArgb(200,200,200)},
            };

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
