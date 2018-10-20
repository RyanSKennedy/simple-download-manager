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
            if (!File.Exists(filePath + Path.DirectorySeparatorChar + fileName)) File.Create(filePath + Path.DirectorySeparatorChar + fileName);
            {
                StreamWriter output = new StreamWriter(filePath + Path.DirectorySeparatorChar + fileName);
                try
                {
                    output.Write(fileData);
                    output.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        //=============================================
    }
}
