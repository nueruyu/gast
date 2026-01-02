using DescrioGames.Commands;
using DescrioGames.Domain.Pickups;
using DescrioGames.UseCases.Economy;
using DescrioGames.UseCases.Gathering;
using DescrioGames.UseCases.Loot;

namespace DescrioGames.Api.CommandHandlers
{
    public class PickupCommandHandler :
        ICommandHandler<PickUpItemCommand, bool>,
        ICommandHandler<SpawnGatheringItemCommand, IPickup>,
        ICommandHandler<SpawnLootCommand>
    {
        readonly PickUpItemUseCase pickUpItemUseCase;
        readonly SpawnGatheringItemUseCase spawnGatheringItemUseCase;
        readonly SpawnLootUseCase spawnLootUseCase;

        public PickupCommandHandler(
            PickUpItemUseCase pickUpItemUseCase,
            SpawnGatheringItemUseCase spawnGatheringItemUseCase,
            SpawnLootUseCase spawnLootUseCase)
        {
            this.pickUpItemUseCase = pickUpItemUseCase;
            this.spawnGatheringItemUseCase = spawnGatheringItemUseCase;
            this.spawnLootUseCase = spawnLootUseCase;
        }

        public bool Execute(in PickUpItemCommand command)
        {
            return pickUpItemUseCase.Execute(command.PickerId, command.ItemId, command.Quantity);
        }

        public IPickup Execute(in SpawnGatheringItemCommand command)
        {
            return spawnGatheringItemUseCase.Execute(command.ItemId, command.Quantity, command.Position);
        }

        public void Execute(in SpawnLootCommand command)
        {
            spawnLootUseCase.Execute(command.CharacterTypeId, command.Position);
        }
    }
}