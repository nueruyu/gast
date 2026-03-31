using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Testing;

namespace Gast.Lib.AI.Tasks
{
    class LazyTask<TActorContext, TWorldState> : ITask<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly Func<ITask<TActorContext, TWorldState>> factory;
        ITask<TActorContext, TWorldState> inner;

        ITask<TActorContext, TWorldState> Inner => inner ??= factory();

        internal LazyTask(Func<ITask<TActorContext, TWorldState>> factory)
        {
            this.factory = factory;
        }

        public string Name => Inner.Name;

        public UniTask<bool> ValidateAsync(ValidationContext<TWorldState> context, CancellationToken cancellationToken)
            => Inner.ValidateAsync(context, cancellationToken);

        public UniTask SimulateAsync(SimulationContext<TWorldState> context, CancellationToken cancellationToken)
            => Inner.SimulateAsync(context, cancellationToken);

        public UniTask RunAsync(ExecutionContext<TActorContext> context, CancellationToken cancellationToken)
            => Inner.RunAsync(context, cancellationToken);
    }
}
