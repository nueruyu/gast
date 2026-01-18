using Gast.Core.Commands;
using Gast.Core.Events;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Events;

namespace Gast.Application.Economy
{
    /// <summary>
    /// Use case for picking up items and adding them to inventory.
    /// </summary>
    public class PickUpItemUseCase : ICommandHandler<PickUpItemCommand, bool>
    {
        readonly ICharacterRepository characterRepository;
        readonly IItemRepository itemRepository;
        readonly IDomainEventPublisher eventPublisher;

        public PickUpItemUseCase(ICharacterRepository characterRepository, IItemRepository itemRepository, IDomainEventPublisher eventPublisher)
        {
            this.characterRepository = characterRepository;
            this.itemRepository = itemRepository;
            this.eventPublisher = eventPublisher;
        }

        public bool Execute(in PickUpItemCommand command)
        {
            var character = characterRepository.Get(command.PickerId);
            var itemDefinition = itemRepository.Get(command.ItemId);

            var addedCount = character.Inventory.AddItem(itemDefinition, command.Quantity);

            if (addedCount > 0)
            {
                eventPublisher.Publish(new ItemAcquiredEvent(command.PickerId, command.ItemId, addedCount));
            }

            return addedCount == command.Quantity;
        }
    }
}