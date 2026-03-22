using System;
using System.Collections.Generic;
using Cryst.Domain.Characters;
using Cryst.Features.CharacterAI.Humanoid.Objective;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Pickups;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI
{
    public class ActorContext<TWorldState> : IActorContext<TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
    {
        readonly Dictionary<Type, object> modules = new();
        readonly AIBrainServices services;
        readonly Action<ActorContext<TWorldState>> worldStateUpdater;

        public ActorContext(
            AIBrainServices services,
            ObjectiveManager objectiveManager,
            BaseCharacter actor,
            ICharacter character,
            TWorldState worldState,
            Action<ActorContext<TWorldState>> worldStateUpdater)
        {
            this.services = services;
            ObjectiveManager = objectiveManager;
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
        public ObjectiveManager ObjectiveManager { get; }
        public IObjectiveQueries ObjectiveQueries => services.ObjectiveQueries;

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
            if (modules.TryGetValue(typeof(T), out var module)) return (T)module;
            throw new KeyNotFoundException($"Module of type '{typeof(T).Name}' not found in ActorContext.");
        }

        record ActorInfo(
            CharacterId Id,
            CharacterTypeId TypeId,
            Vector3 Position,
            Faction Faction,
            bool IsAlive) : IActorInfo;
    }
}