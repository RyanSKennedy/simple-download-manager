using System;
using SDMCore;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace SDMConsoleUtility
{
    class Program
    {
        public static Arguments[] argsMass;
        public static string baseDir = "";
        public static Dictionary<string, string> argsArray = new Dictionary<string, string>();
        public static Dictionary<string, string> dData = new Dictionary<string, string>();
        public SDMCore.DownloadClass.ContentData myContent = new SDMCore.DownloadClass.ContentData();


        public struct Arguments
        {
            public string arg;
            public string value;
        }

        static void Main(string[] args)
        {
            // получение пути до базовой директории где расположено приложение
            //============================================= 
            System.Reflection.Assembly a = System.Reflection.Assembly.GetEntryAssembly();
            baseDir = System.IO.Path.GetDirectoryName(a.Location);
            //=============================================

            SDMCore.DownloadClass myClass; 

            argsMass = new Arguments[args.Length];
            int i = 0;

            // проверка массива переданных приложению аргументов на наличие команд для вывода версии или справки, а также на корректность вводимых аргументов
            //=============================================
            foreach (string el in args)
            {
                if (String.IsNullOrEmpty(el))
                {
                    break;
                }

                if (el.StartsWith('-') && el.Contains(":"))
                {
                    string[] tmpArg = el.Split(':', 2);
                    argsMass[i].arg = tmpArg[0].Substring(1);
                    argsMass[i].value = tmpArg[1];
                    i++;
                }
                else if (el.StartsWith('-') && (el.Substring(1, el.Length - 1).StartsWith('h') || el.Contains("help")))
                {
                    Console.WriteLine(SDMCore.InfoClass.GetHelp());
                    return;
                }
                else if (el.StartsWith('-') && (el.Substring(1, el.Length - 1).StartsWith('v') || el.Contains("version")))
                {
                    Console.WriteLine("Version SDMCore.dll: " + SDMCore.InfoClass.GetVersion());
                    Console.WriteLine("Version SDMConsoleUtility: " + typeof(Program).GetTypeInfo().Assembly.GetName().Version.ToString());
                    return;
                }
                else if (el.StartsWith('-') && (el.Substring(1, el.Length - 1).StartsWith('r') || el.Contains("resume")))
                {
                    argsMass[i].arg = el.Substring(1);
                    argsMass[i].value = el;
                    i++;
                }
                else
                {
                    Console.WriteLine("You using incorrect arguments: " + el + Environment.NewLine);
                    return;
                }
            }
            //=============================================

            // проверка массива переданных приложению аргументов и их значений на валидность и заполнение коллекцию ключей и значений
            //=============================================
            foreach (Arguments el in argsMass)
            {
                switch (el.arg)
                {
                    case "u":
                    case "url":
                        argsArray.Add("url", el.value);
                        dData.Add("url", el.value);
                        break;

                    case "l":
                    case "login":
                        argsArray.Add("login", el.value);
                        break;

                    case "p":
                    case "password":
                        argsArray.Add("password", el.value);
                        break;

                    case "f":
                    case "folder":
                        argsArray.Add("folder", el.value);
                        dData.Add("folder", el.value);
                        break;

                    case "n":
                    case "name":
                        argsArray.Add("name", el.value);
                        dData.Add("full_name", el.value);
                        if (el.value.Split('.').Length > 1) {
                            dData.Add("name", el.value.Substring(0, el.value.LastIndexOf('.')));
                            dData.Add("extension", el.value.Substring(el.value.LastIndexOf('.') + 1, el.value.Length - 1 - el.value.LastIndexOf('.')));
                        } else {
                            dData.Add("name", el.value);
                            dData.Add("extension", "");
                        }
                        break;

                    case "r":
                    case "resume":
                        argsArray.Add("resume", "true");
                        break;

                    case "t":
                    case "thread":
                        if (0 < Convert.ToInt32(el.value) && Convert.ToInt32(el.value) < 6)
                        {
                            argsArray.Add("thread", el.value);
                        }
                        else
                        {
                            argsArray.Add("thread", "1");
                        }
                        break;

                    case "a":
                    case "adapter":
                        argsArray.Add("adapter", el.value);
                        break;

                    default:
                        Console.WriteLine("You using incorrect arguments: " + el + Environment.NewLine);
                        return;
                }
            }
            //=============================================

            // проверка переданных приложению аргументов и их значений на наличие обязательного аргумента - URL, иначе нечего скачивать
            //=============================================
            bool urlIsExist = false;
            bool fileNameIsExist = false;
            bool filePathIsExist = false;
            foreach (KeyValuePair<string, string> el in argsArray)
            {
                if (el.Key == "url" && !String.IsNullOrEmpty(el.Value))
                {
                    urlIsExist = true;
                } else if(el.Key == "name" && !String.IsNullOrEmpty(el.Value)) {
                    fileNameIsExist = true;
                } else if (el.Key == "folder" && !String.IsNullOrEmpty(el.Value)) {
                    filePathIsExist = true;
                }
            }
            //=============================================

            // задаём имя по умолчанию (если через параметры ничего не передали)
            //=============================================
            if (fileNameIsExist == false) {
                dData.Add("full_name", "");
                dData.Add("name", "");
                dData.Add("extension", "");
            }
            //=============================================

            // задаём путь до сохраняемого файла по умолчанию (если через параметры ничего не передали)
            //=============================================
            if (filePathIsExist == false) {
                argsArray.Add("folder", baseDir + Path.DirectorySeparatorChar + "download");
                dData.Add("folder", baseDir + Path.DirectorySeparatorChar + "download");
            }
            //=============================================

            myClass = new SDMCore.DownloadClass(dData);

            // выполняем загрузку
            //=============================================
            if (urlIsExist == true)
            {
                // do something
                string tmpCheck = SDMCore.DownloadClass.CheckSiteAvaliable("http://www.ya.ru");
                if (tmpCheck != "OK")
                {
                    Console.WriteLine("Problem with internet connection. Error: " + Environment.NewLine + tmpCheck + Environment.NewLine);
                    return;
                }

                SDMCore.DownloadClass.ContentData myContent = myClass.DownloadHttp(dData["url"], false, 1, "", true, tmpCheck);
            }
            else
            {
                Console.WriteLine("You not add url, nothing to download!" + Environment.NewLine);
                return;
            }
            //=============================================
        }
    }
}
