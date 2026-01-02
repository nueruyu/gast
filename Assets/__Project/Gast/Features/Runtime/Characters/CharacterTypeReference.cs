using System;
using UnityEngine;
using DescrioGames.Domain.Characters;

namespace DescrioGames.Features.Characters
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