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
        public static DataForDownloadingFromGoogleDrive myGoogleDrive = new DataForDownloadingFromGoogleDrive();
        private static int prev_ticks = 0;
        private static Dictionary<string, string> dData = new Dictionary<string, string>();


        public struct ContentData {
            public string contentName;
            public string contentFullName;
            public string contentPath;
            public string contentExtension;
            public long contentSize;
            public string connectionStatus;
        }

        public struct DataForDownloadingFromGoogleDrive {
            public string url;
            public HttpClient httpClient;
            //public Dictionary<string, string> cookies;
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
                myContent.contentExtension = null;
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
                    // загрузка с yadi.sk пример: -url:"https://yadi.sk/d/9kwNhX7T3aEwB7"
                    //=============================================
                    JObject tmpResult = null;
                    string getDataFromUrl = YaDiskClass.GetInfo(dData["url"]);
                    tmpResult = (!String.IsNullOrEmpty(getDataFromUrl) ? (JObject)JsonConvert.DeserializeObject(getDataFromUrl) : null);

                    if (String.IsNullOrEmpty(dData["full_name"])) {
                        if (tmpResult == null) {
                            string tmpAutoName = SDMCore.InfoClass.GetNewFileName();

                            dData["full_name"] = tmpAutoName;
                            dData["name"] = tmpAutoName;
                            dData["extension"] = "";
                        } else {
                            dData["full_name"] = tmpResult["name"].ToString();
                            dData["name"] = (tmpResult["name"].ToString().Contains(".") ? tmpResult["name"].ToString().Substring(0, tmpResult["name"].ToString().LastIndexOf('.')) : tmpResult["name"].ToString());
                            dData["extension"] = (tmpResult["name"].ToString().Contains(".") ? tmpResult["name"].ToString().Substring(tmpResult["name"].ToString().LastIndexOf('.') + 1, tmpResult["name"].ToString().Length - 1 - tmpResult["name"].ToString().LastIndexOf('.')) : "");
                        }
                    }

                    myContent.contentName = dData["full_name"];
                    myContent.contentFullName = dData["folder"] + Path.DirectorySeparatorChar + dData["full_name"];
                    myContent.contentPath = dData["folder"];
                    myContent.contentExtension = dData["extension"];
                    myContent.contentSize = (tmpResult == null ? 0 : (long)Convert.ToInt32(tmpResult["size"].ToString()));

                    var urlForDownload = YaDiskClass.GetUrlForDownloadingData(dData["url"]);

                    StandartFileDownloaderClass iw = new StandartFileDownloaderClass(urlForDownload, myContent.contentFullName);
                    iw.ProgressChanged += iw_ProgressChanged;
                    iw.FileCompleted += iw_FileCompleted;
                    iw.StartDownload();
                    //=============================================
                }
                else if (dData["url"].Contains("drive.google.com")) {
                    // загрузка с drive.google.com пример: -url:"https://drive.google.com/open?id=16m1ptk2N9iV4nzCW9FMjZArdxe8J3KG6"
                    //=============================================
                    var urlForDownload = GoogleDriveClass.GetUrlForDownloadingData(dData["url"]);

                    // получаем имя файла 
                    var tmpResponse = GoogleDriveClass.GetInfo(urlForDownload.url, urlForDownload.httpClient);
                    string tmpFileName = tmpResponse.Content.Headers.ContentDisposition.FileNameStar;
                    //

                    if (String.IsNullOrEmpty(dData["full_name"]))
                    {
                        if (tmpFileName == null)
                        {
                            string tmpAutoName = SDMCore.InfoClass.GetNewFileName();

                            dData["full_name"] = tmpAutoName;
                            dData["name"] = tmpAutoName;
                            dData["extension"] = "";
                        }
                        else
                        {
                            dData["full_name"] = tmpFileName;
                            dData["name"] = (tmpFileName.Contains(".") ? tmpFileName.Substring(0, tmpFileName.LastIndexOf('.')) : tmpFileName);
                            dData["extension"] = (tmpFileName.Contains(".") ? tmpFileName.Substring(tmpFileName.LastIndexOf('.') + 1, tmpFileName.Length - 1 - tmpFileName.LastIndexOf('.')) : "");
                        }
                    }

                    myContent.contentName = dData["full_name"];
                    myContent.contentFullName = dData["folder"] + Path.DirectorySeparatorChar + dData["full_name"];
                    myContent.contentPath = dData["folder"];
                    myContent.contentExtension = dData["extension"];
                    myContent.contentSize = 0;

                    StandartFileDownloaderClass iw = new StandartFileDownloaderClass(urlForDownload.url, myContent.contentFullName);
                    iw.ProgressChanged += iw_ProgressChanged;
                    iw.FileCompleted += iw_FileCompleted;
                    iw.StartDownload(urlForDownload.httpClient);
                    //=============================================
                }
                else {
                    // загрузка с обычного http пример: -url:"http://safenet-sentinel.ru/files/sentinel_ldk_run-time_gui.zip"
                    //=============================================
                    if (String.IsNullOrEmpty(dData["full_name"])) {
                        string tmpNameFromUrl = StandartFileDownloaderClass.GetContentName(url);
                        if (String.IsNullOrEmpty(tmpNameFromUrl)) {
                            string tmpAutoName = SDMCore.InfoClass.GetNewFileName();

                            dData["full_name"] = tmpAutoName;
                            dData["name"] = tmpAutoName;
                            dData["extension"] = "";
                        } else {
                            dData["full_name"] = tmpNameFromUrl;
                            dData["name"] = (tmpNameFromUrl.Contains(".") ? tmpNameFromUrl.Substring(0, tmpNameFromUrl.LastIndexOf('.')) : tmpNameFromUrl);
                            dData["extension"] = (tmpNameFromUrl.Contains(".") ? tmpNameFromUrl.Substring(tmpNameFromUrl.LastIndexOf('.') + 1, tmpNameFromUrl.Length - 1 - tmpNameFromUrl.LastIndexOf('.')) : "");
                        }
                    }

                    myContent.contentName = dData["full_name"];
                    myContent.contentFullName = dData["folder"] + Path.DirectorySeparatorChar + dData["full_name"];
                    myContent.contentPath = dData["folder"];
                    myContent.contentExtension = dData["extension"];
                    myContent.contentSize = StandartFileDownloaderClass.GetContentSize(url);

                    StandartFileDownloaderClass iw = new StandartFileDownloaderClass(dData["url"], myContent.contentFullName);
                    iw.ProgressChanged += iw_ProgressChanged;
                    iw.FileCompleted += iw_FileCompleted;
                    iw.StartDownload();
                    //=============================================
                }
            }
            else {
                myContent.contentName = null;
                myContent.contentFullName = null;
                myContent.contentPath = null;
                myContent.contentExtension = null;
                myContent.contentSize = 0;
                myContent.connectionStatus = isAvaliableSite;
            }

            return myContent;
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
