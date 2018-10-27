using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SDMCore
{
    public class HttpRequestClass
    {
        public HttpRequestClass()
        {
        }

        public static string GetRequest (string requestUrl) 
        {
            HttpClient request = new HttpClient();
            string response = null;
            response = request.GetStringAsync(requestUrl).Result;

            return response;
        }
    }
}
