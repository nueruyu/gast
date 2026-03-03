using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using Gast.Unity.Features.SpawnSites;
using UnityEngine;

namespace Cryst.Infrastructure.Characters
{
    public class NpcSpawnSiteEntry : SpawnSiteEntryBase
    {
        [SerializeField]
        CharacterTypeReference characterTypeReference;

        [SerializeField]
        Faction faction = Faction.Enemy;

        public override ICharacterCreationParameters CreationParameters =>
            new CharacterCreationParameters(characterTypeReference.Id, faction);
    }
}
