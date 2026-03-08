using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI
{
    public interface IWorldStateUpdater<TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
    {
        void Update(ActorContext<TWorldState> context);
    }
}
