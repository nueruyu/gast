using System;
using System.Collections.Generic;
using System.Linq;
using Gast.Domain.Economy;

namespace Gast.Infrastructure.Items
{
    public class ItemRepository : IItemRepository
    {
        readonly Dictionary<ItemId, ItemDefinition> definitionMap;

        public ItemRepository(ItemDatabaseSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            definitionMap = settings.Items.ToDictionary(
                def => def.Id,
                def => def);
        }

        public ItemDefinition Get(ItemId id)
        {
            if (definitionMap.TryGetValue(id, out var definition))
                return definition;

            throw new KeyNotFoundException($"Item definition with ID '{id}' not found in repository.");
        }

        public IEnumerable<ItemDefinition> GetAllDefinitions()
        {
            return definitionMap.Values;
        }

        IItemDefinition IItemRepository.Get(ItemId id)
        {
            return Get(id);
        }

        IEnumerable<IItemDefinition> IItemRepository.GetAllDefinitions()
        {
            return definitionMap.Values;
        }
    }
}