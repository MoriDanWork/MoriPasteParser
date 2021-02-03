using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using static MoriAnonfilesChecker.ProgramLogic.ProxyParseLogic;

namespace MoriAnonfilesChecker.ProgramLogic
{
    public static class SettingsLogic
    {
        private const string currentVersion = "1.0.0";
        private const int deepMax = 15;
        private const int deepMin = 1;
        private const int threadMax = 15;
        private const int threadMin = 1;
        private const int attempsMax = 20;
        private const int attempsMin = 1;
        private const int minSize = 1;
        private const int maxSize = 1000;
        private static int downloadThreadsCount = 3;
        private static int parseThreadsCount = 3;
        private static int deep = 5;
        private static int downloadAttemps = 3;
        private static int fileSize = 15;
        private static int existFilter = 0;
        private static int sizeFilter = 0;
        private static int filterCount = 0;
        private static int extensionFilter = 0;
        private static int proxyCount = 0;
        private static int errorCount = 0;
        private static int progressCount = 0;
        private static int linksCount = 0;
        private static int badUrl = 0;
        private static string extenstion = ".txt;.rar";
        private static string progressReadably = $"Awaiting start..";
        private static string searchWord = "Good";
        private static List<Proxy> proxies = new List<Proxy>();
        private static string currentPath = Directory.GetCurrentDirectory();
        private static string loadPath = Directory.GetCurrentDirectory() + "\\Files";
        private static bool downloadComplete = true;
        private static bool parseComplete = true;
        private static bool stopWork = false;
        private static bool existCheck = true;


        public static int ProxyCount { get { return proxyCount; } set { proxyCount = value; NotifyStaticPropertyChanged(); } }
        public static List<Proxy> Proxies { get { return proxies; } set { proxies = value; ProxyCount = proxies.Count; NotifyStaticPropertyChanged(); } }
        public static int ParseThreadsCount { get { return parseThreadsCount; } }
        public static string ParseThreadsCountReadably { get { return parseThreadsCount.ToString(); } set { if (Int32.TryParse(value, out int num)) { if (num > threadMax) parseThreadsCount = threadMax; else if (num < threadMin) parseThreadsCount = threadMin; else parseThreadsCount = num; NotifyStaticPropertyChanged(); } } }
        public static string DownloadThreadsCountReadably { get { return downloadThreadsCount.ToString(); } set { if (Int32.TryParse(value, out int num)) { if (num > threadMax) downloadThreadsCount = threadMax; else if (num < threadMin) downloadThreadsCount = threadMin; else downloadThreadsCount = num; NotifyStaticPropertyChanged(); } } }
        public static int Deep { get => deep; }
        public static string DeepReadably { get { return deep.ToString(); } set { if (Int32.TryParse(value, out int num)) { if (num > deepMax) deep = deepMax; else if (num < deepMin) deep = deepMin; else deep = num; NotifyStaticPropertyChanged(); } } }
        public static int FileSize { get => fileSize * 1000000; }
        public static string FileSizeReadably { get { return fileSize.ToString(); } set { if (Int32.TryParse(value, out int num)) { if (num > maxSize) fileSize = maxSize; else if (num < Deep) fileSize = minSize; else fileSize = num; NotifyStaticPropertyChanged(); } } }
        public static string CurrentPath { get => currentPath; set => currentPath = value; }
        public static string ExtenstionReadable { get => extenstion; set { if (!string.IsNullOrEmpty(value)) extenstion = value; } }
        public static string[] Extenstion { get => extenstion.Split(';'); }
        public static int LinksCount { get { return linksCount; } set { linksCount = value; NotifyStaticPropertyChanged(); } }
        public static int ExistFilter { get { return existFilter; } set { existFilter = value; if (value != 0) ++FilterCount; NotifyStaticPropertyChanged(); } }
        public static int SizeFilter { get { return sizeFilter; } set { sizeFilter = value; if (value != 0) ++FilterCount; NotifyStaticPropertyChanged(); } }
        public static int ExtensionFilter { get { return extensionFilter; } set { extensionFilter = value; if (value != 0) ++FilterCount; NotifyStaticPropertyChanged(); } }
        public static int BadUrl { get => badUrl; set { badUrl = value; if (value != 0) ++FilterCount; NotifyStaticPropertyChanged(); } }
        public static int FilterCount { get => filterCount; private set { filterCount = value; NotifyStaticPropertyChanged(); } }
        public static bool DownloadComplete { get => downloadComplete; set => downloadComplete = value; }
        public static bool ParseComplete { get => parseComplete; set => parseComplete = value; }
        public static int ProgressCount { get { return progressCount; } set { progressCount = value; ProgressReadably = $"Working [{ProgressCount}/{SettingsLogic.Deep}]"; NotifyStaticPropertyChanged(); } }
        public static string ProgressReadably { get { return progressReadably; } private set { progressReadably = value; NotifyStaticPropertyChanged(); } }
        public static int ErrorCount { get { return errorCount; } set { errorCount = value; NotifyStaticPropertyChanged(); } }
        public static int DownloadThreadsCount { get => downloadThreadsCount; set => downloadThreadsCount = value; }
        public static string SearchWord { get => searchWord; set { if(!string.IsNullOrEmpty(value)) searchWord = value; NotifyStaticPropertyChanged(); } }
        public static bool StopWork { get => stopWork; }
        public static string CurrentVersion => currentVersion;
        public static string LoadPath { get => loadPath; set { if (!string.IsNullOrEmpty(value)) loadPath = value; NotifyStaticPropertyChanged(); } }
        public static bool ExistCheck { get => existCheck; set { existCheck = value; NotifyStaticPropertyChanged(); } }
        public static int DownloadAttemps { get => downloadAttemps; }
        public static string DownloadAttempsReadably { get { return downloadAttemps.ToString(); } set { if (Int32.TryParse(value, out int num)) { if (num > attempsMax) downloadAttemps = attempsMax; else if (num < threadMin) downloadAttemps = attempsMin; else downloadAttemps = num; NotifyStaticPropertyChanged(); } } }

        public static event PropertyChangedEventHandler StaticPropertyChanged;

        public static void RemoveProxy(Proxy proxy)
        {
            Proxies.Remove(proxy);
            --ProxyCount;
        }
        public static void Reload()
        {
            ExistFilter = 0;
            SizeFilter = 0;
            FilterCount = 0;
            ExtensionFilter = 0;
            ErrorCount = 0;
            ProgressCount = 0;
            LinksCount = 0;
            BadUrl = 0;
        }

        public static void StopWorking()
        {
            stopWork = true;
        }
        private static void NotifyStaticPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        public static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return (appSettings[key] ?? "");
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
            return null;
        }

        public static void UpdateSetting(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
}
