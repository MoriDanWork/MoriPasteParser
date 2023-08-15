using MoriPattern.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MoriPattern.Controller
{
    public enum ParseMode
    {
        Url,
        Clipboard,
        File,
        UrlClipboard,
        UrlFile,
        ClipboardFile,
        Everything,
        None
    }

    public static partial class ProxyController
    {
        private static string path;
        private static string[] urls;
        private static string urlsPath;
        private static string[] clipboard;
        private static string filePath;
        private static readonly Regex regexProxy = RegexProxy();
        private static readonly Regex regexUrl = RegexUrl();
        private static List<Proxy> proxies = new();

        private static ProxyType proxyType;
        private static ParseMode parseMode;

        public static async Task ParseProxyAsync(ParseMode _parseMode, ProxyType _proxyType, string _filePath = null, string[] _urls = null, string _urlsPath = null, string[] _clipboard = null)
        {
            path = _filePath;
            proxyType = _proxyType;
            parseMode = _parseMode;
            filePath = _filePath;
            urls = _urls ?? Array.Empty<string>();
            urlsPath = _urlsPath;
            clipboard = _clipboard ?? Array.Empty<string>();
            await StartParseAsync();
        }

        private static async Task StartParseAsync()
        {
            proxies.Clear();

            List<Task> tasks = new();

            switch (parseMode)
            {
                case ParseMode.Url:
                    tasks.Add(ParseFromUrlsAsync());
                    break;
                case ParseMode.Clipboard:
                    tasks.Add(ParseFromClipBoardAsync());
                    break;
                case ParseMode.File:
                    tasks.Add(ParseFromFileAsync());
                    break;
                case ParseMode.UrlClipboard:
                    tasks.Add(ParseFromUrlsAsync());
                    tasks.Add(ParseFromClipBoardAsync());
                    break;
                case ParseMode.UrlFile:
                    tasks.Add(ParseFromUrlsAsync());
                    tasks.Add(ParseFromFileAsync());
                    break;
                case ParseMode.Everything:
                    tasks.Add(ParseFromUrlsAsync());
                    tasks.Add(ParseFromClipBoardAsync());
                    tasks.Add(ParseFromFileAsync());
                    break;
            }

            await Task.WhenAll(tasks);
        }

        private static async Task ParseFromClipBoardAsync()
        {
            List<Proxy> localProxies = new();

            foreach (var line in clipboard)
            {
                if (regexProxy.IsMatch(line))
                {
                    string[] splittedProxy = Regex.Split(regexProxy.Match(line).ToString(), @"\:|\;");
                    localProxies.Add(new Proxy(splittedProxy[0], Convert.ToInt32(splittedProxy[1]), proxyType));
                }
            }

            await AddProxiesAsync(localProxies);
        }

        private static async Task ParseFromFileAsync()
        {
            List<Proxy> localProxies = new();

            using (var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (regexProxy.IsMatch(line))
                    {
                        string[] splittedProxy = Regex.Split(regexProxy.Match(line).ToString(), @"\:|\;");
                        localProxies.Add(new Proxy(splittedProxy[0], Convert.ToInt32(splittedProxy[1]), proxyType));
                    }
                }
            }

            await AddProxiesAsync(localProxies);
        }

        private static async Task ParseFromUrlsAsync()
        {
            List<Proxy> localProxies = new();

            List<string> urlsList = new();
            urlsList.AddRange(urls);

            if (!string.IsNullOrEmpty(urlsPath))
                urlsList.AddRange(GetUrlFromFile());

            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36");

            int count = 0;
            while (urlsList.Count > count)
            {
                if (regexUrl.IsMatch(urlsList[count]))
                {
                    try
                    {
                        string htmlCode = await client.GetStringAsync(urlsList[count]);
                        foreach (Match match in regexProxy.Matches(htmlCode).Cast<Match>())
                        {
                            string[] splittedProxy = regexSplittedProxy().Split(match.ToString());
                            localProxies.Add(new Proxy(splittedProxy[0], Convert.ToInt32(splittedProxy[1]), proxyType));
                        }
                    }
                    catch { }
                }

                ++count;
            }

            await AddProxiesAsync(localProxies);
        }

        private static async Task AddProxiesAsync(List<Proxy> localProxies)
        {
            await Task.Run(() =>
            {
                lock (proxies)
                {
                    proxies.AddRange(localProxies);
                }
            });
        }

        private static string[] GetUrlFromFile()
        {
            List<string> urlsList = new();

            using (var sr = new StreamReader(urlsPath))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    urlsList.Add(line);
                }
            }

            return urlsList.ToArray();
        }

        public static List<Proxy> GetProxies()
        {
            return proxies.Shuffle().Distinct().ToList();
        }

        public static ParseMode ChooseProxyParse(bool urls, bool files, bool clipboard)
        {
            if (urls && files && clipboard)
            {
                return ParseMode.Everything;
            }
            else if (files && clipboard)
            {
                return ParseMode.ClipboardFile;
            }
            else if (urls && files)
            {
                return ParseMode.UrlFile;
            }
            else if (urls)
            {
                return ParseMode.Url;
            }
            else if (files)
            {
                return ParseMode.File;
            }
            else if (clipboard)
            {
                return ParseMode.Clipboard;
            }

            return ParseMode.None;
        }

        [GeneratedRegex("\\:|\\;")]
        private static partial Regex regexSplittedProxy();
        [GeneratedRegex("\\d{1,3}(\\.\\d{1,3}){3}:\\d{1,5}")]
        private static partial Regex RegexProxy();
        [GeneratedRegex("(http|ftp|https):\\/\\/([\\w_-]+(?:(?:\\.[\\w_-]+)+))([\\w.,@?^=%&:\\/~+#-]*[\\w@?^=%&\\/~+#-])")]
        private static partial Regex RegexUrl();
    }
}
