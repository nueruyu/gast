using Gast.Features.Combat;
using UnityEngine;

namespace Gast.Features.Characters.Actions
{
    [CreateAssetMenu(fileName = "AttackActionSettings", menuName = "Gast/Actions/Attack Action Settings")]
    public class AttackActionSettings : CharacterActionSettings
    {
        [SerializeField]
        float cooldown = 1f;

        [SerializeField]
        float duration = 0.6f;

        public float Cooldown => cooldown;
        public float Duration => duration;

        public override ICharacterAction CreateAction(CharacterContext context, ICombatMethod combatMethod)
        {
            return new AttackAction(context, combatMethod, this);
        }
    }
}