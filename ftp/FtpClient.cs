using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ajkControls.ftp
{
    public class FtpClient : FileTransfer
    {
        public FtpClient(string serverUri,string userName,string password)
        {
            this.serverUri = serverUri;
            if (!serverUri.EndsWith("/")) serverUri = serverUri + "/";

            this.userName = userName;
            this.password = password;
        }

        private string serverUri;
        private string userName;
        private string password;

        public override bool DownloadFile(string localPath,string serverPath)
        {
            System.Net.FtpWebRequest ftpRequest = (System.Net.FtpWebRequest)
            System.Net.WebRequest.Create( new Uri(serverUri+"%20"+serverPath));
            ftpRequest.Credentials = new System.Net.NetworkCredential(userName, password);
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

        public override bool UploadFile(string serverPath, string localPath)
        {
            System.Net.FtpWebRequest ftpReq = (System.Net.FtpWebRequest)
            System.Net.WebRequest.Create(new Uri(serverUri+"%20"+serverPath));

            ftpReq.Credentials = new System.Net.NetworkCredential(userName, password);
            ftpReq.Method = System.Net.WebRequestMethods.Ftp.UploadFile;

            ftpReq.KeepAlive = KeepAlive;
            ftpReq.UseBinary = false;
            ftpReq.UsePassive = false;

            using (System.IO.Stream reqStrm = ftpReq.GetRequestStream())
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(localPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        int readSize = fs.Read(buffer, 0, buffer.Length);
                        if (readSize == 0)
                            break;
                        reqStrm.Write(buffer, 0, readSize);
                    }
                }
            }

            using (System.Net.FtpWebResponse ftpResponse = (System.Net.FtpWebResponse)ftpReq.GetResponse())
            {
                if (ftpResponse.StatusCode != System.Net.FtpStatusCode.CommandOK && ftpResponse.StatusCode != System.Net.FtpStatusCode.FileActionOK)
                {
                    OnMessageReceived(new MessageReceivedEventArgs(ftpResponse.StatusDescription));
                    return false;
                }
            }
            return true;
        }

        public bool Rename(string fromServerPath,string toServerPath)
        {
            System.Net.FtpWebRequest ftpReq = (System.Net.FtpWebRequest)
            System.Net.WebRequest.Create(new Uri(serverUri + "%20" + fromServerPath));

            ftpReq.Credentials = new System.Net.NetworkCredential(userName, password);
            ftpReq.Method = System.Net.WebRequestMethods.Ftp.Rename;

            ftpReq.RenameTo = toServerPath;

            using (System.Net.FtpWebResponse ftpResponse = (System.Net.FtpWebResponse)ftpReq.GetResponse())
            {
                if (ftpResponse.StatusCode != System.Net.FtpStatusCode.CommandOK && ftpResponse.StatusCode != System.Net.FtpStatusCode.FileActionOK)
                {
                    OnMessageReceived(new MessageReceivedEventArgs(ftpResponse.StatusDescription));
                    return false;
                }
            }
            return true;
        }

    }
}
