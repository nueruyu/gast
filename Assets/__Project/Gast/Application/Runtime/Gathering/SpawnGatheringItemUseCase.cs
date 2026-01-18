using Gast.Core.Commands;
using Gast.Domain.Pickups;
using System;

namespace Gast.Application.Gathering
{
    /// <summary>
    /// Use case for spawning a gathering item (pickup) in the world.
    /// Handles creation, registration, and initial physics ejection.
    /// </summary>
    public class SpawnGatheringItemUseCase : ICommandHandler<SpawnGatheringItemCommand, IPickup>
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

        public IPickup Execute(in SpawnGatheringItemCommand command)
        {
            var pickup = factory.Create(command.ItemId, command.Quantity, command.Position);

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