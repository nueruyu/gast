using System;
using Gast.Domain.Economy;
using Gast.Domain.Pickups;
using UnityEngine;

namespace Gast.UseCases.Gathering
{
    /// <summary>
    /// Use case for spawning a gathering item (pickup) in the world.
    /// Handles creation, registration, and initial physics ejection.
    /// </summary>
    public class SpawnGatheringItemUseCase
    {
        readonly IPickupFactory factory;
        readonly IPickupRepository repository;

        public SpawnGatheringItemUseCase(
            IPickupFactory factory,
            IPickupRepository repository)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Spawns a pickup item at the specified position.
        /// </summary>
        public IPickup Execute(ItemId itemId, int quantity, Vector3 position)
        {
            var pickup = factory.Create(itemId, quantity, position);

            repository.Register(pickup);

            pickup.Destroyed.Subscribe(OnPickupDestroyed);

            pickup.Eject();

            return pickup;
        }

        void OnPickupDestroyed(IPickup pickup)
        {
            repository.Unregister(pickup.Id);
        }
    }
}