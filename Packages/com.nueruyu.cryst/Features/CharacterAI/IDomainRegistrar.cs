using System;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI
{
    public interface IDomainRegistrar
    {
        void Register<TWorldState>(
            string domainName,
            IDomainProvider<ActorContext<TWorldState>, TWorldState> domainProvider,
            TWorldState worldState,
            Action<ActorContext<TWorldState>> worldStateUpdater)
            where TWorldState : class, IWorldState<TWorldState>;

        void RegisterPreUpdate(Action action);
    }
}