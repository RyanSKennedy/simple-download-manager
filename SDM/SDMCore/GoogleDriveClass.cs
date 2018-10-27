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

namespace SDMCore
{
    public class GoogleDriveClass
    {
        public GoogleDriveClass()
        {
        }

        public static string GetInfo() 
        {
            var response = HttpRequestClass.GetRequest("");

            return response;
        }
    }
}
