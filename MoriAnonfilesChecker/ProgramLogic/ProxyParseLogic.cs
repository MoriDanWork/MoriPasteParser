using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MoriAnonfilesChecker.ProgramLogic
{
    public class ProxyParseLogic
    {
        string path;
        string[] urls;
        int mode;
        Regex regex = new Regex(@"\d{1,3}(\.\d{1,3}){3}:\d{1,5}");
        List<Proxy> proxies = null;
        ProxyType proxyType;

        public enum ProxyType
        {
            HTTPs,
            Socks4,
            Socks5
        }

        public enum CheckMode
        {
            [Display(Name = "Imap")]
            Imap,
            [Display(Name = "TCP")]
            TCP,
            [Display(Name = "Web (Get)")]
            Web
        }

        public struct Proxy
        {
            public string ip;
            public int port;
            public ProxyType proxyType;

            public Proxy(string Ip, int Port, ProxyType proxyType)
            {
                this.ip = Ip;
                this.port = Port;
                this.proxyType = proxyType;
            }
        }

        public ProxyParseLogic(string path, ProxyType proxyType)
        {
            this.path = path;
            mode = 1;
            this.proxyType = proxyType;
            StartParse();
        }

        public ProxyParseLogic(string[] urls, ProxyType proxyType)
        {
            this.urls = urls;
            mode = 2;
            this.proxyType = proxyType;
            StartParse();
        }

        public ProxyParseLogic(string path, string[] urls, ProxyType proxyType)
        {
            this.path = path;
            this.urls = urls;
            mode = 3;
            this.proxyType = proxyType;
            StartParse();
        }

        void StartParse()
        {
            proxies = new List<Proxy>();
            switch (mode)
            {
                case 1: // file
                    proxies.AddRange(ParseFromFile(proxyType));
                    break;
                case 2: // urls
                    proxies.AddRange(ParseFromUrls(proxyType));
                    break;
                case 3: // files and urls
                    proxies.AddRange(ParseFromFile(proxyType));
                    proxies.AddRange(ParseFromUrls(proxyType));
                    break;
            }

            if (proxies.Count != 0)
                proxies = proxies.Distinct().ToList();
        }

        private List<Proxy> ParseFromFile(ProxyType proxyType)
        {
            var proxies = new List<Proxy>();
            using (var sr = new StreamReader(this.path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (regex.IsMatch(line))
                    {
                        string[] splittedProxy = Regex.Split(regex.Match(line).ToString(), @"\:|\;");
                        proxies.Add(new Proxy(splittedProxy[0], Convert.ToInt32(splittedProxy[1]), proxyType));
                    }
                }
            }
            return proxies;
        }
        private List<Proxy> ParseFromUrls(ProxyType proxyType)
        {
            var proxies = new List<Proxy>();
            using (WebClient client = new WebClient())
            {
                int count = 0;
                while (urls.Length > count)
                {
                    string htmlCode = client.DownloadString(this.urls[count]);
                    regex.Matches(htmlCode).ToList().ForEach((x) =>
                    {
                        string[] splittedProxy = Regex.Split(x.ToString(), @"\:|\;");

                        proxies.Add(new Proxy(splittedProxy[0], Convert.ToInt32(splittedProxy[1]), proxyType));

                    });
                    ++count;
                }
            }
            return proxies;
        }

        public List<Proxy> GetProxies()
        {
            return proxies;
        }
    }
}
