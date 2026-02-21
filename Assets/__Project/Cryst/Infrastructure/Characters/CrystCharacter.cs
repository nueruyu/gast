using Cryst.Domain.Characters;
using Cryst.Domain.Combat;
using Cryst.Modules.CharacterActions;
using Gast.Core.Events;
using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Loot;
using R3;
using UnityEngine;

namespace Cryst.Infrastructure.Characters
{
    public class CrystCharacter : ICrystCharacter
    {
        readonly ICharacter character;
        readonly CharacterActionStateStore stateStore;
        readonly IDomainEventPublisher eventPublisher;
        readonly CharacterStatus status;

        public CrystCharacter(ICharacter character, IDomainEventPublisher eventPublisher)
        {
            this.character = character;
            stateStore = character.Resolve<CharacterActionStateStore>();

            this.eventPublisher = eventPublisher;

            status = character.Resolve<CharacterStatus>();
            IsAlive = status.Health.Select(h => h > 0);
        }

        public ICharacter Character => character;
        public CharacterId Id => character.Id;
        public CharacterTypeId TypeId => character.TypeId;
        public Faction Faction => character.Faction;
        public ICharacterBody Body => character.Resolve<ICharacterBody>();
        public IVisionSensor VisionSensor => character.Resolve<IVisionSensor>();
        public INavigationProvider NavigationProvider => character.Resolve<INavigationProvider>();

        public ILive<bool> IsAlive { get; }
        public ILive<float> Health => status.Health;
        public ILive<float> MaxHealth => status.MaxHealth;

        public void SetHealth(float newHealth)
        {
            status.SetHealth(newHealth);
        }

        public bool IsThreatTo(ICrystCharacter other)
        {
            if (!IsAlive.Value)
                return false;
            if (Faction == other.Faction)
                return false;
            return true;
        }

        public void Move(Vector3 direction) => character.ActionController.Move(direction);

        public void SetSprint(bool isSprinting) => stateStore.IsSprinting = isSprinting;

        public bool CanAttack() => character.ActionController.CanExecuteAction<AttackCommand>();

        public void Attack() => character.ActionController.ExecuteAction(new AttackCommand());

        public bool CanGuard() => character.ActionController.CanExecuteAction<GuardCommand>();

        public void StartGuard() => character.ActionController.StartAction(new GuardCommand());

        public void StopGuard() => character.ActionController.StopAction<GuardCommand>();

        public void Dash(Vector3 direction) => character.ActionController.ExecuteAction(new DashCommand(direction));

        public void Jump() => character.ActionController.ExecuteAction(new JumpCommand());

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (!IsAlive.Value)
                return;

            character.ActionController.ExecuteAction(new HitCommand(damageInfo));

            SetHealth(Health.Value - damageInfo.Amount);

            if (Health.Value <= 0)
            {
                Die();

                eventPublisher.Publish(new CharacterDefeatedEvent(this, damageInfo.AttackerId));
                eventPublisher.Publish(
                    new LootPotentialDropEvent(
                        character.TypeDefinition.LootTable,
                        Body.Position));
            }
        }

        void Die()
        {
            character.ActionController.ExecuteAction(new DieCommand());
            character.DetachBrain();
        }
    }
}