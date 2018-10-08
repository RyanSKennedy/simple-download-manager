using System;
using SDMCore;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SDMConsoleUtilityUniverse
{
    class Program
    {
        public static Arguments[] argsMass;
        public static string baseDir = "";
        public static Dictionary<string, string> argsArray = new Dictionary<string, string>();
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

            SDMCore.DownloadClass myClass = new SDMCore.DownloadClass();

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
            foreach (KeyValuePair<string, string> el in argsArray)
            {
                if (el.Key == "url" && !String.IsNullOrEmpty(el.Value))
                {
                    urlIsExist = true;
                }
            }
            //=============================================

            myClass = new SDMCore.DownloadClass();

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

                myClass.myContent = myClass.DownloadHttp(argsArray["url"], false, 1, "", true, tmpCheck);

                if (myClass.myContent.contentData != null && myClass.myContent.connectionStatus == "OK")
                {
                    Console.WriteLine("Connection status: " + myClass.myContent.connectionStatus + Environment.NewLine);
                    Console.WriteLine("File name: " + myClass.myContent.contentName + Environment.NewLine);
                    Console.WriteLine("File size: " + myClass.myContent.contentSize + " bytes" + Environment.NewLine);
                    Console.WriteLine("File data: " + myClass.myContent.contentData + Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Problem with internet connection. Error: " + Environment.NewLine + myClass.myContent.connectionStatus + Environment.NewLine);
                    return;
                }
            }
            else
            {
                Console.WriteLine("You not add url, nothing to download!" + Environment.NewLine);
                return;
            }
            //=============================================

            /*Console.WriteLine("//===========================");
            Console.WriteLine(SDMCore.InfoClass.SayHi());
            Console.WriteLine("//===========================");*/
        }
    }
}
