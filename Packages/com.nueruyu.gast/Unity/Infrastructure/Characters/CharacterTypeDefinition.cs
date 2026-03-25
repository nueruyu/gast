using System;
using System.Collections.Generic;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gast.Unity.Infrastructure.Characters
{
    [CreateAssetMenu(fileName = "CharacterType", menuName = "Gast/Characters/Type Definition")]
    public class CharacterTypeDefinition : ScriptableObject, ICharacterTypeDefinition
    {
        [SerializeField] CharacterTypeReference reference;

        [SerializeField] CharacterArchetype archetype;

        [SerializeField] string displayName;

        [SerializeField] Object[] settings = { };

        [NonSerialized] Dictionary<Type, object> settingsCache;

        void OnValidate()
        {
            settingsCache = null;
        }

        public CharacterTypeId TypeId => reference.Id;
        public CharacterArchetypeId ArchetypeId => archetype != null ? archetype.Id : default;
        public string DisplayName => displayName;

        public bool TryGetSettings<T>(out T value)
        {
            EnsureCache();

            if (settingsCache.TryGetValue(typeof(T), out var setting))
            {
                value = (T)setting;
                return true;
            }

            foreach (var s in settings)
            {
                if (s is T match)
                {
                    settingsCache[typeof(T)] = match;
                    value = match;
                    return true;
                }
            }

            if (archetype != null) return archetype.TryGetSettings(out value);

            value = default;
            return false;
        }

        void EnsureCache()
        {
            if (settingsCache != null) return;

            settingsCache = new Dictionary<Type, object>();
            foreach (var s in settings)
            {
                if (s == null) continue;
                settingsCache[s.GetType()] = s;
            }
        }
    }
}