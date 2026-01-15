using System.Threading;
using Gast.Domain.Characters;
using Gast.Lib.AI;

namespace Gast.Features.Npcs
{
    public readonly struct AIContext<TWorldState> : IContext<TWorldState>
        where TWorldState : class, IWorldState<TWorldState>, new()
    {
        public ICharacter Actor { get; }
        public TWorldState WorldState { get; }
        public CancellationToken CancellationToken { get; }

        public AIContext(ICharacter actor, TWorldState worldState, CancellationToken cancellationToken)
        {
            Actor = actor;
            WorldState = worldState;
            CancellationToken = cancellationToken;
        }
    }
}
