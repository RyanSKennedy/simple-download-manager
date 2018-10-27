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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SDMCore
{
    public class YaDiskClass
    {
        private static string getInfoPreLink = "https://cloud-api.yandex.net:443/v1/disk/public/resources?public_key=";
        private static string getDataPreLink = "https://cloud-api.yandex.net:443/v1/disk/public/resources/download?public_key=";

        public YaDiskClass()
        {
        }

        public static string GetInfo(string url)
        {
            return HttpRequestClass.GetRequest(getInfoPreLink + ConvertUrl(url));
        }

        public static string GetData(string url) 
        {
            JObject tmpResult = (JObject)JsonConvert.DeserializeObject(HttpRequestClass.GetRequest(getDataPreLink + ConvertUrl(url)));
            string urlForDownload = null;
            if (tmpResult.ContainsKey("href")) {
                urlForDownload = tmpResult["href"].ToString();
            }

            return urlForDownload;
        }

        private static string ConvertUrl (string url) 
        {
            url = url.Replace(":", "%3A");
            url = url.Replace("/", "%2F");

            return url;
        }
    }
}
