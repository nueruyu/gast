using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using Gast.Unity.Features.Gameplay;
using UnityEngine;

namespace Cryst.Infrastructure.Characters
{
    [CreateAssetMenu(fileName = "PlayerCreationSettings", menuName = "Cryst/Player Creation Settings")]
    public class CrystPlayerCreationSettings : PlayerCreationSettings
    {
        [SerializeField]
        CharacterTypeReference characterTypeReference;

        [SerializeField]
        Faction faction = Faction.Ally;

        [SerializeField]
        [Tooltip("Optional fixed character id. Leave empty for dynamic generation.")]
        string fixedCharacterId;

        public override ICharacterCreationParameters Create()
        {
            var fixedId = string.IsNullOrEmpty(fixedCharacterId)
                ? (CharacterId?)null
                : CharacterId.FromString(fixedCharacterId);
            return new CharacterCreationParameters(characterTypeReference.Id, faction, fixedId: fixedId);
        }
    }
}
