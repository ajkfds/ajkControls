using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ajkControls
{
    public abstract class FileTransfer: IDisposable
    {
        public class MessageReceivedEventArgs : EventArgs
        {
            public MessageReceivedEventArgs(string message)
            {
                this.Message = message;
            }

            public string Message;
        }

        public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);
        public event MessageReceivedEventHandler MessageReceived;

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            if (MessageReceived != null) MessageReceived(this, e);
        }

        public void Dispose() { }

        public bool KeepAlive { get; set; }

        public virtual bool DownloadFile(string localPath, string serverPath)
        {
            return false;
        }

        public virtual bool UploadFile(string serverPath, string localPath)
        {
            return false;
        }

        public virtual bool GetFileList(string serverPath, out List<string> fileList)
        {
            fileList = new List<string>();
            return false;
        }

        public virtual bool GetLastUpdate(string serverPath, out DateTime dateTime)
        {
            dateTime = DateTime.MinValue;
            return false;
        }

        public virtual bool MakeDirectory(string serverPath)
        {
            return false;
        }
    }
}
