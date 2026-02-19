using Gast.Core.Events;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Domain.Combat
{
    public readonly struct CharacterHitEvent : IDomainEvent
    {
        public ICharacter HitCharacter { get; }
        public object Context { get; }
        public Pose HitPoint { get; }

        public CharacterHitEvent(ICharacter hitCharacter, Pose hitPoint, object context)
        {
            HitCharacter = hitCharacter;
            Context = context;
            HitPoint = hitPoint;
        }
    }
}
