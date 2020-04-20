using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using System.Web.Http.Results;

namespace Comm100.Framework.Exceptions
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            HandleExceptionAsync(context);
            context.ExceptionHandled = true;
        }

        private static void HandleExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;

            if (exception is FileKeyNotFoundException)
                SetExceptionResult(context, exception, HttpStatusCode.NotFound);
            else if (exception is UnauthorizedException)
                SetExceptionResult(context, exception, HttpStatusCode.Unauthorized);
            else if (exception is FileKeyExistsException)
                SetExceptionResult(context, exception, HttpStatusCode.BadRequest);
            else
                SetExceptionResult(context, exception, HttpStatusCode.InternalServerError);
        }

        private static void SetExceptionResult(
            ExceptionContext context,
            Exception exception,
            HttpStatusCode statusCode)
        {
            var response = new ApiResponse() { code = (int)statusCode, message = exception.Message };
            context.Result = new InternalServerErrorObjectResult(response)
            {
                StatusCode = (int)statusCode
            };
        }
    }
    public class ApiResponse
    {
        public int code { get; set; }
        public string message { get; set; }
    }

}
