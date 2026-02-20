using Gast.Core.Events;
using UnityEngine;

namespace Gast.Domain.Characters
{
    public readonly struct CharacterHitEvent : IDomainEvent
    {
        public ICharacter HitCharacter { get; }
        public Pose HitPoint { get; }
        public object Context { get; }

        public CharacterHitEvent(ICharacter hitCharacter, Pose hitPoint, object context)
        {
            HitCharacter = hitCharacter;
            Context = context;
            HitPoint = hitPoint;
        }
    }
}