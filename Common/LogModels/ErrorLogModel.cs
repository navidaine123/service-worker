using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.LogModels
{
    public class ErrorLogModel
    {
        public ErrorLogModel(Exception ex)
        {
            Data = JsonConvert.SerializeObject(ex.Data);
            ExceptionType = ex.GetType().Name;
            InnerException = ex.InnerException != null ?ex.InnerException.Message:"";
            Message = ex.Message;
            StackTrace = ex.StackTrace;
        }
        public string Data { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string ExceptionType { get; set; }
        public string InnerException { get; set; }

    }
}
