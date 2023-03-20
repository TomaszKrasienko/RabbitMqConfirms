using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

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
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.Span);
                try
                {
                    await HandleMessage(content, stoppingToken);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(content);
                    _channel.BasicReject(ea.DeliveryTag, false);
                }
                
            };
            consumer.Shutdown += OnConsumerShoutDown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerCancelled;
            _channel.BasicConsume("rejected.queue", false, consumer);
        }
        private async Task HandleMessage(string content, CancellationToken cancellationToken)
        {
            if (content == "throw")
                throw new Exception();
            Console.WriteLine(content);
        }
        private void OnConsumerShoutDown(object sender, ShutdownEventArgs eventArgs) => Console.WriteLine("Consumer shut down");
        private void OnConsumerRegistered(object sender, ConsumerEventArgs eventArgs) => Console.WriteLine("Consumer registered");
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs eventArgs) => Console.WriteLine("Consumer unregistered");
        private void OnConsumerCancelled(object sender, ConsumerEventArgs eventArgs) => Console.WriteLine("Consumer cancelled");
    }
}
