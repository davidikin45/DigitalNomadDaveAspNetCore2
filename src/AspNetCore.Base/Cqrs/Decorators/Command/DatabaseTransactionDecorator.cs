using AspNetCore.Base.Settings;
using AspNetCore.Base.Validation;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace AspNetCore.Base.Cqrs.Decorators.Command
{
    public sealed class DatabaseTransactionDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;
        private readonly TransactionScope _transactionScope;

        public DatabaseTransactionDecorator(ICommandHandler<TCommand> handler)
        {
            _handler = handler;
            _transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted });
        }

        public async Task<Result> HandleAsync(TCommand command)
        {
            try
            {
                Result result = await _handler.HandleAsync(command);

                _transactionScope.Complete();

                return result;
            }
            catch (Exception)
            {
                if (_transactionScope != null)
                {
                    _transactionScope.Dispose();
                }
                throw;
            }
        }
    }
}
