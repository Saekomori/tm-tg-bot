using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aikso_bot
{
    internal class RabbitMQSubscriber
    {
        private readonly string rabbitMqHost = "";
        private readonly int rabbitMqPort = 5672; 
        private readonly string rabbitMqVirtualHost = "cpp"; 
        private readonly string rabbitMqUserName = "admin"; 
        private readonly string rabbitMqPassword = "admin"; 
        private readonly string rabbitMqQueueName = "tmQueues"; 

        public event EventHandler<string> MessageReceived;

        public void StartListening()
        {
            var factory = new ConnectionFactory()
            {
                HostName = rabbitMqHost,
                Port = rabbitMqPort,
                VirtualHost = rabbitMqVirtualHost,
                UserName = rabbitMqUserName,
                Password = rabbitMqPassword
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: rabbitMqQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                MessageReceived?.Invoke(this, message);
            };

            channel.BasicConsume(queue: rabbitMqQueueName, autoAck: true, consumer: consumer);
        }
    }
}
