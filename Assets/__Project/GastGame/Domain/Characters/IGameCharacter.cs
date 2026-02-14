using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using GastGame.Domain.Combat;
using UnityEngine;

namespace GastGame.Domain.Characters
{
    public interface IGameCharacter : ICharacterAspect
    {
        CharacterId Id { get; }
        CharacterTypeId TypeId { get; }
        Faction Faction { get; }
        ICharacterBody Body { get; }
        IVisionSensor VisionSensor { get; }
        IInteractionSensor InteractionSensor { get; }
        INavigationProvider NavigationProvider { get; }

        ILive<bool> IsAlive { get; }
        ILive<float> Health { get; }
        ILive<float> MaxHealth { get; }

        void SetHealth(float newHealth);

        bool IsThreatTo(IGameCharacter other);

        void Move(Vector3 direction);

        void SetSprint(bool isSprinting);

        bool CanAttack();

        void Attack();

        bool CanGuard();

        void StartGuard();

        void StopGuard();

        void Dash(Vector3 direction);

        void Jump();

        void Hit(DamageInfo damageInfo);

        void Die();
    }
}