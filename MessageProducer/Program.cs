using Common;
using Data.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;

namespace MessageProducer

{
    class Program
    {
        private static Timer aTimer;
        public static void Main()
        {
            // Create a timer and set a two second interval.
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 2000;

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;

            // Start the timer
            aTimer.Enabled = true;

            Console.WriteLine("Press the Enter key to exit the program at any time... ");
            Console.ReadLine();
        }

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            var dummy = new DummyData();
            var person = new Person();
            person.Age = (new Random()).Next(0,50);
            person.FirstName = dummy.GenerateName();
            person.LastName = dummy.GenerateLastName();
            PublishMessage(person);
        }

        public static void PublishMessage(Person person)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            var factory = new ConnectionFactory();

            var HostName = configuration["RabbitConfig:HostName"];
            var Port = Convert.ToInt32(configuration["RabbitConfig:Port"]);
            var UserName = configuration["RabbitConfig:UserName"];
            var Password = configuration["RabbitConfig:Password"];

            factory.Uri = new Uri($"amqp://{UserName}:{Password}@{HostName}:{Port}/");
            using var connection = factory.CreateConnection();

            using var chanel = connection.CreateModel();
            chanel.QueueDeclare(configuration["RabbitConfig:PersonQueue"],
                durable: true, exclusive: false,
                autoDelete: false);

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(person));
            chanel.BasicPublish("", configuration["RabbitConfig:PersonQueue"], null, body);
        }


    }
}
