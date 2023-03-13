using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Consumer
{
    internal class Receiver : BackgroundService
    {
        private static ConnectionFactory? _connectionFactory;
        private static IConnection? _connection;
        private static IModel? _channel;
        public Receiver()
        {
            InitializeRabbitMqListener();
        }
        private void InitializeRabbitMqListener()
        {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 10000,
                UserName = "guest",
                Password = "guest"
            };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _connection.ConnectionShutdown += RabbitMq_ConnectionShutDow;
        }
        private void RabbitMq_ConnectionShutDow(object sender, ShutdownEventArgs e) => Console.WriteLine("Connection shut down");
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while(!(stoppingToken.IsCancellationRequested))
            //{
            //    Console.WriteLine("coś tam");
            //}
            //return Task.CompletedTask;
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.Span);
                HandleMessage(content, stoppingToken);
                if (content == "negative")
                {
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
                else
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            consumer.Shutdown += OnConsumerShoutDown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerCancelled;
            _channel.BasicConsume("test.queue", false, consumer);
            return Task.CompletedTask;
        }
        private async Task HandleMessage(string content, CancellationToken cancellationToken)
        {
            Console.WriteLine(content);
        }
        private void OnConsumerShoutDown(object sender, ShutdownEventArgs eventArgs) => Console.WriteLine("Consumer shut down");
        private void OnConsumerRegistered(object sender, ConsumerEventArgs eventArgs) => Console.WriteLine("Consumer registered");
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs eventArgs) => Console.WriteLine("Consumer unregistered");
        private void OnConsumerCancelled(object sender, ConsumerEventArgs eventArgs) => Console.WriteLine("Consumer cancelled");

    }
}
