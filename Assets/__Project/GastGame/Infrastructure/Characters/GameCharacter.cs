using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using Gast.Domain.Sensors;
using Gast.Domain.Stats;
using GastGame.Domain.Characters;
using GastGame.Features.CharacterActions;
using R3;
using System;
using UnityEngine;

namespace GastGame.Infrastructure.Characters
{
    public class GameCharacter : IGameCharacter
    {
        readonly ICharacter character;

        public CharacterId Id => character.Id;
        public CharacterTypeId TypeId => character.TypeId;
        public Faction Faction => character.Faction;
        public ICharacterBody Body => character.Body;
        public IVisionSensor VisionSensor => character.VisionSensor;
        public IInteractionSensor InteractionSensor => character.InteractionSensor;
        public INavigationProvider NavigationProvider => character.NavigationProvider;

        public ILive<bool> IsAlive { get; }
        public ILive<float> Health { get; }
        public ILive<float> MaxHealth { get; }

        public GameCharacter(ICharacter character)
        {
            this.character = character;

            var healthStatId = StatId.FromString("Health");
            var maxHealthStatId = StatId.FromString("MaxHealth");

            Health = character.Status.GetStat(healthStatId);
            MaxHealth = character.Status.GetStat(maxHealthStatId);
            IsAlive = Health.Select(h => h > 0);
        }

        public bool IsThreatTo(IGameCharacter other)
        {
            if (!IsAlive.Value)
                return false;
            if (Faction == other.Faction)
                return false;
            return true;
        }

        public void Move(Vector3 direction) => character.Move(direction);

        public void SetSprint(bool isSprinting) => character.SetSprint(isSprinting);

        public bool CanAttack() => character.ActionController.CanExecuteAction<AttackCommand>();

        public void Attack() => character.ActionController.ExecuteAction(new AttackCommand());

        public bool CanGuard() => character.ActionController.CanExecuteAction<GuardCommand>();

        public void StartGuard() => character.ActionController.StartAction(new GuardCommand());

        public void StopGuard() => character.ActionController.StopAction<GuardCommand>();

        public void Dash(Vector3 direction) => character.ActionController.ExecuteAction(new DashCommand(direction));

        public void Jump() => character.ActionController.ExecuteAction(new JumpCommand());
    }
}