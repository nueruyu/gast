using System.Collections.Generic;
using UnityEngine;

namespace Gast.Unity.Infrastructure.Characters
{
    /// <summary>
    /// Settings container for character type definitions.
    /// Holds references to all CharacterTypeDefinitionSO assets.
    /// </summary>
    [CreateAssetMenu(menuName = "Gast/Characters/Database Settings")]
    public class CharacterDatabaseSettings : ScriptableObject
    {
        [SerializeField]
        List<CharacterTypeDefinition> definitions = new();

        public IReadOnlyList<CharacterTypeDefinition> Definitions => definitions;
    }
}