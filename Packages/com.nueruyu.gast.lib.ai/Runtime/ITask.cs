using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Testing;

namespace Gast.Lib.AI
{
    public interface ITask
    {
        string Name { get; }
    }

    public interface ITask<TActorContext, TWorldState> : ITask
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        UniTask<bool> ValidateAsync(
            TWorldState worldState,
            CancellationToken cancellationToken);

        UniTask RunAsync(AIContext<TActorContext> context, CancellationToken cancellationToken);

        /// <summary>
        /// Executes a simulation of the task's logic, collecting execution details into the context.
        /// </summary>
        UniTask SimulateAsync(SimulationContext<TWorldState> context, CancellationToken cancellationToken);
    }
}
