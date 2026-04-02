using Gast.Core.Stats;
using Gast.Domain.Characters;

namespace Gast.Domain.Equipment
{
    public interface IEquipmentBonusApplicable : ICharacterFacet
    {
        void ApplyEquipmentBonuses(StatBuilder stats);
    }
}
