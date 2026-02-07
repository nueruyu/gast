using Gast.Features.Combat;
using UnityEngine;

namespace Gast.Features.Characters.Actions
{
    [CreateAssetMenu(fileName = "AttackActionSettings", menuName = "Gast/Actions/Attack Action Settings")]
    public class AttackActionSettings : CharacterActionSettings
    {
        [SerializeField]
        float cooldown = 1f;

        public float Cooldown => cooldown;

        public override ICharacterAction CreateAction(CharacterContext context, ICombatMethod combatMethod)
        {
            return new AttackAction(context, combatMethod, this);
        }
    }
}
