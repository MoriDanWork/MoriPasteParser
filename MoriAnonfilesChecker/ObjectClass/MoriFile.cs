using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MoriAnonfilesChecker.Object_Class
{
    public class MoriFile : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string SizeReadable { get; set; }
        public string DownloadURL { get; set; }
        public string Extension { get; set; }
        private string status;
        public string Status 
        {
            get { return status; }
            set { if(value != status)
                {
                    status = value;
                    OnPropertyChanged("Status");
                }
            }
        }
        public string Uid { get; set; }
        public string Url { get; set; }
        public int DownloadRetry { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
