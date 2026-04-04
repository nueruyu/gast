using Gast.Unity.Features.Characters;
using Gast.Unity.Shared.Animations;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Grapple
{
    [CreateAssetMenu(fileName = "GrappledActionSettings", menuName = "Gast/Actions/Grappled Action Settings")]
    public class GrappledActionSettings : CharacterActionSettings
    {
        [Header("Core")]
        [SerializeField]
        float duration = 1.5f;

        [Header("Animation")]
        [SerializeField]
        AnimatorTriggerSymbol grappledTrigger;

        public float Duration => duration;
        public AnimatorTriggerSymbol GrappledTrigger => grappledTrigger;
    }
}
