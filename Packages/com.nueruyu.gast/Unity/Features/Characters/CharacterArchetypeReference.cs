using System;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Unity.Features.Characters
{
    [CreateAssetMenu(fileName = "CharacterArchetypeReference", menuName = "Gast/Characters/Character Archetype Reference")]
    public class CharacterArchetypeReference : ScriptableObject
    {
        [SerializeField]
        string id;

        public CharacterArchetypeId Id => CharacterArchetypeId.FromString(id);

        void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }
        }
    }
}
