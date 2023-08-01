using System.Text;
using RabbitMQ.Client;

namespace Crawler.Infra.RabbitMq.Pub;
public class MessagePublisher
{
    private readonly IModel _channel;

    public MessagePublisher(IModel channel)
    {
        _channel = channel;
    }

    public void PublishMessage(string exchangeName, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: null, body: body);
    }

    public void PublishMessageAtQueue(string queueName, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        // can add it here to guarantee that the queue is created
        //_channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        
        _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
    }
}
