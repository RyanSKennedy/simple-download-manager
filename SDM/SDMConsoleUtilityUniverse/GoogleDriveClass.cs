using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Net;

namespace SDMCore
{
    // Web клиент, используемый для загрузки с Google Drive (так как в ряде случаев, когда скачивается 
    // большой файл, гугл требует подтверждения от пользователя, код которого он пихает в куки)
    public class CookieAwareWebClient : WebClient
    {
        private class CookieContainer
        {
            Dictionary<string, string> _cookies;

            public string this[Uri url]
            {
                get
                {
                    string cookie;
                    if (_cookies.TryGetValue(url.Host, out cookie))
                        return cookie;

                    return null;
                }
                set
                {
                    _cookies[url.Host] = value;
                }
            }

            public CookieContainer()
            {
                _cookies = new Dictionary<string, string>();
            }
        }

        private CookieContainer cookies;

        public CookieAwareWebClient() : base()
        {
            cookies = new CookieContainer();
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

            if (request is HttpWebRequest)
            {
                string cookie = cookies[address];
                if (cookie != null)
                    ((HttpWebRequest)request).Headers.Set("cookie", cookie);
            }

            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);

            string[] cookies = response.Headers.GetValues("Set-Cookie");
            if (cookies != null && cookies.Length > 0)
            {
                string cookie = "";
                foreach (string c in cookies)
                    cookie += c;

                this.cookies[response.ResponseUri] = cookie;
            }

            return response;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);

            string[] cookies = response.Headers.GetValues("Set-Cookie");
            if (cookies != null && cookies.Length > 0)
            {
                string cookie = "";
                foreach (string c in cookies)
                    cookie += c;

                this.cookies[response.ResponseUri] = cookie;
            }

            return response;
        }
    }

    public class GoogleDriveClass
    {
        private static string fileIdFromUrl = null;
        private static string getInfoPreLink = null;
        private static string getDataPreLink = null;
        private const string domainPartFromUrl = "https://drive.google.com/";


        public GoogleDriveClass(string url = null)
        {
            if (url != null) fileIdFromUrl = GetFileIdFromUrl(url);
        }

        public static string GetInfo(string url) 
        {


            return null;
        }

        public static DownloadClass.DataForDownloadingFromGoogleDrive GetUrlForDownloadingData(string url)
        {
            DownloadClass.DataForDownloadingFromGoogleDrive urlForDownload;
            urlForDownload.url = null;
            urlForDownload.cookies = null;

            Uri uri = new Uri(url);
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();

            handler.CookieContainer.Add(uri, new Cookie("cookie", "My_test_cookies!")); // добавляем куку
            HttpClient httpClient = new HttpClient(handler);
            var task = httpClient.GetAsync(uri);
            task.Wait();
            CookieCollection collection = handler.CookieContainer.GetCookies(uri); // читаем куку из ответа

            //string tmpStr = "Stop" + " working!"; // строка для точки останова

            if (task.Result.RequestMessage.RequestUri.AbsoluteUri.Contains("/view")) {
                urlForDownload = GetUrlForDownloadingData(UrlBuilder(GetFileIdFromUrl(url), null, false, true));
            } else {
                foreach (Cookie c in collection)
                {
                    if (c.Name.StartsWith("download_warning") && c.Name.Contains(GetFileIdFromUrl(url)) && !String.IsNullOrEmpty(c.Value))
                    {
                        urlForDownload.url = UrlBuilder(GetFileIdFromUrl(url), c.Value, true);
                        urlForDownload.cookies = new Dictionary<string, string>();
                        urlForDownload.cookies.Add("Name", c.Name);
                        urlForDownload.cookies.Add("Value", c.Value);
                        urlForDownload.cookies.Add("Domain", c.Domain);
                        urlForDownload.cookies.Add("Path", c.Path);
                    }
                }

                if (urlForDownload.url == null)
                {
                    urlForDownload = GetUrlForDownloadingData(url);
                }
            }

            return urlForDownload;
        }

        public static string GetFileIdFromUrl(string url)
        {
            // варианты ссылок:
            // https://drive.google.com/open?id=1UfgaDHxc29ygvYucZ1vjX2Tk6GdwTzra
            // https://drive.google.com/file/d/1UfgaDHxc29ygvYucZ1vjX2Tk6GdwTzra/view
            // https://drive.google.com/uc?id=1UfgaDHxc29ygvYucZ1vjX2Tk6GdwTzra&export=download
            // https://drive.google.com/uc?export=download&confirm=3YOp&id=1UfgaDHxc29ygvYucZ1vjX2Tk6GdwTzra
            // https://drive.google.com/uc?authuser=0&id=1WO_DuoQAnUQEBV_KbYtnAeH738rWFhvH&export=download

            string fileId = null;
            if (url.Contains("id=")) {
                string subStr = url.Split("id=")[1];
                fileId = (subStr.Contains("&") ? subStr.Split("&")[0] : subStr);
            } else if (url.Contains("file/d/")) {
                string subStr = url.Split("file/d/")[1];
                fileId = (subStr.Contains("/") ? subStr.Split("/")[0] : subStr);
            }

            return fileId;
        }

        private static string ConvertUrl(string url)
        {
            url = url.Replace(":", "%3A");
            url = url.Replace("/", "%2F");

            return url;
        }

        private static string UrlBuilder(string fileId, string confirmId, bool virusScanChecker = false, bool viewParam = false)
        {
            if (virusScanChecker == false && viewParam == false) {
                return (domainPartFromUrl + "uc?authuser=0&id=" + fileId + "&export=download");
            } else if (virusScanChecker != false && viewParam == false) { 
                return (domainPartFromUrl + "uc?export=download&confirm=" + confirmId + "&id=" + fileId);
            } else {
                return (domainPartFromUrl + "uc?id=" + fileId + "&export=download");
            } 
        }
    }
}