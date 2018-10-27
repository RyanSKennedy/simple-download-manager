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
    public class DownloadClass
    {
        public static ContentData myContent = new ContentData();
        private static int prev_ticks = 0;
        private static Dictionary<string, string> dData = new Dictionary<string, string>();

        public struct ContentData {
            public string contentName;
            public string contentFullName;
            public string contentPath;
            public string contentExtention;
            public long contentSize;
            public string connectionStatus;
        }

        // конструктор класса
        //=============================================
        public DownloadClass(Dictionary<string, string> data)
        {
            dData = data;
        }
        //=============================================

        // функция проверки доступности сайта/интренета
        //=============================================
        public static string CheckSiteAvaliable(string url)
        {
            HttpClient client1 = new HttpClient();
            string statusRequest = "";
            try {
                Task<HttpResponseMessage> task1 = client1.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
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
                myContent.contentFullName = null;
                myContent.contentPath = null;
                myContent.contentExtention = null;
                myContent.contentSize = 0;
                myContent.connectionStatus = isAvaliableInet;

                return myContent;
            }

            if (isAvaliableSite == "OK")
            {
                myContent.connectionStatus = isAvaliableSite;

                // создаём директорию если она не существует
                //=============================================
                if (!Directory.Exists(dData["folder"]))
                {
                    Directory.CreateDirectory(dData["folder"]);
                }
                //=============================================

                if (dData["url"].Contains("yadi.sk")) {
                    // загрузка с обычного http пример: -url:"https://yadi.sk/d/9kwNhX7T3aEwB7"
                    //=============================================
                    JObject tmpResult = (JObject)JsonConvert.DeserializeObject(YaDiskClass.GetInfo(dData["url"]));

                    myContent.contentName = tmpResult["name"].ToString();
                    myContent.contentFullName = dData["folder"] + Path.DirectorySeparatorChar + tmpResult["name"].ToString();
                    myContent.contentPath = dData["folder"];
                    myContent.contentExtention = (tmpResult["name"].ToString().Contains(".") ? tmpResult["name"].ToString().Split(".")[tmpResult["name"].ToString().Split(".").Length - 1] : "");
                    myContent.contentSize = (long)Convert.ToInt32(tmpResult["size"].ToString());

                    var urlForDownload = YaDiskClass.GetData(dData["url"]);
                    StandartFileDownloaderClass iw = new StandartFileDownloaderClass(urlForDownload, myContent.contentFullName);
                    iw.ProgressChanged += iw_ProgressChanged;
                    iw.FileCompleted += iw_FileCompleted;
                    iw.StartDownload();
                    //=============================================
                }
                else if (dData["url"].Contains("drive.google.com")) {

                } else {
                    // загрузка с обычного http пример: -url:"http://safenet-sentinel.ru/files/sentinel_ldk_run-time_gui.zip"
                    //=============================================
                    StandartFileDownloaderClass iw = new StandartFileDownloaderClass(dData["url"], dData["folder"] + Path.DirectorySeparatorChar);
                    iw.ProgressChanged += iw_ProgressChanged;
                    iw.FileCompleted += iw_FileCompleted;
                    iw.StartDownload(true);
                    //=============================================
                }
            }
            else {
                myContent.contentName = null;
                myContent.contentFullName = null;
                myContent.contentPath = null;
                myContent.contentExtention = null;
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

        // событие по окончании загрузки
        //=============================================
        static void iw_FileCompleted(object sender, DownloadFileCompletedArgs e)
        {
            if (myContent.contentFullName == null)
            {
                myContent.contentFullName = e.FileName;
                myContent.contentName = e.FileName.Split(Path.DirectorySeparatorChar.ToString())[e.FileName.Split(Path.DirectorySeparatorChar.ToString()).Length - 1]; //dData["name"] + (String.IsNullOrEmpty(dData["extension"]) ? "" : "." + dData["extension"]);
                myContent.contentPath = dData["folder"];
                myContent.contentExtention = myContent.contentName.Split(".")[myContent.contentName.Split(".").Length - 1];
                myContent.contentSize = e.FileSize;
            }

            Console.WriteLine(Environment.NewLine + "Completed downloading and saving file: \"" + myContent.contentName + "\"\r\n");
        }
        //=============================================

        // событие при изменении состояния загрузки 
        //=============================================
        static void iw_ProgressChanged(object sender, DownloadProgressChangedArgs e)
        {
            int currect_ticks = Environment.TickCount;
            if (prev_ticks == 0)
                prev_ticks = currect_ticks;

            int diff = currect_ticks - prev_ticks;
            int speed = 0;
            if (diff != 0)
            {
                speed = (int)((float)e.BytesRead / (float)diff * 1000.0 / 1024);
            }

            prev_ticks = currect_ticks;

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("{0}%... Speed: {1} KBytes/sec", e.ProgressPercentage, speed);
        }
        //=============================================
    }
}
