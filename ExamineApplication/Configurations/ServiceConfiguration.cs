using Data;
using EasyCaching.Core.Configurations;
using ExamineApplication.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamineApplication.Configurations
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection UseAppJson(this IServiceCollection services,out IConfigurationRoot configurationRoot)
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

            configurationRoot = builder.Build();
            return services;
        }
        public static IServiceCollection UseSerilog(this IServiceCollection services,IConfigurationRoot configurationRoot,out ILogger logger)
        {
            var seriLogConf = new LoggerConfiguration(configurationRoot,out logger);
            return services;
        }
        public static IServiceCollection UseRedis(this IServiceCollection services,IConfigurationRoot configurationRoot)
        {
            
            services.AddEasyCaching(options =>
            {
                options.UseRedis(redisConfig =>
                {
                    redisConfig.DBConfig.Endpoints.Add(new ServerEndPoint(configurationRoot["Redis:host"],Convert.ToInt32(configurationRoot["Redis:port"])));
                    redisConfig.DBConfig.AllowAdmin = true;
                }, "navid");
            });
            return services;
        }

        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfigurationRoot configurationRoot,ILogger logger)
        {
            var context = new Context(configurationRoot["ConnectionStrings:Sql_Server"],logger);
            context.InitialDataBase().GetAwaiter();
            return services;
        }


    }
}
