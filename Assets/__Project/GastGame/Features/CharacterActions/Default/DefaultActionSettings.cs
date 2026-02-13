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
            var actionContext = CharacterActionContextFactory.Create(context);
            return new DefaultAction(actionContext);
        }
    }
}