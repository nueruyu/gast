using System;
using UnityEngine;
using Gast.Domain.Characters;

namespace Gast.Features.Characters
{
    [CreateAssetMenu(fileName = "CharacterTypeReference", menuName = "DescrioGames/Characters/Character Type Reference")]
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