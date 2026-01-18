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
        public CancellationToken CancellationToken { get; }

        public AIContext(ICharacter actor, TWorldState worldState, AIMemory memory, CancellationToken cancellationToken)
        {
            Actor = actor;
            WorldState = worldState;
            Memory = memory;
            CancellationToken = cancellationToken;
        }

        public AIContext<TWorldState> WithCancellationToken(CancellationToken cancellationToken)
        {
            return new AIContext<TWorldState>(Actor, WorldState, Memory, cancellationToken);
        }
    }
}