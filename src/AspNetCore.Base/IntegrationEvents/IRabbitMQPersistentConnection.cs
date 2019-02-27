using RabbitMQ.Client;
using System;

namespace AspNetCore.Base.IntegrationEvents
{
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
