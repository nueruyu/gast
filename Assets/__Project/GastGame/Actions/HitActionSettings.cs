using Gast.Features.Characters;
using Gast.Features.Combat;
using UnityEngine;

namespace GastGame.Actions
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

        public override ICharacterAction CreateAction(CharacterContext context)
        {
            return new HitAction(context, this);
        }
    }
}