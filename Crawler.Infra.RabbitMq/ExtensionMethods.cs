using Crawler.Infra.RabbitMq.Pub;
using Crawler.Infra.RabbitMq.Sub;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Crawler.Infra.RabbitMq;

public static class ExtensionMethods
{
    public static IServiceCollection ConfigureRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        // if you want to bind config from environment variables
        //var host = Environment.GetEnvironmentVariable("RabbitHost");
        //var user = Environment.GetEnvironmentVariable("RabbitUser");
        //var pass = Environment.GetEnvironmentVariable("RabbitPass");

        var settings = new RabbitMqSettings();
        configuration.GetSection("RabbitMq").Bind(settings);
        services.AddSingleton(settings);

        if (settings.HostName is null || settings.User is null || settings.Pass is null) 
            throw new Exception("Invalid rabbitmq credentials");

        var connection = services.CreateConnection(settings);
        var channel = services.CreateChannel(connection);
        var manager = CreateManager(channel);
        
        services.ConfigurePublisher();

        // add queues
        manager.AddQueue(Constants.Queues.FirstTest);

        // add consumers
        services.AddConsumer(Constants.Queues.FirstTest)
                .AddQueueAndConsumer(manager, Constants.Queues.SecondTest);

        return services;
    }

    private static IServiceCollection ConfigurePublisher(this IServiceCollection services)
    {
        services.AddSingleton<MessagePublisher>();
        return services;
    }

    private static IConnection CreateConnection(this IServiceCollection services, RabbitMqSettings settings)
    {
        var factory = new ConnectionFactory
        {
            HostName = settings.HostName,
            UserName = settings.User,
            Password = settings.Pass
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

    public static RabbitMQQueueManager AddQueue(this RabbitMQQueueManager manager, string queue)
    {
        manager.DeclareQueue(queue);
        return manager;
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
