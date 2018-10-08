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
    public class DownloadClass
    {
        public ContentData myContent = new ContentData();

        public struct ContentData {
            public string contentName;
            public byte[] contentData;
            public long contentSize;
            public string connectionStatus;
        }

        // конструктор класса
        //=============================================
        public DownloadClass()
        {}
        //=============================================

        // функция проверки доступности сайта/интренета
        //=============================================
        public static string CheckSiteAvaliable(string url)
        {
            HttpClient client1 = new HttpClient();
            string statusRequest = "";
            try {
                Task<HttpResponseMessage> task1 = client1.GetAsync(url);
                task1.Wait();
                statusRequest = task1.Result.StatusCode.ToString();
            }catch (Exception ex){
                return ex.Message;
            }
            return statusRequest;
        }
        //=============================================

        // функция загрузки с HTTP с указанием URL, PATH(by default=the same dir where run this ultility), RESUME(by default=false), THREAD(by default=1), ADAPTER(by default=not set)
        //=============================================
        public ContentData DownloadHttp(string url, bool resume = false, int thread = 1, string adapter = "", bool inetAlreadyChecked = false, string inetConnectionStatus = "")
        {
            string isAvaliableInet = "";
            string isAvaliableSite = "";

            isAvaliableInet = (inetAlreadyChecked) ? inetConnectionStatus : CheckSiteAvaliable("http://www.ya.ru");
            if (isAvaliableInet == "OK") {
                Uri uri = new Uri(url);
                isAvaliableSite = CheckSiteAvaliable(((uri.Host.Contains("http://") || (uri.Host.Contains("https://")))? uri.Host : uri.Scheme + "://" + uri.Host));
            } else {
                myContent.contentName = null;
                myContent.contentData = null;
                myContent.contentSize = 0;
                myContent.connectionStatus = isAvaliableInet;

                return myContent;
            }

            if (isAvaliableSite == "OK")
            {
                WebClient webClient = new WebClient();


                /*HttpClient client = new HttpClient();
                Task<HttpResponseMessage> task = client.GetAsync(url);
                task.Wait();

                myContent.contentName = task.Result.Content.Headers.ContentDisposition.FileName;
                myContent.contentData = task.Result.Content.ReadAsByteArrayAsync().Result;
                myContent.contentSize = task.Result.Content.Headers.ContentLength.Value;
                myContent.connectionStatus = isAvaliableSite;*/
            } else {
                myContent.contentName = null;
                myContent.contentData = null;
                myContent.contentSize = 0;
                myContent.connectionStatus = isAvaliableSite;
            }

            return myContent;
        }
        //=============================================

        // функция загрузки с HTTPS с указанием URL, PATH(by default=the same dir where run this ultility), RESUME(by default=false), THREAD(by default=1), ADAPTER(by default=not set)
        //=============================================
        public string DownloadHttps(string url, string path = null, bool resume = false, int thread = 1, string adapter = "")
        {


            return "https_result";
        }
        //=============================================

        // функция загрузки с FTP с указанием URL, PATH(by default=the same dir where run this ultility), RESUME(by default=false), THREAD(by default=1), ADAPTER(by default=not set)
        //=============================================
        public string DownloadFtp()
        {
            

            return "ftp_result";
        }
        //=============================================
    }
}
