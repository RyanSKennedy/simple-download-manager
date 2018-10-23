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

        public static HttpResponseMessage GetRequest (string requestUrl) 
        {
            HttpClient request = new HttpClient();
            HttpResponseMessage response = null;
            response = request.GetAsync(requestUrl).Result;

            return response;
        }
    }
}
