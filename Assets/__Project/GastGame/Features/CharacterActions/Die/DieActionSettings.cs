using Gast.Features.Characters;
using GastGame.Features.Characters;
using UnityEngine;

namespace GastGame.Features.CharacterActions
{
    [CreateAssetMenu(fileName = "DieActionSettings", menuName = "Gast/Actions/Die Action Settings")]
    public class DieActionSettings : CharacterActionSettings
    {
        public override ICharacterAction CreateAction(CharacterContext context)
        {
            var actionContext = CharacterActionContextFactory.Create(context);
            return new DieAction(actionContext);
        }
    }
}