using HtmlAgilityPack;
using Leaf.xNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using static MoriAnonfilesChecker.ProgramLogic.ProxyParseLogic;
using ProxyType = MoriAnonfilesChecker.ProgramLogic.ProxyParseLogic.ProxyType;

namespace MoriAnonfilesChecker
{
    public class GetLogic
    {
        

        public struct FileInfo
        {
            public string Name;
            public long Size;
            public string SizeReadable;
            public string DownloadURL;
            public string Extension;
        }

        public static List<string> GoogleParse(int page, string Request, Proxy proxy)
        {
            page = page * 10;

            try
            {
                string url = "https://api.qwant.com/api/search/web?count=10&offset={page}&q=site%3Aanonfiles.com%20{request}&t=web&device=smartphone&extensionDisabled=true&safesearch=1&locale=en_US&uiv=4";
                //string jsonResponse = GetHtml(url, proxy);
                string jsonResponse = "{"status":true,"data":{"file":{"url":{"short":"https:\/\/anonfiles.com\/fdM0c369n4","full":"https:\/\/anonfiles.com\/fdM0c369n4\/SpotifyGOOD_txt"},"metadata":{"size":{"bytes":1006070,"readable":"982.49 KB"},"name":"SpotifyGOOD_txt","id":"fdM0c369n4"}}}}";

                return GetUrls(jsonResponse).Distinct().ToList();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        static List<string> GetUrls(string jsonResponse)
        {
            List<string> Urls = new List<string>();
            Regex regex = new Regex(@"^https:\/\/?.{1,8}anonfiles\.com\/([a-zA-Z0-9]+)?(\/)");


            dynamic DynamicData = JsonConvert.DeserializeObject(jsonResponse);
            foreach(dynamic item in DynamicData.data.result.items)
            {
                string url = (string)item.url;
                if (regex.IsMatch(url))
                {
                    Urls.Add(url);
                }
            }
            return Urls;
        }

        static string GetHtml(string url, Proxy proxy)
        {
            using var request = new HttpRequest() { UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36" };

            switch (proxy.proxyType)
            {
                case ProxyType.HTTPs:
                    request.Proxy = new HttpProxyClient(proxy.ip, proxy.port);
                    break;
                case ProxyType.Socks4:
                    request.Proxy = new Socks4ProxyClient(proxy.ip, proxy.port);
                    break;
                case ProxyType.Socks5:
                    request.Proxy = new Socks5ProxyClient(proxy.ip, proxy.port);
                    break;
            }


            return request.Get(url).ToString();
        }

        public static List<FileInfo> WorkWithUrls(List<string> requestMatches)
        {
            List<FileInfo> files = new List<FileInfo>(); 
            foreach (var uid in requestMatches)
            {
                try
                {
                    FileInfo file = GetInfo(uid);
                    var web = new HtmlWeb();
                    var doc = web.Load("https://anonfiles.com/" + uid);
                    var UrlNode = doc.DocumentNode.SelectNodes("//*[@id=\"download-url\"]");
                    file.DownloadURL = UrlNode[0].Attributes["href"].Value;
                    files.Add(file);
                }
                catch (WebException ex)
                {
                    Debug.WriteLine(ex.Message);
                    continue;
                }
            }
            return files;
        }

        private static FileInfo GetInfo(string uid)
        {
            FileInfo file = new FileInfo();
            var json = new WebClient().DownloadString($"https://api.anonfiles.com/v2/file/{uid}/info");
            dynamic DynamicData = JsonConvert.DeserializeObject(json);
            file.Name = DynamicData.data.file.metadata.name.ToString().Replace('_', '.');
            file.Extension = Path.GetExtension(file.Name);
            file.Size = (long)DynamicData.data.file.metadata.size.bytes;
            file.SizeReadable = DynamicData.data.file.metadata.size.readable;
            Console.WriteLine(file.Name + file.Size + "  " + file.SizeReadable);
            return file;
        }

        private static void DownloadFile(FileInfo file)
        {
            using (WebClient myWebClient = new WebClient())
            {
                Console.WriteLine(file.Size);
                myWebClient.DownloadFile(file.DownloadURL, @"d:\test\[" + DateTime.Now.ToString("dd-MM HH.mm.ss.f") + "]" + file.Name);
            }
        }
    }
}
