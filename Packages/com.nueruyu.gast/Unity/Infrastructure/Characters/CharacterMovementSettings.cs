using UnityEngine;

namespace Gast.Unity.Infrastructure.Characters
{
    [CreateAssetMenu(fileName = "CharacterMovementSettings", menuName = "Gast/Characters/Settings/Movement Settings")]
    public class CharacterMovementSettings : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField]
        float walkSpeed = 4f;

        [SerializeField]
        float sprintSpeed = 7f;

        public float WalkSpeed => walkSpeed;
        public float SprintSpeed => sprintSpeed;
    }
}
