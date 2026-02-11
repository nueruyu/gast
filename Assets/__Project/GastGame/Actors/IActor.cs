using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using Gast.Domain.Sensors;
using UnityEngine;

namespace GastGame.Actors
{
    public interface IActor : ICharacterAspect
    {
        ICharacter Character { get; }
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

        bool IsThreatTo(IActor other);

        void Move(Vector3 direction);

        void SetSprint(bool isSprinting);

        bool CanAttack();

        void Attack();

        bool CanGuard();

        void StartGuard();

        void StopGuard();

        void Dash(Vector3 direction);

        void Jump();
    }
}