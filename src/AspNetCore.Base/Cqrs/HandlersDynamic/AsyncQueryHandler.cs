using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs.HandlersDynamic
{
    public abstract class AsyncDynamicQueryHandler<TQuery, TResult> : IDynamicQueryHandler<TQuery, TResult>
    {
        public abstract Task<TResult> HandleAsync(string queryName, TQuery query, CancellationToken cancellationToken = default);
    }

    public abstract class AsyncDynamicRequestQueryHandler<TResult> : AsyncDynamicQueryHandler<dynamic, TResult>
    {

    }

    public abstract class AsyncDynamicRequestResponseQueryHandler : AsyncDynamicQueryHandler<dynamic, object>
    {

    }
}
