using System;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Unity.Features.Characters
{
    [CreateAssetMenu(fileName = "CharacterTypeReference", menuName = "Gast/Characters/Character Type Reference")]
    public class CharacterTypeReference : ScriptableObject
    {
        [SerializeField]
        string id;

        public CharacterTypeId Id => CharacterTypeId.FromString(id);

        void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }
        }
    }
}