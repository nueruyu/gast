using System.Collections.Generic;
using UnityEngine;

namespace Gast.Infrastructure.Settings
{
    /// <summary>
    /// Settings container for character type definitions.
    /// Holds references to all CharacterTypeDefinitionSO assets.
    /// </summary>
    [CreateAssetMenu(menuName = "DescrioGames/Characters/Database Settings")]
    public class CharacterDatabaseSettings : ScriptableObject
    {
        [SerializeField]
        List<CharacterTypeDefinition> definitions = new();

        public IReadOnlyList<CharacterTypeDefinition> Definitions => definitions;
    }
}