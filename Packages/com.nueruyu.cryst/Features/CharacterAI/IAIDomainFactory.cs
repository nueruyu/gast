using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI
{
    public interface IAIDomainFactory<TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
    {
        AIDomain<ActorContext<TWorldState>, TWorldState> CreateDomain();
    }
}
