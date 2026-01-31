using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.AI;
using Gast.Domain.Players;
using R3;
using UnityEngine;

namespace Gast.Features.Players
{
    public class PlayerAIControlMonitorService : ILifecycleTask
    {
        // Polling interval for checking goal completion
        const int PollIntervalMilliseconds = 500;

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
            });

            playerManager.CurrentAIBrain
                .Subscribe(aiBrain =>
                {
                    monitorCts?.Cancel();

                    if (aiBrain != null)
                    {
                        monitorCts = new CancellationTokenSource();
                        MonitorAsync(aiBrain, monitorCts.Token).Forget();
                    }
                })
                .AddTo(cancellationToken);

            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        async UniTask MonitorAsync(ICharacterAIBrain brain, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (AreAllGoalsCompleted(brain))
                {
                    Debug.Log("AIControlMonitorService: All goals completed");
                    playerManager.RestorePlayerControl();
                    break;
                }

                await UniTask.Delay(PollIntervalMilliseconds, cancellationToken: cancellationToken);
            }
        }

        bool AreAllGoalsCompleted(ICharacterAIBrain brain)
        {
            var goals = brain.CurrentObjectives;

            if (goals == null || goals.Count == 0)
                return true;

            return goals.All(g => g.IsCompleted);
        }
    }
}
