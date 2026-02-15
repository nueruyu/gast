using Gast.Core.Events;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Domain.Combat
{
    public readonly struct CharacterDamagedEvent : IDomainEvent
    {
        public ICharacter HitCharacter { get; }
        public AttackInfo AttackInfo { get; }
        public Pose HitPoint { get; }

        public CharacterDamagedEvent(ICharacter hitCharacter, AttackInfo attackInfo, Pose hitPoint)
        {
            HitCharacter = hitCharacter;
            AttackInfo = attackInfo;
            HitPoint = hitPoint;
        }
    }
}
