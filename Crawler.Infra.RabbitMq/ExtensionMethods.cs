using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Threading.Channels;

namespace Crawler.Infra.RabbitMq;

public static class ExtensionMethods
{
    public static IServiceCollection ConfigureRabbitMq(this IServiceCollection services)
    {
        var connection = services.CreateConnection("rabbitmq", "guest", "guest");
        var channel = services.CreateChannel(connection);
        var manager = CreateManager(channel);
        
        services.ConfigurePublisher();

        manager.AddQueues();
        
        services.AddConsumer("teste-queue")
                .AddQueueAndConsumer(manager, "teste2");

        return services;
    }

    private static IServiceCollection ConfigurePublisher(this IServiceCollection services)
    {
        services.AddSingleton<MessagePublisher>();
        return services;
    }

    private static IConnection CreateConnection(this IServiceCollection services, string hostName, string userName, string password)
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        };

        var connection = factory.CreateConnection();
        services.AddSingleton<IConnection>(connection);
        return connection;
    }

    private static IModel CreateChannel(this IServiceCollection services, IConnection connection)
    {
        var channel = connection.CreateModel();
        services.AddSingleton<IModel>(channel);
        return channel;
    }

    private static RabbitMQQueueManager CreateManager(IModel channel)
    {
        return new RabbitMQQueueManager(channel);
    }

    public static IServiceCollection AddQueueAndConsumer(this IServiceCollection services, RabbitMQQueueManager manager, string queue)
    {
        manager.DeclareQueue(queue);
        services.AddConsumer(queue);
        return services;
    }

    public static RabbitMQQueueManager AddExchange(this RabbitMQQueueManager manager, string exchange, string queueType)
    {
        manager.DeclareExchange(exchange, queueType);
        return manager;
    }

    public static RabbitMQQueueManager BindQueueToExchange(this RabbitMQQueueManager manager, string exchange, string queue)
    {
        manager.BindQueueToExchange(queue, exchange);
        return manager;
    }

    public static void AddQueues(this RabbitMQQueueManager manager)
    {
        manager.DeclareQueue("teste-queue");
    }

    public static IServiceCollection AddConsumer(this IServiceCollection services, string queueName)
    {
        services.AddSingleton<IHostedService>(provider =>
        {
            var channel = provider.GetRequiredService<IModel>();
            return new MessageConsumerBackgroundService(channel, queueName);
        });

        return services;
    }

    #region Adds queue with binding to exchange and consumer at background service
    //public static IServiceCollection AddRabbitMQQueue(this IServiceCollection services, string exchangeName, string queueName, string queueType)
    //{
    //    services.AddSingleton<IHostedService>(provider =>
    //    {
    //        var channel = provider.GetRequiredService<IModel>();

    //        var queueManager = new RabbitMQQueueManager(channel);
    //        queueManager.DeclareExchange(exchangeName, queueType);   
    //        queueManager.DeclareQueue(queueName);
    //        queueManager.BindQueueToExchange(queueName, exchangeName);

    //        return new MessageConsumerBackgroundService(channel, queueName);
    //    });

    //    return services;
    //}
    #endregion
}
