using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.Lib.AI
{
    public interface ITask
    {
        string Name { get; }
    }

    public interface ITask<TActorContext, in TWorldState> : ITask
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        UniTask<bool> ValidateAsync(
            TWorldState worldState,
            CancellationToken cancellationToken);

        UniTask RunAsync(AIContext<TActorContext> context, CancellationToken cancellationToken);
    }
}