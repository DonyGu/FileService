using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Comm100.Framework.Common
{
    public class CallApiHelper
    {
        private static readonly int CallApiTimeOutSeconds = 60;
        public static T CallApi<T>(out HttpStatusCode statusCode , string url, HttpMethod httpMethod, string token = "", string paraJson = "")
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var req = new HttpRequestMessage(httpMethod, url);
                    if (httpMethod == HttpMethod.Put || httpMethod == HttpMethod.Post)
                    {
                        HttpContent content = new StringContent(paraJson);
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        req.Content = content;
                    }
                    client.Timeout = TimeSpan.FromSeconds(60);
                    if (!string.IsNullOrEmpty(token))
                    {
                        req.Headers.Add("Authorization", $"Bearer {token}");
                    }
                    var res = client.SendAsync(req).Result;
                    var o = res.Content.ReadAsStringAsync().Result;
                    statusCode = res.StatusCode;
                    if (res.IsSuccessStatusCode)
                    {
                        T result = JsonConvert.DeserializeObject<T>(o);
                        return result;
                    }
                    else
                    {
                        throw new Exception(res.ReasonPhrase);
                    }
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }
}
