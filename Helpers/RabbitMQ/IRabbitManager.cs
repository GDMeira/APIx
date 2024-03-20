using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace APIx.Helpers.RabbitMQ
{
    public interface IRabbitManager
    {
        void Publish(int message, string routingKey);

        QueueDeclareOk QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object>? arguments);
    }

}
