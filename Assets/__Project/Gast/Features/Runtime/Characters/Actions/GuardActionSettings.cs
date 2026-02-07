using Gast.Features.Combat;
using UnityEngine;

namespace Gast.Features.Characters.Actions
{
    [CreateAssetMenu(fileName = "GuardActionSettings", menuName = "Gast/Actions/Guard Action Settings")]
    public class GuardActionSettings : CharacterActionSettings
    {
        public override ICharacterAction CreateAction(CharacterContext context, ICombatMethod combatMethod)
        {
            return new GuardAction(context);
        }
    }
}
