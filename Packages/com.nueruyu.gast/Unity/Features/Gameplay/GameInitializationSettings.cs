using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Unity.Features.Gameplay
{
    /// <summary>
    /// Settings for game initialization and startup configuration.
    /// Defines default player character type and other initialization parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "GameInitializationSettings", menuName = "Gast/Game/Initialization Settings")]
    public class GameInitializationSettings : ScriptableObject
    {
        [SerializeField]
        ScriptableObject playerCreationParameters;

        public ICharacterCreationParameters PlayerCreationParameters => playerCreationParameters as ICharacterCreationParameters;
    }
}
