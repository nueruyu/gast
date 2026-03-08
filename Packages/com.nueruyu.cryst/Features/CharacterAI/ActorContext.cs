using System;
using System.Collections.Generic;
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
        readonly Dictionary<Type, object> modules = new();

        public ActorContext(
            AIBrainServices services,
            BaseCharacter actor,
            ICharacter character,
            TWorldState worldState,
            Action<ActorContext<TWorldState>> worldStateUpdater)
        {
            this.services = services;
            Actor = actor;
            Character = character;
            WorldState = worldState;
            this.worldStateUpdater = worldStateUpdater;
        }

        public BaseCharacter Actor { get; }
        public ICharacter Character { get; }
        public ICharacterRepository CharacterRepository => services.CharacterRepository;
        public IPickupRepository PickupRepository => services.PickupRepository;
        public ICommandDispatcher CommandDispatcher => services.CommandDispatcher;
        public ObjectiveManager ObjectiveManager => services.ObjectiveManager;

        public TWorldState WorldState { get; }

        public void UpdateWorldState()
        {
            worldStateUpdater.Invoke(this);
        }

        public void RegisterModule<T>(T module) where T : class
        {
            modules[typeof(T)] = module;
        }

        public T GetModule<T>() where T : class
        {
            if (modules.TryGetValue(typeof(T), out var module))
            {
                return (T)module;
            }
            throw new KeyNotFoundException($"Module of type '{typeof(T).Name}' not found in ActorContext.");
        }
    }
}