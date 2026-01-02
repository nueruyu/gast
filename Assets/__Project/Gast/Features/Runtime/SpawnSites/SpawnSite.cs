using Cysharp.Threading.Tasks;
using Gast.Commands;
using Gast.Core.Commands;
using Gast.Core.Observables;
using Gast.Domain.Characters;
using Gast.Features.Characters;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Gast.Features.SpawnSites
{
    /// <summary>
    /// Scene placement marker for a spawn site.
    /// Links a position in the scene to spawn site settings.
    /// </summary>
    public class SpawnSite : MonoBehaviour
    {
        [SerializeField]
        SpawnSiteEntry[] entries = { };

        [SerializeField]
        float respawnCooldown = 30f;

        ICommandDispatcher commandDispatcher;

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
            foreach (var entry in entries)
            {
                var worldPosition = transform.position + transform.rotation * entry.LocalPosition;
                var worldRotation = transform.rotation * entry.LocalRotation;

                var character = commandDispatcher.Dispatch<CreateNpcCommand, ICharacter>(new(
                    entry.TypeId,
                    worldPosition,
                    worldRotation,
                    entry.Faction));

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

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            // Draw anchor center
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            // Draw each spawn entry position
            foreach (var entry in entries)
            {
                var worldPosition = transform.position + transform.rotation * entry.LocalPosition;
                var worldRotation = transform.rotation * entry.LocalRotation;

                // Draw spawn point sphere
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(worldPosition, 0.3f);

                // Draw forward direction arrow
                Gizmos.color = Color.blue;
                var forward = worldRotation * Vector3.forward;
                Gizmos.DrawLine(worldPosition, worldPosition + forward * 1f);

                // Draw line from anchor to spawn point
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(transform.position, worldPosition);
            }
        }

        /// <summary>
        /// Defines a single character spawn entry within a spawn site.
        /// </summary>
        [Serializable]
        class SpawnSiteEntry
        {
            [SerializeField]
            CharacterTypeReference characterTypeReference;

            [SerializeField]
            Vector3 localPosition;

            [SerializeField]
            Vector3 localRotationEuler;

            [SerializeField]
            Faction faction = Faction.Enemy;

            public CharacterTypeId TypeId => characterTypeReference.Id;
            public Vector3 LocalPosition => localPosition;
            public Quaternion LocalRotation => Quaternion.Euler(localRotationEuler);
            public Faction Faction => faction;
        }
    }
}