using Cysharp.Threading.Tasks;
using Gast.Commands;
using Gast.Core.Commands;
using Gast.Core.Observables;
using Gast.Core.Tasks;
using Gast.Domain.Characters;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gast.Features.Loot
{
    public class LootSystem : ILifecycleTask
    {
        readonly ICharacterRepository characterRepository;
        readonly ICommandDispatcher commandDispatcher;

        public LootSystem(
            ICharacterRepository characterRepository,
            ICommandDispatcher commandDispatcher)
        {
            this.characterRepository = characterRepository;
            this.commandDispatcher = commandDispatcher;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            characterRepository.Registered
                .Subscribe(character => MonitorCharacter(character, cancellationToken))
                .AddTo(cancellationToken);

            foreach (var character in characterRepository.GetAll())
            {
                MonitorCharacter(character, cancellationToken);
            }

            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        void MonitorCharacter(ICharacter character, CancellationToken cancellationToken)
        {
            character.Status.IsAlive.SubscribeWithCurrent(alive =>
            {
                if (!alive)
                {
                    SpawnLoot(character);
                }
            }).AddTo(cancellationToken);
        }

        void SpawnLoot(ICharacter character)
        {
            const float RandomOffset = 0.3f;

            var spawnPos = character.Body.Position +
                Vector3.up * RandomOffset +
                Random.insideUnitSphere * RandomOffset;

            commandDispatcher.Dispatch<SpawnLootCommand>(new(character.TypeId, spawnPos));
        }
    }
}