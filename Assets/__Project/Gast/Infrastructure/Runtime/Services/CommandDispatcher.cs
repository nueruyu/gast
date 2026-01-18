using Gast.Core.Commands;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VContainer;

namespace Gast.Infrastructure.Services
{
    public class CommandDispatcher : ICommandDispatcher
    {
        readonly IObjectResolver resolver;

        public CommandDispatcher(IObjectResolver resolver)
        {
            this.resolver = resolver;
        }

        public void Dispatch<TCommand>(in TCommand command) where TCommand : struct, ICommand
        {
            var handler = resolver.Resolve<ICommandHandler<TCommand>>();
            handler.Execute(command);
        }

        public TResult Dispatch<TCommand, TResult>(in TCommand command) where TCommand : struct, ICommand<TResult>
        {
            var handler = resolver.Resolve<ICommandHandler<TCommand, TResult>>();
            return handler.Execute(command);
        }

        public ValueTask DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : struct, IAsyncCommand
        {
            var handler = resolver.Resolve<IAsyncCommandHandler<TCommand>>();
            return handler.ExecuteAsync(command, cancellationToken);
        }

        public ValueTask<TResult> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default) where TCommand : struct, IAsyncCommand<TResult>
        {
            var handler = resolver.Resolve<IAsyncCommandHandler<TCommand, TResult>>();
            return handler.ExecuteAsync(command, cancellationToken);
        }
    }
}