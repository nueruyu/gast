using Cysharp.Threading.Tasks;
using Gast.Application.Items;
using Gast.Core.Commands;
using Gast.Core.Events;
using Gast.Core.Tasks;
using Gast.Domain.Characters;
using Gast.Domain.Events;
using Gast.Domain.Pickups;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gast.Features.Loot
{
    public class LootSystem : ILifecycleTask
    {
        readonly IDomainEventSubscriber eventSubscriber;
        readonly ICommandDispatcher commandDispatcher;
        readonly ICharacterTypeRepository characterTypeRepository;

        public LootSystem(
            IDomainEventSubscriber eventSubscriber,
            ICommandDispatcher commandDispatcher,
            ICharacterTypeRepository characterTypeRepository)
        {
            this.eventSubscriber = eventSubscriber;
            this.commandDispatcher = commandDispatcher;
            this.characterTypeRepository = characterTypeRepository;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            eventSubscriber
                .Subscribe<CharacterDefeatedEvent>(OnCharacterDefeated)
                .AddTo(cancellationToken);

            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        void OnCharacterDefeated(CharacterDefeatedEvent e)
        {
            var character = e.DefeatedCharacter;
            var characterType = characterTypeRepository.Get(character.TypeId);

            foreach (var entry in characterType.LootTable.Entries)
            {
                if (Random.value > entry.DropRate)
                    continue;

                const float RandomOffset = 0.3f;

                var spawnPos = character.Body.Position +
                    Vector3.up * RandomOffset +
                    Random.insideUnitSphere * RandomOffset;

                var quantity = Random.Range(entry.MinQuantity, entry.MaxQuantity + 1);

                commandDispatcher.Dispatch<SpawnItemCommand, IPickup>(new(
                    entry.ItemId,
                    quantity,
                    spawnPos));
            }
        }
    }
}