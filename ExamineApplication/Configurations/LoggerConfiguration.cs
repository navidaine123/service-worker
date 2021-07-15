using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamineApplication.Configurations
{
    public class LoggerConfiguration
    {
        private readonly IConfigurationRoot _configurationRoot;
        private ILogger _logger;

        public LoggerConfiguration(IConfigurationRoot configurationRoot,out ILogger logger)
        {
            _configurationRoot = configurationRoot;
            InitialSerilogLogger();
            logger = _logger;
        }
        private void InitialSerilogLogger()
        {
            Log.Logger = new Serilog.LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.RollingFile(new JsonFormatter(),
                _configurationRoot["SeriLog:FilePath"],
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .WriteTo.Seq(_configurationRoot["SeriLog:SeqHost"])
                .CreateLogger();
            _logger = Log.Logger;
        }
    }
}
