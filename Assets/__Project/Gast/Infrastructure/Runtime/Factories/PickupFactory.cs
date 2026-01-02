using DescrioGames.Core.Commands;
using DescrioGames.Domain.Economy;
using DescrioGames.Domain.Pickups;
using DescrioGames.Features.Pickups;
using DescrioGames.Infrastructure.Settings;
using System;
using UnityEngine;

namespace DescrioGames.Infrastructure.Factories
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