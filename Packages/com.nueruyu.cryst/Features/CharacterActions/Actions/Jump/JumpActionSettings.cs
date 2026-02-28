using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Jump
{
    [CreateAssetMenu(fileName = "JumpActionSettings", menuName = "Gast/Actions/Jump Action Settings")]
    public class JumpActionSettings : CharacterActionSettings
    {
        [SerializeField]
        float force = 5f;

        [SerializeField]
        float lookDirectionSpeed = 10f;

        public float Force => force;
        public float LookDirectionSpeed => lookDirectionSpeed;
    }
}