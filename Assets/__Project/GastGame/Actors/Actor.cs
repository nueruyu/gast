using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using Gast.Domain.Sensors;
using Gast.Domain.Stats;
using GastGame.Actions.Commands;
using UnityEngine;

namespace GastGame.Actors
{
    public readonly struct Actor
    {
        static readonly StatId HealthStatId = StatId.FromString("Health");
        static readonly StatId MaxHealthStatId = StatId.FromString("MaxHealth");

        public ICharacter Character { get; }

        public Actor(ICharacter character)
        {
            Character = character;
        }

        public CharacterId Id => Character.Id;
        public CharacterTypeId TypeId => Character.TypeId;
        public Faction Faction => Character.Faction;
        public ICharacterBody Body => Character.Body;
        public IVisionSensor VisionSensor => Character.VisionSensor;
        public IInteractionSensor InteractionSensor => Character.InteractionSensor;
        public INavigationProvider NavigationProvider => Character.NavigationProvider;

        public float Health
        {
            get
            {
                Character.Status.TryGetStatValue(HealthStatId, out var value);
                return value;
            }
        }

        public float MaxHealth
        {
            get
            {
                Character.Status.TryGetStatValue(MaxHealthStatId, out var value);
                return value;
            }
        }

        public bool IsAlive => Health > 0;

        public bool IsThreatTo(Actor other)
        {
            if (!IsAlive)
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