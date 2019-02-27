using AspNetCore.Base.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Base.Domain
{
    public interface IEntityDomainEvents
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void AddDomainEvent(IDomainEvent eventItem);
        void RemoveDomainEvent(IDomainEvent eventItem);
        void ClearDomainEvents();
    }
}
