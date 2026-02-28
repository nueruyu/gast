using Cryst.Domain.Characters.Commands;
using Cryst.Domain.Combat;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using UnityEngine;

namespace Cryst.Domain.Characters
{
    public class CrystCharacter : ICharacterFacet
    {
        readonly CharacterId id;
        readonly ICharacterTypeDefinition typeDefinition;
        readonly Faction faction;
        readonly ICharacterActionController actionController;
        readonly CharacterStatus status;
        readonly CharacterActionStateStore stateStore;
        readonly ICharacterBody body;
        readonly IVisionSensor visionSensor;
        readonly INavigationProvider navigationProvider;

        public CrystCharacter(
            CharacterId id,
            ICharacterTypeDefinition typeDefinition,
            Faction faction,
            ICharacterActionController actionController,
            CharacterStatus status,
            CharacterActionStateStore stateStore,
            ICharacterBody body,
            IVisionSensor visionSensor,
            INavigationProvider navigationProvider)
        {
            this.id = id;
            this.typeDefinition = typeDefinition;
            this.faction = faction;
            this.actionController = actionController;
            this.status = status;
            this.stateStore = stateStore;
            this.body = body;
            this.visionSensor = visionSensor;
            this.navigationProvider = navigationProvider;
        }

        public CharacterId Id => id;
        public CharacterTypeId TypeId => typeDefinition.TypeId;
        public Faction Faction => faction;
        public ICharacterBody Body => body;
        public IVisionSensor VisionSensor => visionSensor;
        public INavigationProvider NavigationProvider => navigationProvider;

        public CharacterStatus Status => status;

        public bool IsThreatTo(CrystCharacter other)
        {
            if (!Status.IsAlive.Value)
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

        public TakeDamageResult TakeDamage(DamageInfo damageInfo)
        {
            if (!Status.IsAlive.Value)
                return TakeDamageResult.NoDamage;

            Hit(damageInfo);

            Status.SetHealth(Status.Health.Value - damageInfo.Amount);

            if (Status.Health.Value <= 0)
            {
                Die();
                return TakeDamageResult.Defeated;
            }

            return TakeDamageResult.Alive;
        }

        void Hit(DamageInfo damageInfo) => actionController.ExecuteAction(new HitCommand(damageInfo));

        void Die() => actionController.ExecuteAction(new DieCommand());
    }
}