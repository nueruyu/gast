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
        readonly CharacterId id;
        readonly ICharacterTypeDefinition typeDefinition;
        readonly Faction faction;
        readonly ICharacterActionController actionController;
        readonly CharacterStatus status;
        readonly CharacterActionStateStore stateStore;
        readonly ICharacterBody body;
        readonly IVisionSensor visionSensor;
        readonly INavigationProvider navigationProvider;
        readonly IDomainEventPublisher eventPublisher;
        readonly ICharacterBrainManager brainManager;

        public CrystCharacter(
            ICharacter character,
            CharacterId id,
            ICharacterTypeDefinition typeDefinition,
            Faction faction,
            ICharacterActionController actionController,
            CharacterStatus status,
            CharacterActionStateStore stateStore,
            ICharacterBody body,
            IVisionSensor visionSensor,
            INavigationProvider navigationProvider,
            IDomainEventPublisher eventPublisher,
            ICharacterBrainManager brainManager)
        {
            this.character = character;
            this.id = id;
            this.typeDefinition = typeDefinition;
            this.faction = faction;
            this.actionController = actionController;
            this.status = status;
            this.stateStore = stateStore;
            this.body = body;
            this.visionSensor = visionSensor;
            this.navigationProvider = navigationProvider;
            this.eventPublisher = eventPublisher;
            this.brainManager = brainManager;

            IsAlive = status.Health.Select(h => h > 0);
        }

        public CharacterId Id => id;
        public CharacterTypeId TypeId => typeDefinition.TypeId;
        public Faction Faction => faction;
        public ICharacterBody Body => body;
        public IVisionSensor VisionSensor => visionSensor;
        public INavigationProvider NavigationProvider => navigationProvider;

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

        public void Move(Vector3 direction) => actionController.Move(direction);

        public void SetSprint(bool isSprinting) => stateStore.IsSprinting = isSprinting;

        public bool CanAttack() => actionController.CanExecuteAction<AttackCommand>();

        public void Attack() => actionController.ExecuteAction(new AttackCommand());

        public bool CanGuard() => actionController.CanExecuteAction<GuardCommand>();

        public void StartGuard() => actionController.StartAction(new GuardCommand());

        public void StopGuard() => actionController.StopAction<GuardCommand>();

        public void Dash(Vector3 direction) => actionController.ExecuteAction(new DashCommand(direction));

        public void Jump() => actionController.ExecuteAction(new JumpCommand());

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (!IsAlive.Value)
                return;

            Hit(damageInfo);

            SetHealth(Health.Value - damageInfo.Amount);

            if (Health.Value <= 0)
            {
                Die();

                brainManager.DetachBrain(Id);

                eventPublisher.Publish(new CharacterDefeatedEvent(character, damageInfo.AttackerId));
                eventPublisher.Publish(
                    new LootPotentialDropEvent(
                        typeDefinition.LootTable,
                        Body.Position));
            }
        }

        void Hit(DamageInfo damageInfo) => actionController.ExecuteAction(new HitCommand(damageInfo));

        void Die() => actionController.ExecuteAction(new DieCommand());
    }
}