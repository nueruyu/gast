using Gast.Features.Combat;
using UnityEngine;

namespace Gast.Features.Characters.Actions
{
    [CreateAssetMenu(fileName = "HitActionSettings", menuName = "Gast/Actions/Hit Action Settings")]
    public class HitActionSettings : CharacterActionSettings
    {
        [SerializeField]
        float duration = 0.5f;

        [SerializeField]
        float knockbackFriction = 5f;

        public float Duration => duration;
        public float KnockbackFriction => knockbackFriction;

        public override ICharacterAction CreateAction(CharacterContext context, ICombatMethod combatMethod)
        {
            return new HitAction(context, this);
        }
    }
}
