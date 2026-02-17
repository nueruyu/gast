using Cryst.Domain.Combat;
using Gast.Domain.Characters;
using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Cryst.Infrastructure.Effects
{
    class AttackEffectFactory : IAttackEffectFactory
    {
        readonly ObjectPool<AttackEffect> attackEffectPool = new(() => new());

        public IAttackEffect Create(CharacterId sourceCharacterId, float damage, Vector3 knockbackForce)
        {
            var effect = attackEffectPool.Get();
            effect.Pool = attackEffectPool;

            effect.SourceCharacterId = sourceCharacterId;
            effect.Damage = damage;
            effect.KnockbackForce = knockbackForce;

            return effect;
        }

        class AttackEffect : IAttackEffect, IDisposable
        {
            public CharacterId SourceCharacterId { get; set; }
            public float Damage { get; set; }
            public Vector3 KnockbackForce { get; set; }

            public ObjectPool<AttackEffect> Pool { get; set; }

            public void Dispose()
            {
                Pool?.Release(this);
                Pool = null;
            }
        }
    }
}