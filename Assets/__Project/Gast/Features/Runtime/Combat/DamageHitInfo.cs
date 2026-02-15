using Gast.Domain.Characters;
using Gast.Features.Characters;
using UnityEngine;

namespace Gast.Features.Combat
{
    public readonly struct DamageHitInfo
    {
        public DamageHitInfo(CharacterId attackerId, ICharacter character, Pose point)
        {
            AttackerId = attackerId;
            Character = character;
            Point = point;
        }

        public CharacterId AttackerId { get; }
        public ICharacter Character { get; }
        public Pose Point { get; }
    }
}