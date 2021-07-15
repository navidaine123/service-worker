using Data.Models;
using ExamineApplication.RabbitMqServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Service;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamineApplication
{
    public class MessageSubscriber : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private string _consumerTag;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IPersonSubscriber _personSubscriber;
        private readonly ILogger<MessageSubscriber> _logger;
        private IPersonService _personService;

        public MessageSubscriber(ILogger<MessageSubscriber> logger, 
            IConfiguration configuration, 
            IServiceScopeFactory serviceScopeFactory,
            IPersonSubscriber personSubscriber)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _personSubscriber = personSubscriber;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _personSubscriber.Consume();
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _personSubscriber.Disconnect();
            return base.StopAsync(cancellationToken);
        }
    }
}