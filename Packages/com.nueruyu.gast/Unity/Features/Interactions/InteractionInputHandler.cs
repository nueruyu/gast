using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.Inputs;
using Gast.Domain.Interactions;
using Gast.Domain.Players;
using Gast.Unity.Shared.Tasks;

namespace Gast.Unity.Features.Interactions
{
    public class InteractionInputHandler : ILifecycleTask
    {
        readonly IInputProvider inputProvider;
        readonly IInteractionSystem interactionSystem;
        readonly IPlayerInteractionFocusService focusService;
        readonly IPlayerManager playerManager;
        CancellationTokenSource holdInteractionCts;

        public InteractionInputHandler(
            IInputProvider inputProvider,
            IInteractionSystem interactionSystem,
            IPlayerInteractionFocusService focusService,
            IPlayerManager playerManager)
        {
            this.inputProvider = inputProvider;
            this.interactionSystem = interactionSystem;
            this.focusService = focusService;
            this.playerManager = playerManager;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (inputProvider.InteractPressed)
                {
                    TryStartInteraction();
                }

                if (!inputProvider.InteractHeld)
                {
                    CancelHoldInteraction();
                }

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        void TryStartInteraction()
        {
            var player = playerManager.CurrentCharacter.Value;
            var focused = focusService.FocusedInteractable.Value;
            if (player == null || focused == null)
                return;

            if (focused.Config.Type == InteractionType.Hold)
            {
                CancelHoldInteraction();
                holdInteractionCts = new CancellationTokenSource();
                interactionSystem.RequestInteractionAsync(player.Id, focused.Id, holdInteractionCts.Token).Forget();
            }
            else
            {
                interactionSystem.RequestInteractionAsync(player.Id, focused.Id).Forget();
            }
        }

        void CancelHoldInteraction()
        {
            if (holdInteractionCts != null)
            {
                holdInteractionCts.Cancel();
                holdInteractionCts.Dispose();
                holdInteractionCts = null;
            }
        }
    }
}