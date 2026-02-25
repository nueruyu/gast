using Gast.Domain.Characters;
using UnityEngine;

namespace Cryst.Domain.Combat
{
    public readonly struct AttackInfo
    {
        public CharacterId SourceCharacterId { get; }
        public float Damage { get; }
        public Vector3 KnockbackForce { get; }

        public AttackInfo(CharacterId sourceCharacterId, float damage, Vector3 knockbackForce)
        {
            SourceCharacterId = sourceCharacterId;
            Damage = damage;
            KnockbackForce = knockbackForce;
        }
    }
}
