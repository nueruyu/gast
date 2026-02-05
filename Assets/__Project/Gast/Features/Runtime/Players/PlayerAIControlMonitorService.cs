using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.AI;
using Gast.Domain.Players;
using Gast.Shared.Observables;
using R3;
using UnityEngine;

namespace Gast.Features.Players
{
    public class PlayerAIControlMonitorService : ILifecycleTask
    {
        readonly IPlayerManager playerManager;

        public PlayerAIControlMonitorService(IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            CancellationTokenSource monitorCts = null;

            cancellationToken.Register(() =>
            {
                monitorCts?.Cancel();
                monitorCts?.Dispose();
            });

            playerManager.CurrentAIBrain
                .ToObservable()
                .Subscribe(aiBrain =>
                {
                    monitorCts?.Cancel();
                    monitorCts?.Dispose();

                    if (aiBrain != null)
                    {
                        monitorCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        MonitorAsync(aiBrain, monitorCts.Token).Forget();
                    }
                })
                .AddTo(cancellationToken);

            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        async UniTask MonitorAsync(ICharacterAIBrain brain, CancellationToken cancellationToken)
        {
            if (brain.CurrentObjectives == null || brain.CurrentObjectives.Count == 0)
            {
                Debug.Log("AIControlMonitorService: No goals to monitor.");
                playerManager.RestorePlayerControl();
                return;
            }

            // Combine the IsCompleted status of all objectives.
            // When all are true, the combined observable will emit true.

            var allObjectivesCompleted = Observable.CombineLatest(brain.CurrentObjectives
                .Select(obj => obj.IsCompleted.ToObservable()))
                .Select(statuses => statuses.All(isCompleted => isCompleted));

            try
            {
                // Wait until the first time `allObjectivesCompleted` becomes true.
                await allObjectivesCompleted
                    .Where(allCompleted => allCompleted)
                    .FirstAsync(cancellationToken);

                Debug.Log("AIControlMonitorService: All goals completed");
                playerManager.RestorePlayerControl();
            }
            catch (OperationCanceledException)
            {
                // This is expected when monitoring is cancelled.
            }
        }
    }
}