using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Cryst.Domain.Combat;
using UnityEngine;

namespace Cryst.Domain.Characters
{
    public interface ICrystCharacter : ICharacterFacet
    {
        CharacterId Id { get; }
        CharacterTypeId TypeId { get; }
        Faction Faction { get; }
        ICharacterBody Body { get; }
        IVisionSensor VisionSensor { get; }
        INavigationProvider NavigationProvider { get; }

        ILive<bool> IsAlive { get; }
        ILive<float> Health { get; }
        ILive<float> MaxHealth { get; }

        void SetHealth(float newHealth);

        bool IsThreatTo(ICrystCharacter other);

        void Move(Vector3 direction);

        void SetSprint(bool isSprinting);

        bool CanAttack();

        void Attack();

        bool CanGuard();

        void StartGuard();

        void StopGuard();

        void Dash(Vector3 direction);

        void Jump();

        TakeDamageResult TakeDamage(DamageInfo damageInfo);
    }
}