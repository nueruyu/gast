using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using Gast.Features.Characters;
using UnityEngine;

namespace Cryst.Infrastructure.Characters
{
    /// <summary>
    /// ScriptableObject containing parameters for creating a Cryst character.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterCreationParams", menuName = "Cryst/Character Creation Parameters")]
    public class CharacterCreationParameters : ScriptableObject, ICharacterCreationParameters
    {
        [SerializeField]
        CharacterTypeReference characterTypeReference;

        [SerializeField]
        Faction faction = Faction.Enemy;

        public CharacterTypeId TypeId => characterTypeReference.Id;
        public Faction Faction => faction;
    }
}