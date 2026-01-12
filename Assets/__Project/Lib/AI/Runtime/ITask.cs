using Cysharp.Threading.Tasks;
using System.Threading;

namespace Gast.Lib.AI
{
    public interface ITask<TWorldState> where TWorldState : struct
    {
        string Name { get; }

        UniTask<(bool, TWorldState)> ValidateAsync(
            TWorldState state,
            CheckOptions options,
            CancellationToken cancellationToken);

        UniTask RunAsync(Context<TWorldState> ctx, CheckOptions? options = null);
    }
}