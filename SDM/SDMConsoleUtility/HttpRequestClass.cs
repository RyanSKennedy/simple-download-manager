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
            string response = null;

            HttpClient request = new HttpClient();
            response = request.GetStringAsync(requestUrl).Result;
                        
            return response;
        }

        public static HttpResponseMessage GetRequest(string requestUrl, HttpClient httpClient = null)
        {
            var response = httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);
            response.Wait();

            return response.Result;
        }
    }
}
