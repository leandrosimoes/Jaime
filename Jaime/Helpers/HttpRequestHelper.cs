using System;
using System.Net;
using Jaime.Models;

namespace Jaime.Helpers {
    public class HttpRequestHelper {
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        public static HttpWebResponse CreateGetHttpResponse(string url, HttpProxyModel proxy, out string formatedUrl) {
            try {
                if (string.IsNullOrEmpty(url)) {
                    throw new Exception("Url inválida");
                }

                if (!url.Contains("http://") && !url.Contains("https://")) {
                    url = "http://" + url;
                    formatedUrl = url;
                } else {
                    formatedUrl = url;
                }

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                if (proxy != null && proxy.Enabled && !string.IsNullOrEmpty(proxy.Server)) {
                    if (string.IsNullOrEmpty(proxy.UserName) || string.IsNullOrEmpty(proxy.Password)) {
                        request.Proxy = new WebProxy(proxy.Server, proxy.Port);
                    }
                    else {
                        request.Proxy = new WebProxy(proxy.Server, proxy.Port);
                        request.Proxy.Credentials = new NetworkCredential(proxy.UserName, proxy.Password);
                    }
                }
                request.Method = "GET";
                request.UserAgent = DefaultUserAgent;
                request.Timeout = 10*1000;

                return request.GetResponse() as HttpWebResponse;
            } catch (TimeoutException ex) {
                throw new Exception("A Url requisitada não responde, verifique se é uma url válida ou se suas configurações de proxy estão corretas.");
            }
        }
    }
}