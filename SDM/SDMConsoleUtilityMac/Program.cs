using System;
using System.Reflection;
using System.Collections.Generic;

namespace SDMConsoleUtilityMac
{
    class Program
    {
        public static Arguments[] argsMass;
        public static string baseDir = "";
        public static Dictionary<string, string> argsArray = new Dictionary<string, string>();


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
            foreach(string el in args) 
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
                } else if (el.StartsWith('-') && (el.Substring(1,el.Length-1).StartsWith('h') || el.Contains("help"))) {
                    Console.WriteLine(SDMCore.DownloadClass.ReturnHelp());
                    return;
                } else if (el.StartsWith('-') && (el.Substring(1, el.Length - 1).StartsWith('v') || el.Contains("version"))){
                    Console.WriteLine("Version SDMCore.dll: " + SDMCore.DownloadClass.GetVersion());
                    Console.WriteLine("Version SDMConsoleUtility: " + typeof(Program).GetTypeInfo().Assembly.GetName().Version.ToString());
                    return;
                } else {
                    Console.WriteLine("You using incorrect arguments: " + el + Environment.NewLine + "//===========================" + Environment.NewLine);
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
                        argsArray.Add(el.arg,el.value);
                        break;

                    case "l":
                    case "login":
                        argsArray.Add(el.arg, el.value);
                        break;

                    case "p":
                    case "password":
                        argsArray.Add(el.arg, el.value);
                        break;

                    case "f":
                    case "folder":
                        argsArray.Add(el.arg, el.value);
                        break;

                    case "r":
                    case "resume":
                        if (el.value != "0" && el.value != "1") 
                        {
                            argsArray.Add(el.arg, "0");
                        } else {
                            argsArray.Add(el.arg, el.value);
                        } 
                        break;

                    case "t":
                    case "thread":
                        if (0 < Convert.ToInt32(el.value) && Convert.ToInt32(el.value) < 6)
                        {
                            argsArray.Add(el.arg, el.value);
                        }
                        else
                        {
                            argsArray.Add(el.arg, "1");
                        }
                        break;

                    case "a":
                    case "adapter":
                        argsArray.Add(el.arg, el.value);
                        break;

                    default:
                        Console.WriteLine("You using incorrect arguments: " + el + Environment.NewLine + "//===========================" + Environment.NewLine);
                        return;
                }
            }
            //=============================================

            Console.WriteLine("//===========================");
            Console.WriteLine(SDMCore.DownloadClass.SayHi());
        }
    }
}
