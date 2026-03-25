using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Unity.Features.Gameplay
{
    [CreateAssetMenu(fileName = "GameInitializationSettings", menuName = "Gast/Game/Initialization Settings")]
    public class GameInitializationSettings : ScriptableObject
    {
        [SerializeField]
        PlayerCreationSettings playerCreationParameters;

        public ICharacterCreationParameters PlayerCreationParameters =>
            playerCreationParameters.Create();
    }
}
