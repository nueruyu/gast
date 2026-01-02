using Cysharp.Threading.Tasks;

namespace Gast.Lib.AI
{
    public interface ITask<TWorldState> where TWorldState : struct
    {
        string Name { get; }

        bool Validate(
            ref TWorldState state,
            CheckOptions options,
            ISimulationContext context,
            IEnvironmentModel<TWorldState> environment = null
        );

        UniTask RunAsync(Context<TWorldState> ctx);
    }
}