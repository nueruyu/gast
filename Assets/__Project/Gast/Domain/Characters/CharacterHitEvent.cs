using Gast.Core.Events;
using UnityEngine;

namespace Gast.Domain.Characters
{
    public readonly struct CharacterHitEvent<TContext> : IDomainEvent
    {
        public ICharacter HitCharacter { get; }
        public Pose HitPoint { get; }
        public TContext Context { get; }

        public CharacterHitEvent(ICharacter hitCharacter, Pose hitPoint, TContext context)
        {
            HitCharacter = hitCharacter;
            HitPoint = hitPoint;
            Context = context;
        }
    }
}
