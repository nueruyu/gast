using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Domain.Combat
{
    public readonly struct AttackInfo
    {
        public CharacterId AttackerId { get; }
        public float Damage { get; }
        public Vector3 KnockbackForce { get; }

        public AttackInfo(CharacterId attackerId, float damage, Vector3 knockbackForce)
        {
            AttackerId = attackerId;
            Damage = damage;
            KnockbackForce = knockbackForce;
        }
    }
}
