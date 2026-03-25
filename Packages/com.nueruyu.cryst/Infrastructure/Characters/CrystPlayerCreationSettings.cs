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

        public override ICharacterCreationParameters Create()
        {
            return new CharacterCreationParameters(characterTypeReference.Id, faction);
        }
    }
}
