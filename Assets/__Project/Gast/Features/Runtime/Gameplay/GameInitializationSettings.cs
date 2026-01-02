using Gast.Domain.Characters;
using Gast.Features.Characters;
using UnityEngine;

namespace Gast.Features.Gameplay
{
    /// <summary>
    /// Settings for game initialization and startup configuration.
    /// Defines default player character type and other initialization parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "GameInitializationSettings", menuName = "Gast/Game/Initialization Settings")]
    public class GameInitializationSettings : ScriptableObject
    {
        [SerializeField]
        CharacterTypeReference playerCharacterTypeReference;

        public CharacterTypeId PlayerCharacterTypeId => playerCharacterTypeReference.Id;
    }
}