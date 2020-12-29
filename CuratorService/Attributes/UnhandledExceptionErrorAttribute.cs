using CuratorService.Exceptions;
using CuratorService.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
#pragma warning disable 1591

namespace CuratorService.Attributes
{
    /// <summary>
    /// Convert unhandled exceptions to a web response
    /// </summary>
    public class UnhandledExceptionErrorAttribute : ExceptionFilterAttribute
    {
        private readonly Serilog.ILogger _logger = Serilog.Log.Logger;

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception.FlattenException() is CuratorServiceException curatorServiceException)
            {
                context.Result = new JsonResult(new
                {
                    curatorServiceException.Message,
                    curatorServiceException.StackTrace,
                });

                var errorMessage = curatorServiceException.Message;
                var message = $"Executed action: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path} = {context.Exception?.GetType().FullName}: {(int)curatorServiceException.Status} {curatorServiceException.Status.ToString()}\r\n{errorMessage}";
                _logger.Error(message);

                context.Result = new CuratorServiceExceptionResult(context.Exception, curatorServiceException.Status);
            }
            else
            {
                context.Result = new CuratorServiceExceptionResult(context.Exception, HttpStatusCode.InternalServerError);
            }

            context.ExceptionHandled = true;
        }
    }
}
