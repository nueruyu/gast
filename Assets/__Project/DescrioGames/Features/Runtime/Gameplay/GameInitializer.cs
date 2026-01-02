using DescrioGames.Commands;
using DescrioGames.Core.Commands;
using DescrioGames.Core.Tasks;
using DescrioGames.Domain.Characters;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace DescrioGames.Features.Gameplay
{
    /// <summary>
    /// Entry point for game initialization.
    /// Spawns the player character at the designated spawn point.
    /// </summary>
    public class GameInitializer : ILifecycleTask
    {
        readonly ICommandDispatcher commandDispatcher;
        readonly PlayerSpawnPoint spawnPoint;
        readonly GameInitializationSettings settings;

        public GameInitializer(
            ICommandDispatcher commandDispatcher,
            GameInitializationSettings settings,
            PlayerSpawnPoint spawnPoint)
        {
            this.commandDispatcher = commandDispatcher;
            this.settings = settings;
            this.spawnPoint = spawnPoint;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            // Determine spawn position and rotation
            var position = Vector3.zero;
            var rotation = Quaternion.identity;

            if (spawnPoint != null)
            {
                position = spawnPoint.transform.position;
                rotation = spawnPoint.transform.rotation;
            }
            else
            {
                Debug.LogWarning("GameInitializer: No PlayerSpawnPoint found, spawning at origin");
            }

            // Get player type ID from settings
            var playerTypeId = settings.PlayerCharacterTypeId;

            // Create and possess the player character
            commandDispatcher.Dispatch<CreatePlayerCommand, ICharacter>(new(
                playerTypeId,
                position,
                rotation
            ));

            Debug.Log($"GameInitializer: Player '{playerTypeId}' spawned at {position}");

            return Task.CompletedTask;
        }
    }
}