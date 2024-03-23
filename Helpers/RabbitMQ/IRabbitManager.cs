using RabbitMQ.Client;

namespace APIx.Helpers.RabbitMQ
{
    public interface IRabbitManager
    {
        void Publish(int id, string routingKey, bool headers = false);
        QueueDeclareOk QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object>? arguments);
    }

}
