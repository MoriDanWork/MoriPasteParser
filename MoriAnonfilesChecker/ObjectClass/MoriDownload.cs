using MoriAnonfilesChecker.Object_Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace MoriAnonfilesChecker.ProgramLogic
{
    class MoriDownload
    {
        private MoriFile file;
        private MainWindow mainWindow;
        private int TryCount = 1;
        private string FileName;

        public MoriDownload(MoriFile file, MainWindow mainWindow)
        {
            this.file = file;
            this.mainWindow = mainWindow;
            FileName = @"Files\[" + DateTime.Now.ToString("dd-MM HH.mm.ss.f") + "] " + file.Name;
        }

        public void Download()
        {
            using (WebClient myWebClient = new WebClient())
            {
                if (!Directory.Exists("Files"))
                    System.IO.Directory.CreateDirectory("Files");
                try
                {
                    if(file.DownloadRetry == 0) mainWindow.ChangeStatus(file.Uid, "Downloading..");
                    myWebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    myWebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                    myWebClient.DownloadFileAsync(new System.Uri(file.DownloadURL), FileName);
                }
                catch (WebException)
                {
                    Retry();
                }
            }
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            mainWindow.ChangeStatus(file.Uid, e.ProgressPercentage+"%");
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                mainWindow.ChangeStatus(file.Uid, "✔️");
                Interlocked.Decrement(ref DownloadLogic.CurrentThreads);
                DownloadLogic.Check();
                DownloadLogic.Core();
            }
            else
            {
                Retry();
            }
        }

        private void Retry()
        {
            if (file.DownloadRetry < TryCount)
            {
                file.DownloadRetry += 1;
                mainWindow.ChangeStatus(file.Uid, $"Attempt[{file.DownloadRetry}/{TryCount}]..");
                Download();
            }
            else
            {
                mainWindow.ChangeStatus(file.Uid, $"Error..");
                Interlocked.Decrement(ref DownloadLogic.CurrentThreads);
                DownloadLogic.Check();
                DownloadLogic.Core();
            }
        }
    }
}
