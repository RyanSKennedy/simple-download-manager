using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Reflection;

namespace SDMCore
{
    public class DownloadProgressChangedArgs
    {
        protected string filename;
        public string FileName
        {
            get { return filename; }
        }

        protected uint progress_percentage;
        public uint ProgressPercentage
        {
            get { return progress_percentage; }
        }

        protected uint bytes_read;
        public uint BytesRead
        {
            get { return bytes_read; }
        }

        public DownloadProgressChangedArgs(String Filename, uint Progress_percentage, uint Bytes_read)
        {
            this.filename = Filename;
            this.progress_percentage = Progress_percentage;
            this.bytes_read = Bytes_read;
        }
    }
    public class DownloadFileCompletedArgs
    {
        protected string filename;
        public string FileName
        {
            get { return filename; }
        }

        protected long filesize;
        public long FileSize
        {
            get { return filesize; }
        }

        public DownloadFileCompletedArgs(String Filename, long Filesize)
        {
            this.filename = Filename;
            this.filesize = Filesize;
        }
    }
    public class DownloadFileErrorArgs
    {
        protected string filename;
        protected Exception e;
        public string FileName
        {
            get { return filename; }
        }
        public Exception Error
        {
            get { return e; }
        }

        public DownloadFileErrorArgs(String Filename, Exception error)
        {
            this.filename = Filename;
            this.e = error;
        }
    }

    class StandartFileDownloaderClass
    {
        public delegate void DownloadProgressChangedHandler(object sender, DownloadProgressChangedArgs e);
        public delegate void DownloadFileCompletedHandler(object sender, DownloadFileCompletedArgs e);
        public delegate void DownloadFileErrorHandler(object sender, DownloadFileErrorArgs e);
        public event DownloadProgressChangedHandler ProgressChanged;
        public event DownloadFileCompletedHandler FileCompleted;
        public event DownloadFileErrorHandler FileError;

        private const long fragment_size = 500 * 1024;

        private string url;
        private string filename;
        private long filesize;
        private long length;
        private long position;
        private long totalBytesRead;

        public long TotalBytesRead { get { return totalBytesRead; } }
        public long Length { get { return length; } }
        public long Position
        {
            get
            {
                return position;
            }
            set
            {
                if (value < 0) throw new ArgumentException();
                position = value;
            }
        }
        public StandartFileDownloaderClass(string URL, string Filename)
        {
            url = URL;
            filename = Filename;
        }

        public static long GetContentSize (string url) 
        {
            long size = 0;

            HttpClient client = new HttpClient();
            try {
                var task = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                task.Wait();
                size = (long)task.Result.Content.Headers.ContentLength;
            } catch (Exception ex) {
                return 0;
            }

            return size;
        }

        public static string GetContentName (string url) 
        {
            string contentNameStr = null;

            HttpClient client = new HttpClient();
            try
            {
                var task = client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                task.Wait();
                contentNameStr = task.Result.RequestMessage.RequestUri.Segments[task.Result.RequestMessage.RequestUri.Segments.Length - 1].ToString();
            }
            catch (Exception ex)
            {
                return null;
            }

            return contentNameStr;
        }

        public void StartDownload(HttpClient httpClient = null)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            if (httpClient != null) {
                // загрузка через HttpClient
                // в теории пока
                return;
            }

            using (HttpWebResponse result = (HttpWebResponse)request.GetResponse())
            {
                length = result.ContentLength;
                filesize = result.ContentLength;
                result.Close();
            }

            using (FileStream fs = new FileStream(filename, FileMode.Append))
            {
                totalBytesRead = fs.Length;
                Position = fs.Length;

                byte[] buffer = new byte[fragment_size];

                while (TotalBytesRead < Length)
                {
                    try
                    {
                        int receivedLengh = 0;
                        if (TotalBytesRead + fragment_size < Length)
                            receivedLengh = (int)fragment_size;
                        else
                            receivedLengh = (int)(Length - TotalBytesRead);

                        int count = Read(buffer, receivedLengh);

                        OnProgressChanged((uint)count);
                        fs.Write(buffer, 0, count); // сохранение потока бит в файл
                    }
                    catch (Exception e)
                    {
                        OnFileError(e);
                    }
                }

                fs.Close();
            }

            OnDownloadComplete();
        }

        private int Read(byte[] buffer, int count)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            // Gyper hack, because microsoft is stupid
            object[] args = new object[3];
            Type t = typeof(HttpWebRequest);
            args[0] = "bytes";
            args[1] = position.ToString();
            args[2] = (position + count).ToString();

            MethodInfo[] mi = t.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            int MethodI;
            for (MethodI = 0; MethodI < mi.Length; MethodI++)
                if (mi[MethodI].Name == "AddRange")
                    break;

            mi[MethodI].Invoke(request, args);
            request.KeepAlive = true;

            HttpWebResponse result = (HttpWebResponse)request.GetResponse();

            int allreadbytes = 0;
            int readbytes = 0;
            int temp_pos = 0;

            using (Stream stream = result.GetResponseStream())
            {
                while ((readbytes = stream.Read(buffer, temp_pos, count)) > 0)
                {
                    allreadbytes += readbytes;
                    temp_pos += readbytes;
                    count -= readbytes;
                }
                stream.Close();
            }

            result.Close();
            totalBytesRead += allreadbytes;
            Position += allreadbytes;
            return allreadbytes;
        }

        protected void OnProgressChanged(uint bytes_read)
        {
            if (ProgressChanged != null)
            {
                uint percents = (uint)((float)totalBytesRead / (float)Length * 100.0);
                ProgressChanged(this, new DownloadProgressChangedArgs(filename, percents, bytes_read));
            }
        }

        protected void OnDownloadComplete()
        {
            if (FileCompleted != null)
            {
                FileCompleted(this, new DownloadFileCompletedArgs(filename, filesize));
            }
        }

        protected void OnFileError(Exception e)
        {
            if (FileError != null)
            {
                FileError(this, new DownloadFileErrorArgs(filename, e));
            }
        }
    }
}