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
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken);

        UniTask RunAsync(ExecutionContext<TActorContext> context, CancellationToken cancellationToken);

        UniTask SimulateAsync(SimulationContext<TWorldState> context, CancellationToken cancellationToken);
    }
}
