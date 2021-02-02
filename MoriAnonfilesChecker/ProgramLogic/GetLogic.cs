using HtmlAgilityPack;
using Leaf.xNet;
using MoriAnonfilesChecker.Object_Class;
using MoriAnonfilesChecker.ProgramLogic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Markup;
using static MoriAnonfilesChecker.ProgramLogic.ProxyParseLogic;
using ProxyType = MoriAnonfilesChecker.ProgramLogic.ProxyParseLogic.ProxyType;

namespace MoriAnonfilesChecker
{
    public class GetLogic
    {
        public enum Countries
        {
            [Display(Name = "Andorra")] AD,
            [Display(Name = "Argentina")] AR,
            [Display(Name = "Australia")] AU,
            [Display(Name = "Austria")] AT,
            [Display(Name = "Belgium")] BE,
            [Display(Name = "Bulgaria")] BG,
            [Display(Name = "Burkina Faso")] BF,
            [Display(Name = "Burundi")] BI,
            [Display(Name = "Canada")] CA,
            [Display(Name = "Chile")] CL,
            [Display(Name = "China")] CN,
            [Display(Name = "Czechia")] CZ,
            [Display(Name = "Denmark")] DK,
            [Display(Name = "Estonia")] EE,
            [Display(Name = "Finland")] FI,
            [Display(Name = "France")] FR,
            [Display(Name = "Germany")] DE,
            [Display(Name = "Greece")] GR,
            [Display(Name = "Hong Kong")] HK,
            [Display(Name = "Hungary")] HU,
            [Display(Name = "India")] IN,
            [Display(Name = "Ireland")] IE,
            [Display(Name = "Malaysia")] MY,
            [Display(Name = "Mexico")] MX,
            [Display(Name = "Netherlands")] NL,
            [Display(Name = "New Zealand")] NZ,
            [Display(Name = "Poland")] PL,
            [Display(Name = "Portugal")] PT,
            [Display(Name = "Romania")] RO,
            [Display(Name = "Spain")] ES,
            [Display(Name = "Sweden")] SE,
            [Display(Name = "Switzerland")] CH,
            [Display(Name = "Thailand")] TH,
            [Display(Name = "United States")] US
        }


        public static string country;

        public static List<string> GoogleParse(int page, string Request, Proxy proxy)
        {
            page = page * 10;

            try
            {
                string url = $"https://api.qwant.com/api/search/web?count=10&offset={page}&q=site%3Aanonfiles.com%20{Request}&t=web&r={country}&device=smartphone&extensionDisabled=true&safesearch=1&locale=en_US&uiv=4";
                string jsonResponse = GetHtml(url, proxy);
                Debug.WriteLine(jsonResponse);
                return GetUrls(jsonResponse).Distinct().ToList();
            }
            catch (Exception)
            {
                throw new RankException();
            }
        }

        static List<string> GetUrls(string jsonResponse)
        {
            List<string> Urls = new List<string>();
            Regex regex = new Regex(@"^https:\/\/?.{1,8}anonfiles\.com\/([a-zA-Z0-9]+)?(\/)");


            dynamic DynamicData = JsonConvert.DeserializeObject(jsonResponse);
            foreach (dynamic item in DynamicData.data.result.items)
            {
                string url = (string)item.url;
                if (regex.IsMatch(url))
                {
                    var match = regex.Match(url);
                    Urls.Add(match.Groups[1].ToString());
                }
            }
            return Urls;
        }

        static string GetHtml(string url, Proxy proxy)
        {
            using var request = new HttpRequest() { 
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36", 
                ConnectTimeout = 1000 };

            switch (proxy.proxyType)
            {
                case ProxyType.HTTPs:
                    request.Proxy = new HttpProxyClient(proxy.ip, proxy.port);
                    break;
                default:
                case ProxyType.Socks4:
                    request.Proxy = new Socks4ProxyClient(proxy.ip, proxy.port);
                    break;
                case ProxyType.Socks5:
                    request.Proxy = new Socks5ProxyClient(proxy.ip, proxy.port);
                    break;
            }


            return request.Get(url).ToString();
        }

        public static MoriFile WorkWithUrls(string uid)
        {
            try
            {
                MoriFile file = GetInfo(uid);
                var web = new HtmlWeb();
                var doc = web.Load("https://anonfiles.com/" + uid);
                var UrlNode = doc.DocumentNode.SelectNodes("//*[@id=\"download-url\"]");
                file.DownloadURL = UrlNode[0].Attributes["href"].Value;
                file.DownloadURL = file.DownloadURL;

                return file;
            }
            catch (Exception)
            {
                return new MoriFile();
            }
        }

        private static MoriFile GetInfo(string uid)
        {
            MoriFile file = new MoriFile();
            string url = $"https://api.anonfiles.com/v2/file/{uid}/info";
            var json = new WebClient().DownloadString(url);
            dynamic DynamicData = JsonConvert.DeserializeObject(json);
            file.Name = DynamicData.data.file.metadata.name.ToString().Replace('_', '.');
            file.Extension = Path.GetExtension(file.Name);
            file.Size = (long)DynamicData.data.file.metadata.size.bytes;
            file.DownloadRetry = 0;
            file.SizeReadable = DynamicData.data.file.metadata.size.readable;
            file.Status = "Pending..";
            file.Uid = uid;
            file.Url = "https://anonfiles.com/" + uid;;
            return file;
        }

        
    }
}
