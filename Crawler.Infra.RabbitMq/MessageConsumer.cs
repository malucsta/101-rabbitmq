using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Crawler.Infra.RabbitMq;

public class MessageConsumer
{
    private readonly IModel _channel;
    private readonly string _queueName;

    public MessageConsumer(IModel channel, string queueName)
    {
        _channel = channel;
        _queueName = queueName;
    }

    public void StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received message: {message}");
        };

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
    }
}
