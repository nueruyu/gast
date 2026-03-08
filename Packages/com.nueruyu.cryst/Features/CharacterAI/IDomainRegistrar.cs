using System;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI
{
    public interface IDomainRegistrar
    {
        void Register<TWorldState>(
            string domainName,
            AIRunner<ActorContext<TWorldState>, TWorldState> runner,
            TWorldState worldState,
            Action<ActorContext<TWorldState>> worldStateUpdater)
            where TWorldState : class, IWorldState<TWorldState>;
    }
}