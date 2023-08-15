using System.Security.Cryptography.X509Certificates;

namespace MoriPattern.Model
{
    public enum ProxyType
    {
        Socks4,
        Socks5,
        HTTPs
    }

    public class Proxy
    {
        public Proxy() { }

        public Proxy(string ip, int port, ProxyType proxyType)
        {
            Ip = ip;
            Port = port;
            ProxyType = proxyType;
            Try = 0;
        }
        public int Try { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public ProxyType ProxyType { get; set; }

        public static ProxyType GetProxyType(int proxyType)
        {
            return proxyType switch
            {
                0 => ProxyType.HTTPs,
                1 => ProxyType.Socks4,
                2 => ProxyType.Socks5,
                _ => ProxyType.Socks4,
            };
        }

        public static int GetProxyId(ProxyType proxyType)
        {
            return proxyType switch
            {
                ProxyType.HTTPs => 0,
                ProxyType.Socks4 => 1,
                ProxyType.Socks5 => 2,
                _ => 1
            };
        }
    }
}
