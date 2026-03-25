using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI
{
    public interface IAIDomainFactory<TWorldState>
        : IDomainProvider<ActorContext<TWorldState>, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
    {
    }
}
