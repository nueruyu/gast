using Cysharp.Threading.Tasks;
using DescrioGames.Commands;
using DescrioGames.Core.Commands;
using DescrioGames.Core.Observables;
using DescrioGames.Domain.Pickups;
using DescrioGames.Features.Economy;
using System;
using System.Threading;
using UnityEngine;

namespace DescrioGames.Features.Gathering
{
    /// <summary>
    /// Represents a location where items can be gathered.
    /// Manages the spawning lifecycle of the gathering item.
    /// </summary>
    public class GatheringSpot : MonoBehaviour
    {
        [SerializeField]
        ItemReference itemReference;

        [SerializeField]
        int quantity = 1;

        [SerializeField]
        float respawnTime = 60f;

        [SerializeField]
        bool spawnOnStart = true;

        ICommandDispatcher commandDispatcher;

        IPickup currentPickup;

        public void Initialize(ICommandDispatcher commandDispatcher)
        {
            this.commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));

            RunAsync(destroyCancellationToken).Forget();
        }

        async UniTask RunAsync(CancellationToken cancellationToken)
        {
            if (!spawnOnStart)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(respawnTime), cancellationToken: cancellationToken);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                SpawnItem();

                await UniTask.WaitUntil(() => currentPickup == null, cancellationToken: cancellationToken);

                await UniTask.Delay(TimeSpan.FromSeconds(respawnTime), cancellationToken: cancellationToken);
            }
        }

        void SpawnItem()
        {
            try
            {
                currentPickup = commandDispatcher.Dispatch<SpawnGatheringItemCommand, IPickup>(new(
                    itemReference.Id,
                    quantity,
                    transform.position
                ));

                currentPickup.Destroyed.Subscribe(_ =>
                {
                    currentPickup = null;
                }).AddTo(destroyCancellationToken);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GatheringSpot] Failed to spawn item: {ex}");
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        }
    }
}