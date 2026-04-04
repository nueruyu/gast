using Gast.Unity.Features.Characters;
using Gast.Unity.Shared.Animations;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Grapple
{
    [CreateAssetMenu(fileName = "GrappleThrowActionSettings", menuName = "Gast/Actions/Grapple Throw Action Settings")]
    public class GrappleThrowActionSettings : CharacterActionSettings
    {
        [Header("Core")]
        [SerializeField]
        float duration = 1.5f;

        [Header("Animation")]
        [SerializeField]
        AnimatorTriggerSymbol throwTrigger;

        public float Duration => duration;
        public AnimatorTriggerSymbol ThrowTrigger => throwTrigger;
    }
}
