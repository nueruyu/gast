using DescrioGames.Domain.Characters;
using DescrioGames.Features.Characters;
using UnityEngine;

namespace DescrioGames.Features.Combat
{
    public readonly struct DamageHitInfo
    {
        public DamageHitInfo(CharacterId attackerId, Character character, Pose point)
        {
            AttackerId = attackerId;
            Character = character;
            Point = point;
        }

        public CharacterId AttackerId { get; }
        public Character Character { get; }
        public Pose Point { get; }
    }
}