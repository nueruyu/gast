using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Grapple
{
    [CreateAssetMenu(fileName = "GrappleThrowActionSettings", menuName = "Gast/Actions/Grapple Throw Action Settings")]
    public class GrappleThrowActionSettings : CharacterActionSettings
    {
        [SerializeField]
        float duration = 1.5f;

        public float Duration => duration;
    }
}
