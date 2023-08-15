using MoriPattern.Controller;
using MoriPattern.Data;
using MoriPattern.Model;
using System;
using System.Text.RegularExpressions;

namespace MoriPattern.Res
{
    public static partial class GeneralMethods
    {

        public static void LoadSettings()
        {
            GlobalData.Instance.FileProxyPath = Global.ReadSetting<string>("FileProxyPath");
            GlobalData.Instance.Urls = Global.ReadSetting<string>("Urls");
            GlobalData.Instance.FileUrlsPath = Global.ReadSetting<string>("FileUrlsPath");
            GlobalData.Instance.ProxyType = Proxy.GetProxyType(Global.ReadSetting("ProxyType", 1));
            GlobalData.Instance.Timeout = Global.ReadSetting("Timeout", 5);
            GlobalData.Instance.ThreadsCount = Global.ReadSetting("Threads", 50);
            GlobalData.Instance.BadRecheck = Global.ReadSetting("BadRecheck", 0);
            GlobalData.Instance.ProxyRetries = Global.ReadSetting("ProxyRetries", 0);
            GlobalData.Instance.IsFirstTime = Global.ReadSetting("IsFirstTime", true);
            GlobalData.Instance.UseProxy = Global.ReadSetting("UseProxy", true);
        }

        public static void SaveSettings()
        {
            Global.SetSetting("FileProxyPath", GlobalData.Instance.FileProxyPath);
            Global.SetSetting("Urls", GlobalData.Instance.Urls);
            Global.SetSetting("FileUrlsPath", GlobalData.Instance.FileUrlsPath);
            Global.SetSetting("ProxyType", Proxy.GetProxyId(GlobalData.Instance.ProxyType));
            Global.SetSetting("Timeout", GlobalData.Instance.Timeout);
            Global.SetSetting("Threads", GlobalData.Instance.ThreadsCount);
            Global.SetSetting("BadRecheck", GlobalData.Instance.BadRecheck);
            Global.SetSetting("ProxyRetries", GlobalData.Instance.ProxyRetries);
            Global.SetSetting("IsFirstTime", GlobalData.Instance.IsFirstTime);
            Global.SetSetting("UseProxy", GlobalData.Instance.UseProxy);
        }


        public static string[] SplitUrls(string Urls)
        {
            return Urls.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitProxies(string proxies)
        {
            return proxies.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
