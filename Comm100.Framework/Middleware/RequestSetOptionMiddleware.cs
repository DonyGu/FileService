using Comm100.Framework.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comm100.Framework.Middleware
{
    public class RequestSetOptionsMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestSetOptionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            LogHelper.WriteLog($"methord{httpContext.Request.Method}");
            foreach (var item in httpContext.Request.Headers)
            {
                LogHelper.WriteLog(item.Key.ToString()+":"+item.Value.ToString());
            }
            if (httpContext.Request.Method!="GET" && httpContext.Request.Headers.TryGetValue("Origin", out StringValues origin))
            {
                LogHelper.WriteLog($"origin{origin}");
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", origin.ToString());
                httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            }
            await _next(httpContext);
        }
    }
}
