using System;
using System.Reflection;

namespace SDMCore
{
    public class InfoClass
    {
        public InfoClass()
        {
        }

        // тестовая функция, здаровается с мужчинами =)
        //=============================================
        public static string SayHi()
        {
            return "Hi Men!";
        }
        //=============================================

        // функция получения базовой директории приложения
        //=============================================
        public static string GetBaseDir()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }
        //=============================================

        // функция получения версии компонентов
        //=============================================
        public static string GetVersion()
        {
            return typeof(InfoClass).GetTypeInfo().Assembly.GetName().Version.ToString();
        }
        //=============================================

        // функция возвращает справку по возможностям программы
        //=============================================
        public static string GetHelp()
        {
            return "This is Help for SDM Console Utility." + Environment.NewLine +
                          "You can use next arguments:" + Environment.NewLine +
                          "\"-h\" or \"-help\" - for display this Help information." + Environment.NewLine +
                          "\"-v\" or \"-version\" - for display version of SDMConsoleUtility" + Environment.NewLine +
                          "\"-r\" or \"-resume\" - for switch ON resuming downloading proccess." + Environment.NewLine +
                          "\"-u\" or \"-url\" - for set download link." + Environment.NewLine +
                          "\"-l\" or \"-login\" - for set login for FTP connection." + Environment.NewLine +
                          "\"-p\" or \"-password\" - for set password for FTP connection." + Environment.NewLine +
                          "\"-f\" or \"-folder\" - for set folder where should save content (by default using folder where located SDMConsoleUtility)." + Environment.NewLine +
                          "\"-n\" or \"-name\" - for set full name with or without extension(by default name is: \"file_dd_MM_yyyy-HH_mm_ss\")." + Environment.NewLine +
                          "\"-t\" or \"-thread\" - for set number of thread for downloading (min=1, max=5, by Default=1)." + Environment.NewLine +
                          "\"-a\" or \"-adapter\" - for set which network adapter should using for downloading." + Environment.NewLine +
                          Environment.NewLine + "Example of calling:" + Environment.NewLine + "...\\SDMConsoleUtility.dll -u:\"http:\\\\testServer:8080\\1.mp4\" -f:\"C:\\tmp\\\" -n:\"test.txt\" -r:1 -t:3" + Environment.NewLine;
        }
        //=============================================

        // функция получения имени файла из фиксированной части "file_" + текущей даты и времени
        //=============================================
        public static string GetNewFileName()
        {
            return "file_" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss");
        }
        //=============================================

        // перегрузка функции получения имени файла из ПРЕФИКСА + текущей даты и времени + РАСШИРЕНИЯ
        //=============================================
        public static string GetNewFileName(string fileName = "", string extention = "")
        {
            return (fileName != "" ? fileName : "file_") + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss") + (extention != "" ? "." + extention : "");
        }
        //=============================================
    }
}
