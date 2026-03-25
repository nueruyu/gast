using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Application.Characters;
using Gast.Core.Commands;
using Gast.Core.Observables;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Unity.Features.SpawnSites
{
    public class SpawnSite : MonoBehaviour
    {
        [SerializeField]
        float respawnCooldown = 30f;

        [SerializeField]
        float territoryRadius = 20f;

        ICommandDispatcher commandDispatcher;

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            var entries = GetComponentsInChildren<SpawnSiteEntryBase>();
            foreach (var entry in entries)
            {
                var entryTransform = entry.transform;
                var worldPosition = entryTransform.position;
                var worldRotation = entryTransform.rotation;

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(worldPosition, 0.3f);

                Gizmos.color = Color.blue;
                var forward = worldRotation * Vector3.forward;
                Gizmos.DrawLine(worldPosition, worldPosition + forward * 1f);

                Gizmos.color = Color.gray;
                Gizmos.DrawLine(transform.position, worldPosition);
            }
        }

        public float TerritoryRadius => territoryRadius;

        public void Initialize(ICommandDispatcher commandDispatcher)
        {
            this.commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));

            RunAsync(destroyCancellationToken).Forget();
        }

        async UniTask RunAsync(CancellationToken cancellationToken)
        {
            using var subscriptions = new DisposableBag();
            var activeCharacters = new List<ICharacter>();

            while (!cancellationToken.IsCancellationRequested)
            {
                SpawnAll(activeCharacters, subscriptions);

                await UniTask.WaitUntil(
                    () => activeCharacters.Count == 0,
                    cancellationToken: cancellationToken);

                subscriptions.Clear();

                var cooldown = TimeSpan.FromSeconds(respawnCooldown);
                await UniTask.Delay(
                    cooldown,
                    cancellationToken: cancellationToken);
            }
        }

        void SpawnAll(List<ICharacter> activeCharacters, DisposableBag disposableBag)
        {
            var entries = GetComponentsInChildren<SpawnSiteEntryBase>();
            foreach (var entry in entries)
            {
                var entryTransform = entry.transform;
                var worldPosition = entryTransform.position;
                var worldRotation = entryTransform.rotation;

                var character = commandDispatcher.Dispatch<CreateNpcCommand, ICharacter>(new CreateNpcCommand(
                    entry.CreationParameters,
                    worldPosition,
                    worldRotation));

                activeCharacters.Add(character);

                character.Destroyed
                    .Subscribe(OnCharacterDestroyed)
                    .AddTo(disposableBag);
            }

            void OnCharacterDestroyed(ICharacter character)
            {
                activeCharacters.Remove(character);
            }
        }
    }
}
