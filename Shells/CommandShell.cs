using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ajkControls
{
    public class CommandShell : Shell
    {
        public CommandShell()
        {
            initialize(
                System.Environment.GetEnvironmentVariable("ComSpec"),   // cmd.exe
                "" // @"/c dir c:\ /w"; // /c to close after execute
                );
        }

        public CommandShell(string command,string arguments)
        {
            initialize(command, arguments);
        }

        private void initialize(string command, string arguments)
        {
            process = new System.Diagnostics.Process();

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.CreateNoWindow = true;
            process.OutputDataReceived += outputDataReceived;
            process.ErrorDataReceived += errorDataReceived;

            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
        }


        System.Diagnostics.Process process = null;
        public override event ReceivedHandler LineReceived;

        public override void Start()
        {

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.StandardInput.WriteLine("prompt $P$G$_");
        }


        public override void Dispose()
        {
            process.Kill();
            process.WaitForExit();
            process.Close();
        }

        private bool logging = false;

        public override void StartLogging()
        {
            lock (logs)
            {
                logging = true;
                logs.Clear();
            }
        }

        public override void EndLogging()
        {
            lock (logs)
            {
                logging = false;
            }
        }

        public override void Execute(string command)
        {
            System.IO.StreamWriter sw = process.StandardInput;
            if (sw.BaseStream.CanWrite)
            {
                sw.WriteLine(command);
            }
        }

        List<string> logs = new List<string>();


        public override void ClearLogs()
        {
            lock (logs)
            {
                logs.Clear();
            }
        }

        public override List<string> GetLogs()
        {
            List<string> ret = new List<string>();
            lock (logs)
            {
                for(int i = 0; i < logs.Count; i++)
                {
                    ret.Add(logs[i]);
                }
            }
            return ret;
        }

        public override string GetLastLine()
        {
            lock (logs)
            {
                if (logs.Count == 0)
                {
                    return "";
                }
                else
                {
                    return logs.Last();
                }
            }
        }

        private void outputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            lock (logs)
            {
                if (!logging) logs.Clear();
                logs.Add(e.Data);
                if (LineReceived != null) LineReceived(e.Data);
            }
        }


        private void errorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            lock (logs)
            {
                if (!logging) logs.Clear();
                logs.Add(e.Data);
                if (LineReceived != null) LineReceived(e.Data);
            }
        }
    }

}
