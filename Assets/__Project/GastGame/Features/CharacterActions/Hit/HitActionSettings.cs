using Gast.Features.Characters;
using GastGame.Features.Characters;
using UnityEngine;

namespace GastGame.Features.CharacterActions
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
            var actionContext = CharacterActionContextFactory.Create(context);
            return new HitAction(actionContext, this);
        }
    }
}