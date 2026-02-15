using Gast.Features.Characters;
using UnityEngine;

namespace Cryst.Modules.CharacterActions.Default
{
    [CreateAssetMenu(fileName = "DefaultActionSettings", menuName = "Gast/Actions/Default Action Settings")]
    public class DefaultActionSettings : CharacterActionSettings
    {
        public override ICharacterAction CreateAction(CharacterContext context)
        {
            return new DefaultAction(context);
        }
    }
}
