using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using Gast.Domain.Sensors;
using Gast.Domain.Stats;
using Gast.Shared.Observables;
using GastGame.Actions.Commands;
using R3;
using System;
using UnityEngine;

namespace GastGame.Actors
{
    public class Actor : IActor
    {
        public ICharacter Character { get; }

        public CharacterId Id => Character.Id;
        public CharacterTypeId TypeId => Character.TypeId;
        public Faction Faction => Character.Faction;
        public ICharacterBody Body => Character.Body;
        public IVisionSensor VisionSensor => Character.VisionSensor;
        public IInteractionSensor InteractionSensor => Character.InteractionSensor;
        public INavigationProvider NavigationProvider => Character.NavigationProvider;

        public ILive<bool> IsAlive { get; }
        public ILive<float> Health { get; }
        public ILive<float> MaxHealth { get; }

        public Actor(ICharacter character)
        {
            Character = character;

            var healthStatId = StatId.FromString("Health");
            var maxHealthStatId = StatId.FromString("MaxHealth");

            Health = character.Status.GetStat(healthStatId);
            MaxHealth = character.Status.GetStat(maxHealthStatId);
            IsAlive = Health.Select(h => h > 0);
        }

        public bool IsThreatTo(IActor other)
        {
            if (!IsAlive.Value)
                return false;
            if (Character.Faction == other.Character.Faction)
                return false;
            return true;
        }

        public void Move(Vector3 direction) => Character.Move(direction);

        public void SetSprint(bool isSprinting) => Character.SetSprint(isSprinting);

        public bool CanAttack() => Character.ActionController.CanExecuteAction<AttackCommand>();

        public void Attack() => Character.ActionController.ExecuteAction(new AttackCommand());

        public bool CanGuard() => Character.ActionController.CanExecuteAction<GuardCommand>();

        public void StartGuard() => Character.ActionController.StartAction(new GuardCommand());

        public void StopGuard() => Character.ActionController.StopAction<GuardCommand>();

        public void Dash(Vector3 direction) => Character.ActionController.ExecuteAction(new DashCommand(direction));

        public void Jump() => Character.ActionController.ExecuteAction(new JumpCommand());
    }
}