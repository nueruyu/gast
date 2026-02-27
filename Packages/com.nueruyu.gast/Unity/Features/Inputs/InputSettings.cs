using UnityEngine;
using UnityEngine.InputSystem;

namespace Gast.Features.Inputs
{
    /// <summary>
    /// Settings container for Input System configuration.
    /// Holds reference to the InputActionAsset.
    /// </summary>
    [CreateAssetMenu(menuName = "Gast/Inputs/Settings")]
    public class InputSettings : ScriptableObject
    {
        [SerializeField]
        InputActionAsset inputActions;

        public InputActionAsset InputActions => inputActions;
    }
}