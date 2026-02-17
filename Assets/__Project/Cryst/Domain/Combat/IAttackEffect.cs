using UnityEngine;
using Gast.Domain.Characters;
using Gast.Domain.Combat;

namespace Cryst.Domain.Combat
{
    public interface IAttackEffect : IEffect
    {
        CharacterId SourceCharacterId { get; }
        float Damage { get; }
        Vector3 KnockbackForce { get; }

        bool IEffect.CanApplyTo(ICharacter target)
        {
            return target.Id != SourceCharacterId;
        }
    }
}