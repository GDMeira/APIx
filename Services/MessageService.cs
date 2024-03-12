using System.Text.Json;
using APIx.Config;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace APIx.Services;

public class MessageService(IOptions<QueueConfig> queueConfig)
{
    private readonly QueueConfig _queueConfig = queueConfig.Value;
    public void SendMessage(string message)
    {
        ConnectionFactory factory = new()
        {
            HostName = _queueConfig.HostName
        };

        IConnection connection = factory.CreateConnection();
        using IModel channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: _queueConfig.Queue,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        channel.BasicPublish(
            exchange: "",
            routingKey: _queueConfig.Queue,
            basicProperties: null,
            body: body
        );
    }
}