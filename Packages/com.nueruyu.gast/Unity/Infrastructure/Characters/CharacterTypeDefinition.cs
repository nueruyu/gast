using System.Collections.Generic;
using System.Linq;
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
        string displayName;

        [Header("Actions")]
        [SerializeField]
        List<CharacterActionSettings> actionSettings = new();

        [Header("Features / Settings")]
        [SerializeField]
        Object[] settings = { };

        public CharacterTypeId TypeId => reference.Id;
        public string DisplayName => displayName;
        public IReadOnlyList<CharacterActionSettings> ActionSettings => actionSettings;

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
