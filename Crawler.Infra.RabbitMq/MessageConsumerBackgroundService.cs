using System.Text;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Crawler.Infra.RabbitMq;
public class MessageConsumerBackgroundService : BackgroundService
{
    private readonly IModel _channel;
    private readonly string _queueName;

    public MessageConsumerBackgroundService(IModel channel, string queueName)
    {
        _channel = channel;
        _queueName = queueName;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // Process the messsage based on the queue name
            switch (_queueName)
            {
                case "Queue1":
                    Console.WriteLine($"Queue1 - Received message: {message}");
                    break;

                case "Queue2":
                    Console.WriteLine($"Queue2 - Received message: {message}");
                    break;
                case "teste-queue":
                    Console.WriteLine($"teste-queue - Received message: {message}");
                    break;
                case "teste2":
                    Console.WriteLine($"teste2 - Received message: {message}");
                    break;

                default:
                    break;
            }
        };

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
