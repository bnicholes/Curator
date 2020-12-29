using System;

#pragma warning disable 1591

namespace CuratorService.Exceptions
{
    public class CuratorServiceExceptionMessage
    {
        public string Message { get; }
        public string StackTrace { get; }

        public CuratorServiceExceptionMessage(Exception ex)
        {
            Message = ex.Message;
            StackTrace = ex.StackTrace;
        }
    }
}
