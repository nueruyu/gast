using Gast.Application;
using Gast.Core.Commands;
using System;
using System.Linq;
using System.Reflection;
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

        public object Dispatch(object command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var commandType = command.GetType();
            var commandInterfaces = commandType.GetInterfaces();

            var resultCommandInterface = commandInterfaces.FirstOrDefault(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(ICommand<>));

            MethodInfo dispatchMethod;

            if (resultCommandInterface != null)
            {
                // ICommand<TResult>
                var resultType = resultCommandInterface.GetGenericArguments()[0];
                dispatchMethod = GetType()
                    .GetMethods()
                    .First(m => m.Name == nameof(Dispatch) &&
                        m.IsGenericMethodDefinition &&
                        m.GetGenericArguments().Length == 2)
                    .MakeGenericMethod(commandType, resultType);
            }
            else if (commandInterfaces.Contains(typeof(ICommand)))
            {
                // ICommand
                dispatchMethod = GetType()
                    .GetMethods()
                    .First(m => m.Name == nameof(Dispatch) &&
                        m.IsGenericMethodDefinition &&
                        m.GetGenericArguments().Length == 1)
                    .MakeGenericMethod(commandType);
            }
            else
            {
                throw new InvalidOperationException($"The provided object is not a valid command: {commandType.Name}");
            }

            return dispatchMethod.Invoke(this, new[] { command });
        }
    }
}