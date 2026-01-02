using UnityEngine;
using UnityEngine.InputSystem;

namespace DescrioGames.Features.Inputs
{
    /// <summary>
    /// Settings container for Input System configuration.
    /// Holds reference to the InputActionAsset.
    /// </summary>
    [CreateAssetMenu(menuName = "DescrioGames/Inputs/Settings")]
    public class InputSettings : ScriptableObject
    {
        [SerializeField]
        InputActionAsset inputActions;

        public InputActionAsset InputActions => inputActions;
    }
}