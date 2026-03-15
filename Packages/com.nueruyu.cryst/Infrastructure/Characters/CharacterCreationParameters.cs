using Cryst.Domain.Characters;
using Gast.Domain.Characters;

namespace Cryst.Infrastructure.Characters
{
    public class CharacterCreationParameters : ICharacterCreationParameters
    {
        public CharacterTypeId TypeId { get; }
        public Faction Faction { get; }
        public Territory? Territory { get; }

        public CharacterCreationParameters(CharacterTypeId typeId, Faction faction, Territory? territory = null)
        {
            TypeId = typeId;
            Faction = faction;
            Territory = territory;
        }
    }
}
