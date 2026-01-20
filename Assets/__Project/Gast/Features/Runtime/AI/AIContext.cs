using System.Threading;
using Gast.Domain.Characters;
using Gast.Lib.AI;

namespace Gast.Features.AI
{
    public readonly struct AIContext<TWorldState> : IContext<AIContext<TWorldState>, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>, new()
    {
        public ICharacter Actor { get; }
        public TWorldState WorldState { get; }
        public AIMemory Memory { get; }
        public string DomainName { get; }
        public CancellationToken CancellationToken { get; }

        public ContextKey ContextKey { get; }

        public AIContext(ICharacter actor, TWorldState worldState, AIMemory memory, string domainName, CancellationToken cancellationToken)
        {
            Actor = actor;
            WorldState = worldState;
            Memory = memory;
            DomainName = domainName;
            CancellationToken = cancellationToken;
            ContextKey = new ContextKey(actor.Id, domainName);
        }

        public AIContext<TWorldState> WithCancellationToken(CancellationToken cancellationToken)
        {
            return new AIContext<TWorldState>(Actor, WorldState, Memory, DomainName, cancellationToken);
        }
    }
}