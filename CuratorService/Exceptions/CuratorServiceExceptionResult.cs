using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

#pragma warning disable 1591

namespace CuratorService.Exceptions
{
    public class CuratorServiceExceptionResult : JsonResult
    {
        public CuratorServiceExceptionResult(Exception ex, HttpStatusCode status) : base(new CuratorServiceExceptionMessage(ex))
        {
            StatusCode = (int)status;
        }
    }
}
