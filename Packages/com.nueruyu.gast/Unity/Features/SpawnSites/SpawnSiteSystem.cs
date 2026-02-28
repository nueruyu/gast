using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Commands;
using Gast.Core.Tasks;
using UnityEngine;

namespace Gast.Unity.Features.SpawnSites
{
    /// <summary>
    /// System that manages all spawn sites in the scene.
    /// Collects all SpawnSiteAnchors and runs their lifecycle processors.
    /// </summary>
    public class SpawnSiteSystem : ILifecycleTask
    {
        readonly ICommandDispatcher commandDispatcher;
        readonly SpawnSiteRegistry registry;

        public SpawnSiteSystem(ICommandDispatcher commandDispatcher, SpawnSiteRegistry registry)
        {
            this.commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
            this.registry = registry;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var spawnSites = registry.GetSpawnSites().ToArray();

            if (spawnSites.Length == 0)
            {
                Debug.Log("SpawnSiteSystem: No spawn sites found in scene.");
                return;
            }

            Debug.Log($"SpawnSiteSystem: Found {spawnSites.Length} spawn site(s).");

            foreach (var site in spawnSites)
            {
                site.Initialize(commandDispatcher);
            }

            await UniTask.WaitUntilCanceled(cancellationToken);
        }
    }
}