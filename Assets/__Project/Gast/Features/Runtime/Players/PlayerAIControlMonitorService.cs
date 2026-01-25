using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.AI;
using Gast.Domain.Inputs;
using Gast.Domain.Players;
using R3;
using UnityEngine;

namespace Gast.Features.Players
{
    public class PlayerAIControlMonitorService : ILifecycleTask
    {
        readonly IInputProvider inputProvider;
        readonly IPlayerManager playerManager;

        public PlayerAIControlMonitorService(
            IInputProvider inputProvider,
            IPlayerManager playerManager)
        {
            this.inputProvider = inputProvider;
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
                if (ShouldRestore(brain))
                {
                    Debug.Log("AIControlMonitorService: Restoration conditions met, restoring player control");
                    playerManager.RestorePlayerControl();
                    break;
                }

                await UniTask.NextFrame(cancellationToken);
            }
        }

        bool ShouldRestore(ICharacterAIBrain brain)
        {
            if (HasPlayerInput())
            {
                Debug.Log("AIControlMonitorService: Player input detected");
                return true;
            }

            if (AreAllGoalsCompleted(brain))
            {
                Debug.Log("AIControlMonitorService: All goals completed");
                return true;
            }

            return false;
        }

        bool HasPlayerInput()
        {
            return inputProvider.Move.sqrMagnitude > 0.01f
                || inputProvider.Look.sqrMagnitude > 0.01f
                || inputProvider.Jump
                || inputProvider.Sprint
                || inputProvider.InteractPressed
                || inputProvider.Attack
                || inputProvider.Dash
                || inputProvider.GuardHeld;
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