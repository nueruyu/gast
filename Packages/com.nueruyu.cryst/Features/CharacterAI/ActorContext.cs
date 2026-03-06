using System;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Pickups;
using Gast.Lib.AI;
using Cryst.Domain.Characters;

namespace Cryst.Features.CharacterAI
{
    public class ActorContext<TWorldState> : IActorContext<TWorldState>
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

        public ActorContext(
            AIBrainServices services,
            BaseCharacter actor,
            ICharacter character,
            AIMemory memory,
            TWorldState worldState,
            Action worldStateUpdater)
        {
            this.services = services;
            Actor = actor;
            Character = character;
            Memory = memory;
            WorldState = worldState;
            this.worldStateUpdater = worldStateUpdater;
        }

        public void UpdateWorldState()
        {
            worldStateUpdater?.Invoke();
        }
    }
}
