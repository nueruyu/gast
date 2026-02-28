using System.Collections.Generic;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Gast.Unity.Infrastructure.Characters
{
    [CreateAssetMenu(fileName = "CharacterType", menuName = "Gast/Characters/Type Definition")]
    public class CharacterTypeDefinition : ScriptableObject, ICharacterTypeDefinition
    {
        [SerializeField]
        CharacterTypeReference reference;

        [SerializeField]
        CharacterArchetypeReference archetypeReference;

        [SerializeField]
        string displayName;

        [SerializeField]
        Object[] settings = { };

        public CharacterTypeId TypeId => reference.Id;
        public CharacterArchetypeId ArchetypeId => archetypeReference.Id;
        public string DisplayName => displayName;
        
        public bool TryGetSettings<T>(out T value)
        {
            foreach (var s in settings)
            {
                if (s is T matched)
                {
                    value = matched;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public IReadOnlyList<Object> Settings => settings;
    }
}
