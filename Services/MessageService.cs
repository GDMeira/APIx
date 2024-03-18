using System.Text.Json;
using APIx.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace APIx.Services;

public class MessageService(IOptions<QueueConfig> queueConfig)
{
    private readonly QueueConfig _queueConfig = queueConfig.Value;
    public void SendMessage(int paymentId)
    {
        ConnectionFactory factory = new()
        {
            HostName = _queueConfig.HostName,
            UserName = _queueConfig.UserName,
            Password = _queueConfig.Password,
            VirtualHost = _queueConfig.VirtualHost
        };

        IConnection connection = factory.CreateConnection();
        using IModel channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: _queueConfig.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var body = JsonSerializer.SerializeToUtf8Bytes(paymentId);
        var properties = channel.CreateBasicProperties();
        properties.Headers = new Dictionary<string, object>()
        {
            { "retry-count", 0 }
        };
        properties.Persistent = true;

        channel.BasicPublish(
            exchange: "",
            routingKey: _queueConfig.Queue,
            basicProperties: properties,
            body: body
        );
    }
}