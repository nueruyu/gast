using System;
using System.Threading;
using Gast.Lib.AI;
using Cryst.Domain.Characters;
using Gast.Domain.Characters;

namespace Cryst.Features.CharacterAI
{
    public readonly struct AIContext<TWorldState> : IContext<AIContext<TWorldState>, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>, new()
    {
        public BaseCharacter Actor { get; }
        public ICharacter Character { get; }
        public TWorldState WorldState { get; }
        public AIMemory Memory { get; }
        public CancellationToken CancellationToken { get; }
        public ContextKey ContextKey { get; }

        readonly Action worldStateUpdater;

        public AIContext(
            ContextKey contextKey,
            BaseCharacter actor,
            ICharacter character,
            TWorldState worldState,
            AIMemory memory,
            Action worldStateUpdater,
            CancellationToken cancellationToken)
        {
            ContextKey = contextKey;
            Actor = actor;
            Character = character;
            WorldState = worldState;
            Memory = memory;
            CancellationToken = cancellationToken;
            this.worldStateUpdater = worldStateUpdater;
        }

        public void UpdateWorldState()
        {
            worldStateUpdater?.Invoke();
        }

        public AIContext<TWorldState> WithCancellationToken(CancellationToken cancellationToken)
        {
            return new AIContext<TWorldState>(
                ContextKey,
                Actor,
                Character,
                WorldState,
                Memory,
                worldStateUpdater,
                cancellationToken);
        }
    }
}