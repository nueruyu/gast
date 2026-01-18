using Cysharp.Threading.Tasks;
using System.Threading;

namespace Gast.Lib.AI
{
    public interface ITask
    {
        string Name { get; }
    }

    public interface ITask<TWorldState, in TContext> : ITask
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        UniTask<bool> ValidateAsync(
            TWorldState worldState,
            CancellationToken cancellationToken);

        UniTask RunAsync(TContext ctx);
    }
}
