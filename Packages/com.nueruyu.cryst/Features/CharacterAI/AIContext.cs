using System;
using System.Threading;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Pickups;
using Gast.Lib.AI;
using Cryst.Domain.Characters;

namespace Cryst.Features.CharacterAI
{
    public readonly struct AIContext<TWorldState> : IContext<AIContext<TWorldState>, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>, new()
    {
        readonly AIBrainServices services;
        readonly Action worldStateUpdater;

        public BaseCharacter Actor { get; }
        public ICharacter Character { get; }
        public AIMemory Memory { get; }
        public ICharacterRepository CharacterRepository => services.CharacterRepository;
        public IPickupRepository PickupRepository => services.PickupRepository;
        public ICommandDispatcher CommandDispatcher => services.CommandDispatcher;

        public TWorldState WorldState { get; }
        public CancellationToken CancellationToken { get; }
        public ContextKey ContextKey { get; }

        public AIContext(
            ContextKey contextKey,
            AIBrainServices services,
            BaseCharacter actor,
            ICharacter character,
            AIMemory memory,
            TWorldState worldState,
            Action worldStateUpdater,
            CancellationToken cancellationToken)
        {
            ContextKey = contextKey;
            this.services = services;
            Actor = actor;
            Character = character;
            Memory = memory;
            WorldState = worldState;
            this.worldStateUpdater = worldStateUpdater;
            CancellationToken = cancellationToken;
        }

        public void UpdateWorldState()
        {
            worldStateUpdater?.Invoke();
        }

        public AIContext<TWorldState> WithCancellationToken(CancellationToken cancellationToken)
        {
            return new AIContext<TWorldState>(
                ContextKey,
                services,
                Actor,
                Character,
                Memory,
                WorldState,
                worldStateUpdater,
                cancellationToken);
        }
    }
}
