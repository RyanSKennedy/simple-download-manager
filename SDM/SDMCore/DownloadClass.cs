using System;
using System.Reflection;

namespace SDMCore
{
    public class DownloadClass
    {
        public DownloadClass()
        {
            
        }

        public static string SayHi()
        {
            return "Hi Men!";
        }

        public static string ReturnHelp()
        {
            string help = "This is Help for SDMCore DLL." + Environment.NewLine +
                          "You can use next arguments:" + Environment.NewLine +
                          "\"-h\" or \"-help\" - for display this Help information." + Environment.NewLine +
                          "\"-v\" or \"-version\" - for display version of SDMUtility" + Environment.NewLine +
                          "\"-u\" or \"-url\" - for set download link." + Environment.NewLine +
                          "\"-l\" or \"-login\" - for set login for FTP connection." + Environment.NewLine +
                          "\"-p\" or \"-password\" - for set password for FTP connection." + Environment.NewLine +
                          "\"-f\" or \"-folder\" - for set folder where should save content (by default using folder where located SDMUtility)." + Environment.NewLine +
                          "\"-r\" or \"-resume\" - for set on/off resuming downloading proccess if needed (on=1, off=0, by Default=0)." + Environment.NewLine +
                          "\"-t\" or \"-thread\" - for set number of thread for downloading (min=1, max=5, by Default=1)." + Environment.NewLine +
                          "\"-a\" or \"-adapter\" - for set which network adapter should using for downloading." + Environment.NewLine + 
                          Environment.NewLine + "Example of calling:" + Environment.NewLine +
                          "...\\SDMConsoleUtilityMac.dll -u:http:\\\\testServer:8080\\1.mp4 -f:\"C:\\tmp\\\" -r:1 -t:3" + Environment.NewLine;
            

            return help;
        }

        public string GetHttp() 
        {


            return "http_result";
        }

        public string GetHttps()
        {


            return "https_result";
        }

        public string GetFtp()
        {


            return "ftp_result";
        }

        public static string GetVersion()
        {
            return typeof(DownloadClass).GetTypeInfo().Assembly.GetName().Version.ToString();
        }
    }
}
