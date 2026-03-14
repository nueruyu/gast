using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.Lib.AI
{
    public interface IAction
    {
        UniTask ExecuteAsync(CancellationToken cancellationToken);
    }

    public interface IAction<in TActorContext, in TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        bool IsAvailable(TWorldState worldState);

        void Simulate(TWorldState worldState);

        UniTask ExecuteAsync(TActorContext context, CancellationToken cancellationToken);
    }
}