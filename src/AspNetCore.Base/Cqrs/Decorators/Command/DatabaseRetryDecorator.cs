using AspNetCore.Base.Settings;
using AspNetCore.Base.Validation;
using System;
using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs.Decorators.Command
{
    public sealed class DatabaseRetryDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _handler;
        private readonly AppSettings _appSettings;

        public DatabaseRetryDecorator(ICommandHandler<TCommand> handler, AppSettings appSettings)
        {
            _appSettings = appSettings;
            _handler = handler;
        }

        public async Task<Result> HandleAsync(TCommand command)
        {
            for (int i = 0; ; i++)
            {
                try
                {
                    Result result = await _handler.HandleAsync(command);
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
