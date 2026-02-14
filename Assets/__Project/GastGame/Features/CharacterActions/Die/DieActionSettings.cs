using Gast.Features.Characters;
using UnityEngine;

namespace GastGame.Features.CharacterActions
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
