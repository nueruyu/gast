using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Grapple
{
    [CreateAssetMenu(fileName = "GrappledActionSettings", menuName = "Gast/Actions/Grappled Action Settings")]
    public class GrappledActionSettings : CharacterActionSettings
    {
        [SerializeField]
        float duration = 1.5f;

        public float Duration => duration;
    }
}
