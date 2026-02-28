using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Commands;
using Gast.Core.Tasks;
using UnityEngine;

namespace Gast.Unity.Features.Gathering
{
    /// <summary>
    /// System that manages all gathering spots in the scene.
    /// Initializes spots with the necessary use cases.
    /// </summary>
    public class GatheringSystem : ILifecycleTask
    {
        readonly GatheringSpotRegistry spotRegistry;
        readonly ICommandDispatcher commandDispatcher;

        public GatheringSystem(
            GatheringSpotRegistry spotRegistry,
            ICommandDispatcher commandDispatcher)
        {
            this.spotRegistry = spotRegistry;
            this.commandDispatcher = commandDispatcher;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var spots = spotRegistry.GetGatheringSpots().ToArray();

            Debug.Log($"[GatheringSystem] Initializing {spots.Length} gathering spots.");

            foreach (var spot in spots)
            {
                spot.Initialize(commandDispatcher);
            }

            await UniTask.WaitUntilCanceled(cancellationToken);
        }
    }
}