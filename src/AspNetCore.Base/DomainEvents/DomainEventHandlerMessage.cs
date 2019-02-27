using Newtonsoft.Json;
using System;

namespace AspNetCore.Base.DomainEvents
{
    public class DomainEventHandlerMessage
    {
        public DomainEventHandlerMessage(string handlerType, DomainEventMessage domainEventMessage)
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            HandlerType = handlerType;
            DomainEventMessage = domainEventMessage;
        }

        [JsonConstructor]
        public DomainEventHandlerMessage(Guid id, DateTime createDate, string handlerType, DomainEventMessage domainEventMessage)
        {
            Id = id;
            CreationDate = createDate;
            HandlerType = handlerType;
            DomainEventMessage = domainEventMessage;
        }

        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public DateTime CreationDate { get; private set; }

        [JsonProperty]
        public string HandlerType { get; private set; }

        [JsonProperty]
        public DomainEventMessage DomainEventMessage { get; private set; }
    }
}
