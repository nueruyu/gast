using Gast.Core.Events;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Domain.Combat
{
    public readonly struct CharacterHitEvent : IDomainEvent
    {
        public ICharacter HitCharacter { get; }
        public IEffect Effect { get; }
        public Pose HitPoint { get; }

        public CharacterHitEvent(ICharacter hitCharacter, Pose hitPoint, IEffect effect)
        {
            HitCharacter = hitCharacter;
            Effect = effect;
            HitPoint = hitPoint;
        }
    }
}