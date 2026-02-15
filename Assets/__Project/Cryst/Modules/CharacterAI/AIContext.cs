using System;
using System.Threading;
using Gast.Lib.AI;
using Cryst.Domain.Characters;

namespace Cryst.Modules.CharacterAI
{
    public readonly struct AIContext<TWorldState> : IContext<AIContext<TWorldState>, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>, new()
    {
        public ICrystCharacter Actor { get; }
        public TWorldState WorldState { get; }
        public AIMemory Memory { get; }
        public CancellationToken CancellationToken { get; }
        public ContextKey ContextKey { get; }

        readonly Action worldStateUpdater;

        public AIContext(
            ContextKey contextKey,
            ICrystCharacter actor,
            TWorldState worldState,
            AIMemory memory,
            Action worldStateUpdater,
            CancellationToken cancellationToken)
        {
            ContextKey = contextKey;
            Actor = actor;
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
                WorldState,
                Memory,
                worldStateUpdater,
                cancellationToken);
        }
    }
}