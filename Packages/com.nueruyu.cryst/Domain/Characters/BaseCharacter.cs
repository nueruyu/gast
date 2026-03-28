using Cryst.Domain.Characters.Commands;
using Cryst.Domain.Combat;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using UnityEngine;

namespace Cryst.Domain.Characters
{
    public class BaseCharacter : ICharacterFacet
    {
        readonly CharacterId id;
        readonly ICharacterTypeDefinition typeDefinition;
        readonly Faction faction;
        readonly ICharacterActionController actionController;
        readonly CharacterStatus status;
        readonly ICharacterBody body;
        readonly IVisionSensor visionSensor;
        readonly INavigationProvider navigationProvider;

        public BaseCharacter(
            CharacterId id,
            ICharacterTypeDefinition typeDefinition,
            Faction faction,
            ICharacterActionController actionController,
            CharacterStatus status,
            ICharacterBody body,
            IVisionSensor visionSensor,
            INavigationProvider navigationProvider)
        {
            this.id = id;
            this.typeDefinition = typeDefinition;
            this.faction = faction;
            this.actionController = actionController;
            this.status = status;
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

        public bool IsThreatTo(BaseCharacter other)
        {
            if (!Status.IsAlive.Value)
                return false;
            if (Faction == other.Faction)
                return false;
            return true;
        }

        public void Move(Vector3 direction) => actionController.Move(direction);

        public TakeDamageResult TakeDamage(DamageInfo damageInfo)
        {
            if (!Status.IsAlive.Value)
                return TakeDamageResult.NoDamage;

            Hit(damageInfo);

            Status.SetHealth(Status.Health.Value - damageInfo.Amount);

            if (Status.Health.Value <= 0)
            {
                return TakeDamageResult.Defeated;
            }

            return TakeDamageResult.Alive;
        }

        public bool ApplyPassiveDamage(float amount)
        {
            if (!Status.IsAlive.Value) return false;

            Status.SetHealth(Status.Health.Value - amount);

            return Status.Health.Value <= 0;
        }

        void Hit(DamageInfo damageInfo) => actionController.ExecuteAction(new HitCommand(damageInfo));

        public void Die() => actionController.ExecuteAction(new DieCommand());
    }
}
