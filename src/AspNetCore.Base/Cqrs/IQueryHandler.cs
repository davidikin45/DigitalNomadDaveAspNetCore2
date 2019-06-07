﻿using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs
{
    public interface IQueryHandler<in TQuery, TResult>
    {
        Task<TResult> HandleAsync(string queryName, TQuery query, CancellationToken cancellationToken = default);
    }

    public interface ITypedQueryHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {

    }

    public interface IDynamicQueryHandler<in TQuery, TResult> : IQueryHandler<TQuery, TResult>
    {

    }
}
