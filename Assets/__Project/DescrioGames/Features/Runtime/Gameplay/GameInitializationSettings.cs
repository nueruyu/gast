using DescrioGames.Domain.Characters;
using DescrioGames.Features.Characters;
using UnityEngine;

namespace DescrioGames.Features.Gameplay
{
    /// <summary>
    /// Settings for game initialization and startup configuration.
    /// Defines default player character type and other initialization parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "GameInitializationSettings", menuName = "DescrioGames/Game/Initialization Settings")]
    public class GameInitializationSettings : ScriptableObject
    {
        [SerializeField]
        CharacterTypeReference playerCharacterTypeReference;

        public CharacterTypeId PlayerCharacterTypeId => playerCharacterTypeReference.Id;
    }
}