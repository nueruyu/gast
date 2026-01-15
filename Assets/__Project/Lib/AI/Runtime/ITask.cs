using Cysharp.Threading.Tasks;
using System.Threading;

namespace Gast.Lib.AI
{
    public interface ITask<TWorldState, in TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TWorldState>
    {
        string Name { get; }

        UniTask<bool> ValidateAsync(
            TWorldState worldState,
            CheckOptions options,
            CancellationToken cancellationToken);

        UniTask RunAsync(TContext ctx, CheckOptions? options);
    }
}
