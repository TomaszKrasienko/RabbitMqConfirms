﻿using RabbitMQ.Client;
using System.Text;

namespace Producer
{
    internal class Sender
    {
        private static ConnectionFactory? _connectionFactory;
        public Sender()
        {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 10000,
                UserName = "guest",
                Password = "guest"
            };
        }
        public void SendMessage(string msg, string routingKey)
        {
            if (string.IsNullOrEmpty(msg))
                return;
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var body = Encoding.UTF8.GetBytes(msg);
                IBasicProperties properties = channel.CreateBasicProperties();
                properties.ContentType = "application/json";
                channel.ConfirmSelect();
                channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
                channel.BasicPublish(
                    exchange: "dlx_main",
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: body
                    );
            }
        }
    }
}
