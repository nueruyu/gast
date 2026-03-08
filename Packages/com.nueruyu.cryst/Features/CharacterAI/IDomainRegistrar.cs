using System;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI
{
    public interface IDomainRegistrar
    {
        void Register<TWorldState>(
            string domainName,
            AIDomain<ActorContext<TWorldState>, TWorldState> domain,
            TWorldState worldState,
            Action<ActorContext<TWorldState>> worldStateUpdater)
            where TWorldState : class, IWorldState<TWorldState>;
    }
}