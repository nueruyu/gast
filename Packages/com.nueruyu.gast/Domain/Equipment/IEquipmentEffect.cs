using Gast.Core.Stats;

namespace Gast.Domain.Equipment
{
    public interface IEquipmentEffect
    {
        void ApplyTo(StatBuilder stats);
    }
}
