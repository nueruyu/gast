using Gast.Features.Characters;
using Gast.Features.Combat;
using UnityEngine;

namespace GastGame.Actions
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