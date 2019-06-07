using AspNetCore.Base.Settings;
using AspNetCore.Base.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs.Decorators.Command
{
    public sealed class DatabaseRetryDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    {
        private readonly ICommandHandler<TCommand, TResult> _handler;
        private readonly AppSettings _appSettings;

        public DatabaseRetryDecorator(ICommandHandler<TCommand, TResult> handler, AppSettings appSettings)
        {
            _appSettings = appSettings;
            _handler = handler;
        }

        public async Task<Result<TResult>> HandleAsync(string commandName, TCommand command, CancellationToken cancellationToken = default)
        {
            for (int i = 0; ; i++)
            {
                try
                {
                    Result<TResult> result = await _handler.HandleAsync(commandName, command, cancellationToken);
                    return result;
                }
                catch (Exception ex)
                {
                    if (i >= _appSettings.NumberOfDatabaseRetries || !IsDatabaseException(ex))
                        throw;
                }
            }
        }

        private bool IsDatabaseException(Exception exception)
        {
            string message = exception.InnerException?.Message;

            if (message == null)
                return false;

            return message.Contains("The connection is broken and recovery is not possible")
                || message.Contains("error occurred while establishing a connection");
        }
    }
}
