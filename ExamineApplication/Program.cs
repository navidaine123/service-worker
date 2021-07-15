using Data;
using Data.Models;
using EasyCaching.Core.Configurations;
using ExamineApplication.Configurations;
using ExamineApplication.RabbitMqServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Repository;
using Serilog;
using Serilog.Formatting.Json;
using Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExamineApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {

                    services.UseAppJson(out IConfigurationRoot configuration);
                    services.AddSingleton<IConfigurationRoot>(cnfg =>
                    {
                        return configuration;
                    });
                    services.UseSerilog(configuration,out ILogger logger);
                    services.AddDbContext(configuration,logger);
                    services.AddHostedService<MessageSubscriber>();
                    services.AddSingleton<IConnectionFactory>(x =>
                    {
                        var connectionFactory = new ConnectionFactory();
                        var HostName = configuration["RabbitConfig:HostName"];
                        var Port = Convert.ToInt32(configuration["RabbitConfig:Port"]);
                        var UserName = configuration["RabbitConfig:UserName"];
                        var Password = configuration["RabbitConfig:Password"];

                        connectionFactory.Uri = new Uri($"amqp://{UserName}:{Password}@{HostName}:{Port}/");
                        return connectionFactory;
                    });
                    services.AddSingleton<IPersonSubscriber, PersonSubscriber>();
                    services.UseRedis(configuration);
                    services.AddScoped<IPersonService, PersonService>();
                    services.AddScoped<SqlConnection>(sql =>
                    {
                        return new SqlConnection(configuration["ConnectionStrings:Sql_Server"]);
                    });
                    services.AddScoped<IDapperUnitOfWork, DapperUnitOfWork>();
                });
    
    }
}