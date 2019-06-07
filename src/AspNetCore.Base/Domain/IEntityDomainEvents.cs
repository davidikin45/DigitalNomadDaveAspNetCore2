using AspNetCore.Base.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Base.Domain
{
    public interface IEntityDomainEvents
    {
        IReadOnlyCollection<DomainEvent> DomainEvents { get; }
        void AddDomainEvent(DomainEvent eventItem);
        void RemoveDomainEvent(DomainEvent eventItem);
        void ClearDomainEvents();
    }
}
