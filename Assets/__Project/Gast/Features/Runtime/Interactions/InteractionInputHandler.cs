using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.Inputs;
using Gast.Domain.Interactions;

namespace Gast.Features.Interactions
{
    /// <summary>
    /// Connects input system to InteractionDetector.
    /// </summary>
    public class InteractionInputHandler : ILifecycleTask
    {
        readonly IInputProvider inputProvider;
        readonly IInteractionSystem interactionSystem;
        bool wasInteractHeld;

        public InteractionInputHandler(IInputProvider inputProvider, IInteractionSystem interactionSystem)
        {
            this.inputProvider = inputProvider;
            this.interactionSystem = interactionSystem;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.NextFrame(cancellationToken);

                if (inputProvider == null)
                    continue;

                if (inputProvider.InteractPressed)
                {
                    interactionSystem.TryInteract();
                }

                if (wasInteractHeld && !inputProvider.InteractHeld)
                {
                    interactionSystem.CancelInteraction();
                }

                wasInteractHeld = inputProvider.InteractHeld;
            }
        }
    }
}