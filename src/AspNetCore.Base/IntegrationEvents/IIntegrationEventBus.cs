using AspNetCore.Base.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.IntegrationEvents
{
    public interface IIntegrationEventBus
    {
        Task PublishAsync(IntegrationEvent integrationEvent);

        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;

        void SubscribeDynamic<TIntegrationEvent,TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler<TIntegrationEvent>;

        void UnsubscribeDynamic<TIntegrationEvent,TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler<TIntegrationEvent>;

        Task ProcessEventAsync(string eventName, string payload);

        //Task ProcessEventHandlerAsync(string eventName, string payload, string handlerType, int handlerIndex);
    }
}
