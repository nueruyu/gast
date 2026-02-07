using Gast.Features.Combat;
using UnityEngine;

namespace Gast.Features.Characters.Actions
{
    [CreateAssetMenu(fileName = "HitActionSettings", menuName = "Gast/Actions/Hit Action Settings")]
    public class HitActionSettings : CharacterActionSettings
    {
        public override ICharacterAction CreateAction(CharacterContext context, ICombatMethod combatMethod)
        {
            return new HitAction(context);
        }
    }
}
