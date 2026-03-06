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
        public BaseCharacter Actor { get; }
        public ICharacter Character { get; }
        public AIMemory Memory { get; }
        public ICharacterRepository CharacterRepository { get; }
        public IPickupRepository PickupRepository { get; }
        public ICommandDispatcher CommandDispatcher { get; }
        public TWorldState WorldState { get; }
        public CancellationToken CancellationToken { get; }
        public ContextKey ContextKey { get; }

        readonly Action worldStateUpdater;

        public AIContext(
            ContextKey contextKey,
            BaseCharacter actor,
            ICharacter character,
            AIMemory memory,
            ICharacterRepository characterRepository,
            IPickupRepository pickupRepository,
            ICommandDispatcher commandDispatcher,
            TWorldState worldState,
            Action worldStateUpdater,
            CancellationToken cancellationToken)
        {
            ContextKey = contextKey;
            Actor = actor;
            Character = character;
            Memory = memory;
            CharacterRepository = characterRepository;
            PickupRepository = pickupRepository;
            CommandDispatcher = commandDispatcher;
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
                Actor,
                Character,
                Memory,
                CharacterRepository,
                PickupRepository,
                CommandDispatcher,
                WorldState,
                worldStateUpdater,
                cancellationToken);
        }
    }
}
