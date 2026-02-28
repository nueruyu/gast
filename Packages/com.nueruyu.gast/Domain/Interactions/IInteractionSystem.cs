using Gast.Core.Observables;
using Gast.Domain.Characters;
using System.Threading;
using System.Threading.Tasks;

namespace Gast.Domain.Interactions
{
    public interface IInteractionSystem
    {
        /// <summary>
        /// Signal that fires when hold interaction progress changes.
        /// </summary>
        ISignal<InteractionProgressEvent> ProgressChanged { get; }

        ValueTask<bool> RequestInteractionAsync(
            CharacterId interactorId,
            InteractableId interactableId,
            CancellationToken cancellationToken = default);
    }
}