using System;
using System.Net;

#pragma warning disable 1591

namespace CuratorService.Exceptions
{
    public class CuratorServiceException : Exception
    {
        public HttpStatusCode Status;

        public CuratorServiceException(string message, HttpStatusCode status = HttpStatusCode.BadRequest) : base(message)
        {
            Status = status;
        }

        public CuratorServiceException(string message, Exception ex, HttpStatusCode status = HttpStatusCode.BadRequest) : base(message, ex)
        {
            Status = status;
        }
    }
}
