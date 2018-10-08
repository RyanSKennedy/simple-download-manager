using System;
using System.IO;

namespace SDMCore
{
    public class SaveClass
    {
        public static string baseDir = "";

        // конструктор класса
        //=============================================
        public SaveClass()
        {
            // получение пути до базовой директории где расположено приложение
            //============================================= 
            baseDir = SDMCore.InfoClass.GetBaseDir();
            //=============================================
        }
        //=============================================

        // функция сохранения данных в файл 
        //=============================================
        public static bool SaveTo(string fileName, string filePath, byte[] fileData)
        {

            return true;
        }
        //=============================================
    }
}
