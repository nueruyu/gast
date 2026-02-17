using Gast.Domain.Characters;
using UnityEngine;

namespace Cryst.Domain.Combat
{
    public interface IAttackEffectFactory
    {
        IAttackEffect Create(CharacterId sourceCharacterId, float damage, Vector3 knockbackForce);
    }
}