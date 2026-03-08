using System;
using Cryst.Domain.Characters;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Pickups;
using Gast.Lib.AI;

namespace Cryst.Features.CharacterAI
{
    public class ActorContext<TWorldState> : IActorContext<TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
    {
        readonly AIBrainServices services;
        readonly Action<ActorContext<TWorldState>> worldStateUpdater;

        public ActorContext(
            AIBrainServices services,
            BaseCharacter actor,
            ICharacter character,
            AIMemory memory,
            TWorldState worldState,
            Action<ActorContext<TWorldState>> worldStateUpdater)
        {
            this.services = services;
            Actor = actor;
            Character = character;
            Memory = memory;
            WorldState = worldState;
            this.worldStateUpdater = worldStateUpdater;
        }

        public BaseCharacter Actor { get; }
        public ICharacter Character { get; }
        public AIMemory Memory { get; }
        public ICharacterRepository CharacterRepository => services.CharacterRepository;
        public IPickupRepository PickupRepository => services.PickupRepository;
        public ICommandDispatcher CommandDispatcher => services.CommandDispatcher;
        public ObjectiveManager ObjectiveManager => services.ObjectiveManager;

        public TWorldState WorldState { get; }

        public void UpdateWorldState()
        {
            worldStateUpdater.Invoke(this);
        }
    }
}