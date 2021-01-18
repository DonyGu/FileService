using Comm100.Framework.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace Comm100.Framework.Exceptions
{
    public class GlobalExceptions : IExceptionFilter
    {
        private readonly IHostingEnvironment _env;
        public GlobalExceptions(IHostingEnvironment env)
        {
            _env = env;
        }
        public void OnException(ExceptionContext context)
        {
            var json = new JsonErrorResponse();
            if (context.Exception.GetType() == typeof(FileKeyExistsException)
                || context.Exception.GetType() == typeof(FileNotAllowedException))
            {
                json.code = 400;
                json.message = context.Exception.Message;
                context.Result = new BadRequestObjectResult(json);
            }
            else if (context.Exception.GetType() == typeof(FileKeyNotFoundException))
            {
                json.code = 404;
                json.message = context.Exception.Message;
                context.Result = new NotFoundObjectResult(json);
            }
            else if (context.Exception.GetType() == typeof(UnauthorizedException))
            {
                json.code = 401;
                json.message = context.Exception.Message;
                context.Result = new UnauthorizedObjectResult(json);
            }
            else if (context.Exception.GetType() == typeof(FileTooLargeException))
            {
                json.code = 413;
                json.message = context.Exception.Message;
                context.Result = new PayloadTooLargeErrorObjectResult(json);
            }
            else
            {
                json.code = 500;
                json.message = context.Exception.Message;
                context.Result = new InternalServerErrorObjectResult(json);
            }

            //采用log4net 进行错误日志记录
            LogHelper.Error(context.Exception, json.message);

        }
    }
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object value) : base(value)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }

    }
    public class PayloadTooLargeErrorObjectResult : ObjectResult
    {
        public PayloadTooLargeErrorObjectResult(object value) : base(value)
        {
            StatusCode = StatusCodes.Status413PayloadTooLarge;
        }
    }
}
