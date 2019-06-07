using AspNetCore.Base.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.IntegrationEvents.HandlersDynamic
{
    public abstract class AsyncDynamicIntegrationEventHandler<TIntegrationEvent> : IDynamicIntegrationEventHandler<TIntegrationEvent>
    {
        public abstract Task<Result> HandleAsync(string eventName, TIntegrationEvent domainEvent, CancellationToken cancellationToken = default);

    }

    public abstract class AsyncDynamicRequestIntegrationEventHandler : AsyncDynamicIntegrationEventHandler<dynamic>
    {

    }
}
