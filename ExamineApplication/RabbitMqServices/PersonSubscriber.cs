using Common.LogModels;
using Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamineApplication.RabbitMqServices
{
    public class PersonSubscriber:IPersonSubscriber
    {
        private IConnection _connection;
        private IModel _channel;
        private string _consumerTag;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private IPersonService _personService;
        private readonly ILogger<PersonSubscriber> _logger;

        public PersonSubscriber(IConnectionFactory connectionFactory, 
            IServiceScopeFactory serviceScopeFactory,
            IConfiguration configuration,
            ILogger<PersonSubscriber> logger)
        {
            _connectionFactory = connectionFactory;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task Consume()
        {
            try
            {
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            }
            catch (Exception ex)
            {
                _logger.LogError("RabbitMQ Connection Hasfailed{@RabbitError}", new LogModel(
                    new ErrorLogModel(ex)));
            }

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var person = JsonConvert.DeserializeObject<Person>(message);
                _logger.LogInformation("Person From Recieved RabbitMQ {@RabbitLog}", new LogModel(person));

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    _personService = scope.ServiceProvider.GetService<IPersonService>();
                    _personService.AddPerson(person);
                }



                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            var queueName = _configuration["RabbitConfig:PersonQueue"];
            _consumerTag = _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        public void Disconnect()
        {
            _channel.BasicCancel(_consumerTag);
            _channel.Close();
            _connection.Close();
        }

    }
}
