using System;
using Gast.Core.Commands;
using Gast.Domain.Economy;
using Gast.Domain.Pickups;
using Gast.Features.Interactions;
using Gast.Features.Pickups;
using Gast.Infrastructure.Settings;
using UnityEngine;
using VContainer;

namespace Gast.Infrastructure.Factories
{
    public class PickupFactory : IPickupFactory
    {
        readonly PickupSystemSettings settings;
        readonly ICommandDispatcher commandDispatcher;
        readonly IItemRepository itemRepository;
        readonly InteractionSystem interactionSystem;

        public PickupFactory(
            PickupSystemSettings settings,
            ICommandDispatcher commandDispatcher,
            IItemRepository itemRepository,
            InteractionSystem interactionSystem)
        {
            this.settings = settings;
            this.commandDispatcher = commandDispatcher;
            this.itemRepository = itemRepository;
            this.interactionSystem = interactionSystem;
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
                commandDispatcher,
                interactionSystem);

            return pickup;
        }
    }
}