using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Domain.Characters;
using Gast.Infrastructure.Settings;
using UnityEngine;

namespace Gast.Infrastructure.Repositories
{
    /// <summary>
    /// Pure C# repository for accessing character type definitions.
    /// Receives CharacterDatabaseSettings via dependency injection.
    /// </summary>
    public class CharacterTypeRepository : ICharacterTypeRepository
    {
        readonly Dictionary<CharacterTypeId, CharacterTypeDefinition> definitionMap;

        public CharacterTypeRepository(CharacterDatabaseSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            definitionMap = settings.Definitions.ToDictionary(
                def => def.TypeId,
                def => def);
        }

        /// <summary>
        /// Get the concrete ScriptableObject for a character type.
        /// Used by CharacterFactory to access prefab and attack settings.
        /// </summary>
        public CharacterTypeDefinition Get(CharacterTypeId id)
        {
            if (definitionMap.TryGetValue(id, out var definition))
                return definition;

            throw new KeyNotFoundException($"Character type definition with ID '{id}' not found in repository.");
        }

        ICharacterTypeDefinition ICharacterTypeRepository.Get(CharacterTypeId id)
        {
            return Get(id);
        }
    }
}