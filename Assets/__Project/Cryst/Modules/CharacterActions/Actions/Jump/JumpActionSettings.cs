using Gast.Features.Characters;
using UnityEngine;

namespace Cryst.Modules.CharacterActions
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