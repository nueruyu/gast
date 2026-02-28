using System;
using System.Collections.Generic;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gast.Unity.Infrastructure.Characters
{
    [CreateAssetMenu(fileName = "CharacterArchetype", menuName = "Gast/Characters/Archetype")]
    public class CharacterArchetype : ScriptableObject
    {
        [SerializeField] CharacterArchetypeReference reference;

        [SerializeField] Object[] settings = { };

        [NonSerialized] Dictionary<Type, object> settingsCache;

        public CharacterArchetypeId Id => reference.Id;

        void OnValidate()
        {
            settingsCache = null;
        }

        public bool TryGetSettings<T>(out T value)
        {
            EnsureCache();

            if (settingsCache.TryGetValue(typeof(T), out var setting))
            {
                value = (T)setting;
                return true;
            }

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