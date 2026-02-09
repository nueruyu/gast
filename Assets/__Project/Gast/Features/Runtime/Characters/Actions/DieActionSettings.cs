using Gast.Features.Combat;
using UnityEngine;

namespace Gast.Features.Characters.Actions
{
    [CreateAssetMenu(fileName = "DieActionSettings", menuName = "Gast/Actions/Die Action Settings")]
    public class DieActionSettings : CharacterActionSettings
    {
        public override ICharacterAction CreateAction(CharacterContext context)
        {
            return new DieAction(context);
        }
    }
}