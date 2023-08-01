using System.Text;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Crawler.Infra.RabbitMq.Sub;
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
                case Constants.Queues.FirstTest:
                    Console.WriteLine($"{Constants.Queues.FirstTest} - Received message: {message}");
                    break;
                case Constants.Queues.SecondTest:
                    Console.WriteLine($"{Constants.Queues.SecondTest} - Received message: {message}");
                    break;
                default:
                    break;
            }
        };

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
