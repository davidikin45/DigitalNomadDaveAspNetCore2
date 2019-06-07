using AspNetCore.Base.Validation;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.Cqrs.Decorators.Command
{
    public sealed class AuditLoggingDecorator<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    {
        private readonly ICommandHandler<TCommand, TResult> _handler;

        public AuditLoggingDecorator(ICommandHandler<TCommand, TResult> handler)
        {
            _handler = handler;
        }

        public async Task<Result<TResult>> HandleAsync(string commandName, TCommand command, CancellationToken cancellationToken = default)
        {
            string commandJson = JsonConvert.SerializeObject(command);

            // Use proper logging here
            Console.WriteLine($"Command of type {command.GetType().Name}: {commandJson}");

            return await _handler.HandleAsync(commandName, command, cancellationToken);
        }
    }
}
