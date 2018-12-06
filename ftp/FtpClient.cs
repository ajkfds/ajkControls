using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ajkControls
{
    public class FtpClient : FileTransfer
    {
        public FtpClient(string serverUri)
        {
            this.serverUri = serverUri;
        }
        private string serverUri;

        public override bool DownloadFile(string localPath,string serverPath)
        {
            System.Net.FtpWebRequest ftpRequest = (System.Net.FtpWebRequest)
            System.Net.WebRequest.Create( new Uri(serverUri+serverPath));
            ftpRequest.Credentials = new System.Net.NetworkCredential("username", "password");
            ftpRequest.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
            ftpRequest.KeepAlive = KeepAlive;
            ftpRequest.UseBinary = false;
            ftpRequest.UsePassive = false;

            using (System.Net.FtpWebResponse ftpResponse = (System.Net.FtpWebResponse)ftpRequest.GetResponse())
            {
                using (System.IO.Stream responseStream = ftpResponse.GetResponseStream())
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(localPath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                    {
                        byte[] buffer = new byte[1024];
                        while (true)
                        {
                            int readSize = responseStream.Read(buffer, 0, buffer.Length);
                            if (readSize == 0) break;
                            fs.Write(buffer, 0, readSize);
                        }
                    }

                }
                if(ftpResponse.StatusCode != System.Net.FtpStatusCode.CommandOK && ftpResponse.StatusCode != System.Net.FtpStatusCode.FileActionOK)
                {
                    OnMessageReceived(new MessageReceivedEventArgs(ftpResponse.StatusDescription));
                    return false;
                }
            }

            OnMessageReceived(new MessageReceivedEventArgs(serverUri+serverPath+" download complete"));
            return true;
        }


    }
}
