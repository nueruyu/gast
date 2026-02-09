using Gast.Features.Characters;
using Gast.Features.Combat;
using UnityEngine;

namespace GastGame.Actions
{
    [CreateAssetMenu(fileName = "AttackActionSettings", menuName = "Gast/Actions/Attack Action Settings")]
    public class AttackActionSettings : CharacterActionSettings
    {
        [SerializeField]
        float cooldown = 1f;

        [SerializeField]
        float duration = 0.6f;

        [SerializeField]
        MeleeMethodSettings meleeMethodSettings;

        public float Cooldown => cooldown;
        public float Duration => duration;

        public MeleeMethodSettings MeleeMethodSettings => meleeMethodSettings;

        public override ICharacterAction CreateAction(CharacterContext context)
        {
            return new AttackAction(context, this);
        }
    }
}