using Gast.Api.Pickups;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Pickups;
using Random = UnityEngine.Random;

namespace Gast.Application.UseCases.Loot
{
    public class SpawnLootUseCase : ICommandHandler<SpawnLootCommand>
    {
        readonly IPickupFactory pickupFactory;
        readonly IPickupRepository pickupRepository;
        readonly ICharacterTypeRepository characterTypeRepository;

        public SpawnLootUseCase(
            IPickupFactory pickupFactory,
            IPickupRepository pickupRepository,
            ICharacterTypeRepository characterTypeRepository)
        {
            this.pickupFactory = pickupFactory;
            this.pickupRepository = pickupRepository;
            this.characterTypeRepository = characterTypeRepository;
        }

        public void Execute(in SpawnLootCommand command)
        {
            var characterType = characterTypeRepository.Get(command.CharacterTypeId);

            foreach (var entry in characterType.LootTable.Entries)
            {
                if (Random.value > entry.DropRate)
                    continue;

                var quantity = Random.Range(entry.MinQuantity, entry.MaxQuantity + 1);
                var pickup = pickupFactory.Create(entry.ItemId, quantity, command.Position);
                pickupRepository.Register(pickup);

                pickup.Destroyed.Subscribe(OnPickupDestroyed);

                pickup.Eject();
            }
        }

        void OnPickupDestroyed(IPickup pickup)
        {
            pickupRepository.Unregister(pickup.Id);
        }
    }
}