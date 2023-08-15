using MoriPattern.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MoriPattern.Data
{
    public class GlobalData : INotifyPropertyChanged
    {
        private static GlobalData _instance;

        private bool _isWork;
        private bool _stopWork;
        private bool _useProxy;
        private bool _isFirstTime;

        private int _badRecheck;
        private int _badCount;
        private int _currentThreads;
        private int _errorCount;
        private int _goodCount;
        private int _threadsCount;
        private int _proxyRetries;
        private int _timeout;
        private int _currentSourceCount;

        private string _fileProxyPath;
        private string _fileUrlsPath;
        private string _urls;

        private ObservableCollection<string> _source;
        private List<Proxy> _proxyList;
        private ProxyType _proxyType;
        private ProgramInfo _programInfo;

        #region Properties

        public int BadCount
        {
            get => _badCount;
            set => SetProperty(ref _badCount, value);
        }


        public int BadRecheck
        {
            get => _badRecheck;
            set => SetProperty(ref _currentSourceCount, value);
        }


        public int CurrentSourceCount
        {
            get => _currentSourceCount;
            set
            {
                SetProperty(ref _currentSourceCount, value);
                OnPropertyChanged(nameof(SourceProgress));
            }
        }

        public int CurrentThreads
        {
            get => _currentThreads;
            set
            {
                SetProperty(ref _currentThreads, value);
                OnPropertyChanged(nameof(CurrentThreadCount));
            }
        }

        public int ErrorCount
        {
            get => _errorCount;
            set => SetProperty(ref _errorCount, value);
        }

        public int GoodCount
        {
            get => _goodCount;
            set => SetProperty(ref _goodCount, value);
        }

        public int ProxyRetries
        {
            get => _proxyRetries;
            set => SetProperty(ref _proxyRetries, value);
        }

        public int ThreadsCount
        {
            get => _threadsCount;
            set => SetProperty(ref _threadsCount, value);
        }

        public int Timeout
        {
            get => _timeout;
            set => SetProperty(ref _timeout, value);
        }

        public bool StopWork
        {
            get => _stopWork;
            set => SetProperty(ref _stopWork, value);
        }
        public bool IsFirstTime
        {
            get => _isFirstTime;
            set => SetProperty(ref _isFirstTime, value);
        }
        public bool IsWork
        {
            get => _isWork;
            set => SetProperty(ref _isWork, value);
        }

        public bool UseProxy
        {
            get => _useProxy;
            set
            {
                SetProperty(ref _useProxy, value);
                OnPropertyChanged(nameof(ProxyCount));
            }
        }

        public string FileProxyPath
        {
            get => _fileProxyPath;
            set => SetProperty(ref _fileProxyPath, value);
        }

        public string FileUrlsPath
        {
            get => _fileUrlsPath;
            set => SetProperty(ref _fileUrlsPath, value);
        }

        public string Urls
        {
            get => _urls;
            set => SetProperty(ref _urls, value);
        }
        public ProgramInfo ProgramInfo
        {
            get => _programInfo ??= new();
            set => _programInfo = value;
        }
        public ProxyType ProxyType
        {
            get => _proxyType;
            set => SetProperty(ref _proxyType, value);
        }

        public ObservableCollection<string> Source
        {
            get => _source;
            set
            {
                SetProperty(ref _source, value);
                OnPropertyChanged(nameof(SourceCount));
            }
        }

        public string ProxyCount => UseProxy ? ProxyList.Count.ToString() : "-";

        public string CurrentThreadCount => CurrentThreads == 0 ? "-" : CurrentThreads.ToString();

        public string SourceCount => Source.Count.ToString();

        public string SourceProgress =>
            (CurrentSourceCount == Source.Count && Source.Count != 0)
                ? "Finished"
                : CurrentSourceCount != Source.Count && Source.Count != 0
                    ? $@"[{CurrentSourceCount}/{SourceCount}]"
                    : "";

        public List<Proxy> ProxyList
        {
            get => _proxyList ??= new();
            set
            {
                _proxyList = value;
                OnPropertyChanged(nameof(ProxyList));
                OnPropertyChanged(nameof(ProxyCount));
            }
        }

        #endregion Properties

        #region Constructors

        private GlobalData()
        {
            ErrorCount = 0;
            ProxyType = ProxyType.Socks4;
            FileProxyPath = string.Empty;
            Urls = string.Empty;
            FileUrlsPath = string.Empty;
            CurrentThreads = 0;
            _source = new ObservableCollection<string>();
            _stopWork = false;
            _isWork = false;
            _proxyRetries = 0;
            _badRecheck = 0;
        }

        #endregion Constructors

        #region Singleton Instance

        public static GlobalData Instance
        {
            get
            {
                _instance ??= new GlobalData();
                return _instance;
            }
        }

        #endregion Singleton Instance

        #region Event Handling

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        #endregion Event Handling

        #region Other Methods

        public static List<ProxyType> ProxyTypeValues => Enum.GetValues(typeof(ProxyType)).Cast<ProxyType>().ToList();



        #endregion Other Methods
    }
}