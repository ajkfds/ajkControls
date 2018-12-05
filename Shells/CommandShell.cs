using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ajkControls
{
    public class CommandShell : Shell
    {
        System.Diagnostics.Process process = null;
        public override event ReceivedHandler LineReceived;

        public override void Start()
        {
            process = new System.Diagnostics.Process();

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.CreateNoWindow = true;
            process.OutputDataReceived += outputDataReceived;
            process.ErrorDataReceived += errorDataReceived;

            process.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec"); // cmd.exe
            process.StartInfo.Arguments = "";// @"/c dir c:\ /w"; // /c to close after execute

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

        public override void Execute(string command)
        {
            System.IO.StreamWriter sw = process.StandardInput;
            if (sw.BaseStream.CanWrite)
            {
                sw.WriteLine(command);
            }
        }

        List<string> logs = new List<string>();
        private const int maxLogs = 100;

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
                logs.Add(e.Data);
                if (logs.Count > maxLogs) logs.RemoveAt(0);
                if (LineReceived != null) LineReceived(e.Data);
            }
        }


        private void errorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            lock (logs)
            {
                logs.Add(e.Data);
                if (logs.Count > maxLogs) logs.RemoveAt(0);
                if (LineReceived != null) LineReceived(e.Data);
            }
        }
    }

}
