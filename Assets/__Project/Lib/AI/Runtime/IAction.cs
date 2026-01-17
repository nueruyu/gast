using Cysharp.Threading.Tasks;

namespace Gast.Lib.AI
{
    public interface IAction<in TWorldState, in TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        string Name { get; }

        bool CanExecute(TWorldState worldState);

        void Simulate(TWorldState worldState);

        UniTask ExecuteAsync(TContext ctx);
    }
}
