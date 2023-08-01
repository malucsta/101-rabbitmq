using RabbitMQ.Client;

namespace Crawler.Infra.RabbitMq;

public class RabbitMQHelper
{
    private IConnection _connection;
    private IModel _channel;

    public void Connect(string hostName, string userName, string password)
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Close()
    {
        _channel?.Close();
        _connection?.Close();
    }

    public IModel GetChannel()
    {
        return _channel;
    }
}
