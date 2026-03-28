using Cryst.Domain.Characters.Commands;
using Cryst.Domain.Combat;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using UnityEngine;

namespace Cryst.Domain.Characters
{
    public class BaseCharacter : ICharacterFacet
    {
        readonly ICharacterTypeDefinition typeDefinition;
        readonly ICharacterActionController actionController;

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
            Id = id;
            this.typeDefinition = typeDefinition;
            Faction = faction;
            this.actionController = actionController;
            Status = status;
            Body = body;
            VisionSensor = visionSensor;
            NavigationProvider = navigationProvider;
        }

        public CharacterId Id { get; }

        public CharacterTypeId TypeId => typeDefinition.TypeId;
        public Faction Faction { get; }

        public ICharacterBody Body { get; }

        public IVisionSensor VisionSensor { get; }

        public INavigationProvider NavigationProvider { get; }

        public CharacterStatus Status { get; }

        public bool IsThreatTo(BaseCharacter other)
        {
            if (!Status.IsAlive.Value)
                return false;
            
            return Faction != other.Faction;
        }

        public void Move(Vector3 direction) => actionController.Move(direction);

        public CharacterDamageResult TakeDamage(DamageInfo damageInfo)
        {
            if (!Status.IsAlive.Value)
                return CharacterDamageResult.NoDamage;

            actionController.ExecuteAction(new HitCommand(damageInfo));

            Status.SetHealth(Status.Health.Value - damageInfo.Amount);

            return Status.Health.Value <= 0 ?
                CharacterDamageResult.Defeated :
                CharacterDamageResult.Alive;
        }

        public CharacterDamageResult ApplyPassiveDamage(float amount)
        {
            if (!Status.IsAlive.Value)
                return CharacterDamageResult.NoDamage;

            Status.SetHealth(Status.Health.Value - amount);

            return Status.Health.Value <= 0 ?
                CharacterDamageResult.Defeated :
                CharacterDamageResult.Alive;
        }

        public void Die() => actionController.ExecuteAction(new DieCommand());
    }
}