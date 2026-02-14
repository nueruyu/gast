using Cysharp.Threading.Tasks;
using Gast.Application.Items;
using Gast.Core.Commands;
using Gast.Core.Events;
using Gast.Core.Tasks;
using Gast.Domain.Characters;
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

        public LootSystem(
            IDomainEventSubscriber eventSubscriber,
            ICommandDispatcher commandDispatcher)
        {
            this.eventSubscriber = eventSubscriber;
            this.commandDispatcher = commandDispatcher;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            eventSubscriber
                .Subscribe<LootSpawnEvent>(OnLootSpawn)
                .AddTo(cancellationToken);

            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        void OnLootSpawn(LootSpawnEvent e)
        {
            foreach (var entry in e.LootTable.Entries)
            {
                if (Random.value > entry.DropRate)
                    continue;

                const float RandomOffset = 0.3f;

                var spawnPos = e.Position +
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