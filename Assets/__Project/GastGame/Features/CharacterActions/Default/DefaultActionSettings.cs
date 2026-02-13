using Gast.Features.Characters;
using GastGame.Features.Characters;
using UnityEngine;

namespace GastGame.Features.CharacterActions.Default
{
    [CreateAssetMenu(fileName = "DefaultActionSettings", menuName = "Gast/Actions/Default Action Settings")]
    public class DefaultActionSettings : CharacterActionSettings
    {
        public override ICharacterAction CreateAction(CharacterContext context)
        {
            // This is a placeholder. The actual creation with the correct context happens in GameCharacterFactory.
            // This method won't be called directly.
            return null;
        }
    }
}
