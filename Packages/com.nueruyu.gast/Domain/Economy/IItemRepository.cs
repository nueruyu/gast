using System.Collections.Generic;

namespace Gast.Domain.Economy
{
    public interface IItemRepository
    {
        IItemDefinition Get(ItemId id);

        IEnumerable<IItemDefinition> GetAllDefinitions();
    }
}