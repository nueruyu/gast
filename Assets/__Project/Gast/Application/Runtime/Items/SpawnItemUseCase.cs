using Gast.Core.Commands;
using Gast.Domain.Pickups;
using System;

namespace Gast.Application.Items
{
    /// <summary>
    /// Use case for spawning a generic item pickup in the world.
    /// Handles creation, registration, and initial physics ejection.
    /// </summary>
    public class SpawnItemUseCase : ICommandHandler<SpawnItemCommand, IPickup>
    {
        readonly IPickupFactory factory;
        readonly IPickupRepository repository;

        public SpawnItemUseCase(
            IPickupFactory factory,
            IPickupRepository repository)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IPickup Execute(in SpawnItemCommand command)
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
