using Gast.Core.Commands;
using Gast.Domain.Economy;
using Gast.Domain.Pickups;
using Gast.Features.Pickups;
using Gast.Infrastructure.Settings;
using System;
using UnityEngine;

namespace Gast.Infrastructure.Factories
{
    public class PickupFactory : IPickupFactory
    {
        readonly PickupSystemSettings settings;
        readonly ICommandDispatcher commandDispatcher;
        readonly IItemRepository itemRepository;

        public PickupFactory(
            PickupSystemSettings settings,
            ICommandDispatcher commandDispatcher,
            IItemRepository itemRepository)
        {
            this.settings = settings;
            this.commandDispatcher = commandDispatcher;
            this.itemRepository = itemRepository;
        }

        public IPickup Create(ItemId itemId, int quantity, Vector3 position)
        {
            var instance = UnityEngine.Object.Instantiate(settings.PickupPrefab, position, Quaternion.identity);

            if (!instance.TryGetComponent<Pickup>(out var pickup))
            {
                throw new InvalidOperationException("LootPrefab does not have a PickupInteractable component");
            }

            var itemDefinition = itemRepository.Get(itemId);
            pickup.Initialize(
                PickupId.New(),
                itemId,
                itemDefinition.Name,
                quantity,
                commandDispatcher);

            return pickup;
        }
    }
}