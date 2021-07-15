using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Common.LogModels
{
    public class LogModel
    {
        public LogModel(object logdata)
        {
            MachineName = Environment.MachineName;
            ProccessId = Process.GetCurrentProcess().Id;
            Data = logdata;
        }
        public string MachineName { get; set; }
        public int ProccessId { get; set; }
        public object Data { get; set; }
    }
}
