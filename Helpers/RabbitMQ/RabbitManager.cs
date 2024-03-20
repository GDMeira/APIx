using System.Text.Json;
using APIx.Config;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace APIx.Helpers.RabbitMQ;

public class RabbitManager(IOptions<QueueConfig> queueConfig, IPooledObjectPolicy<IModel> objectPolicy) : IRabbitManager
{
    private readonly QueueConfig _queueConfig = queueConfig.Value;
    private readonly DefaultObjectPool<IModel> _objectPool = new DefaultObjectPool<IModel>(objectPolicy, queueConfig.Value.MaxChannelCount);
    public void Publish(int paymentId, string routingKey)
    {
        // ConnectionFactory factory = new()
        // {
        //     HostName = _queueConfig.HostName,
        //     UserName = _queueConfig.UserName,
        //     Password = _queueConfig.Password,
        //     VirtualHost = _queueConfig.VirtualHost
        // };
        // IConnection connection = factory.CreateConnection();
        // using IModel channel = connection.CreateModel();
        using var channel = new PoolObject<IModel>(_objectPool);

        var body = JsonSerializer.SerializeToUtf8Bytes(paymentId);
        var properties = channel.Item.CreateBasicProperties();
        properties.Headers = new Dictionary<string, object>()
        {
            { "retry-count", 0 }
        };
        properties.Persistent = true;

        channel.Item.BasicPublish("", routingKey, properties, body);

        // connection.Close();
    }

    public QueueDeclareOk QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object>? arguments)
    {
        using var channel = new PoolObject<IModel>(_objectPool);
        return channel.Item.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
    }
}