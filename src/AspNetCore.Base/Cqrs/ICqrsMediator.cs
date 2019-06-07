﻿using AspNetCore.Base.DomainEvents.Subscriptions;
using AspNetCore.Base.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs
{
    public interface ICqrsMediator
    {
        ICqrsCommandSubscriptionsManager CqrsCommandSubscriptionManager { get; }
        ICqrsQuerySubscriptionsManager CqrsQuerySubscriptionManager { get; }

        Task<Result<dynamic>> DispatchCommandAsync(string commandName, string payload, CancellationToken cancellationToken = default);
        Task<Result<T>> DispatchAsync<T>(ICommand<T> command, CancellationToken cancellationToken = default);

        Task<dynamic> DispatchQueryAsync(string queryName, string payload, CancellationToken cancellationToken = default);
        Task<T> DispatchAsync<T>(IQuery<T> query, CancellationToken cancellationToken = default);
    }
}
