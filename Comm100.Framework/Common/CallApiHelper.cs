using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Comm100.Framework.Common
{
    public class CallApiHelper
    {
        private static readonly int CallApiTimeOutSeconds = 60;
        public static T CallApi<T>(out HttpStatusCode statusCode, string url, HttpMethod httpMethod, string token = "", string paraJson = "")
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
        public async static Task<HttpStatusCode> UploadFile(string url, string token, string fileName, byte[] content, DTO.FileSyncDTO fileSyncDTO)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                MultipartFormDataContent multipartFormData = new MultipartFormDataContent($"----WebKitFormBoundary{DateTime.Now.Ticks.ToString("x")}");
                multipartFormData.Add(new ByteArrayContent(content), "file", fileName);

                multipartFormData.Add(new StringContent(fileSyncDTO.siteId.ToString()), "siteId");
                multipartFormData.Add(new StringContent(fileSyncDTO.creationTime.ToString()), "creationTime");
                multipartFormData.Add(new StringContent(fileSyncDTO.expireTime.ToString()), "expireTime");


                client.Timeout = TimeSpan.FromSeconds(90);
                var res = await client.PostAsync(new Uri(url), multipartFormData);
                var o = await res.Content.ReadAsStringAsync();
                if (res.IsSuccessStatusCode)
                {
                    return res.StatusCode;
                }
                else if (res.StatusCode==HttpStatusCode.BadRequest && o== "{\"code\":400,\"message\":\"filekey already exisits.\"}")
                {
                    LogHelper.Error($"code : result : {o}, posturl: {url}");
                    return HttpStatusCode.OK;
                }
                else
                {
                    LogHelper.Error($"code : {res.IsSuccessStatusCode}, result : {o}, posturl: {url}");
                    throw new Exception(res.ReasonPhrase);
                }
            }
        }
    }
}
