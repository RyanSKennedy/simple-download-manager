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
    public class GoogleDriveClass
    {
        private static string getInfoPreLink = "";
        private static string getDataPreLink = "";

        public GoogleDriveClass()
        {
        }

        public static string GetInfo(string url) 
        {
            return HttpRequestClass.GetRequest(getInfoPreLink + ConvertUrl(url));
        }

        public static string GetData(string url)
        {


            return null;
        }

        private static string ConvertUrl(string url)
        {
            url = url.Replace(":", "%3A");
            url = url.Replace("/", "%2F");

            return url;
        }
    }
}
