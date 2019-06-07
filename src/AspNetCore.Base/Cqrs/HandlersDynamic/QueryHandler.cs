using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs.HandlersDynamic
{
    public abstract class DynamicQueryHandler<TQuery, TResult> : IDynamicQueryHandler<TQuery, TResult>
    {
        public Task<TResult> HandleAsync(string queryName, TQuery query, CancellationToken cancellationToken)
          => Task.FromResult(Handle(queryName, query));

        protected abstract TResult Handle(string queryName, TQuery query);
    }

    public abstract class DynamicRequestQueryHandler<TResult> : DynamicQueryHandler<dynamic, TResult>
    {

    }

    public abstract class DynamicRequestResponseQueryHandler : DynamicQueryHandler<dynamic, dynamic>
    {


    }
}
