using AspNetCore.Base.Data.UnitOfWork;
using AspNetCore.Base.MultiTenancy;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Transactions;

namespace AspNetCore.Base.Extensions
{
    public static class MvcServiceCollectionExtensions
    {
        public static IServiceCollection AddViewLocationExpander(this IServiceCollection services, string mvcImplementationFolder = "Mvc/")
        {
            return services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Insert(0, new ViewLocationExpander(mvcImplementationFolder));
            });
        }

        public static void UseOneTransactionPerHttpCall(this IServiceCollection serviceCollection, System.Transactions.IsolationLevel level = System.Transactions.IsolationLevel.ReadCommitted)
        {
            serviceCollection.AddScoped<TransactionScope>((serviceProvider) =>
            {
                var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = level });
                return transactionScope;
            });

            serviceCollection.AddScoped(typeof(UnitOfWorkFilter), typeof(UnitOfWorkFilter));

            serviceCollection
                .AddMvc(setup =>
                {
                    setup.Filters.AddService<UnitOfWorkFilter>(1);
                });
        }
    }
}
