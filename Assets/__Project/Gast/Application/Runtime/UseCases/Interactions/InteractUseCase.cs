using System.Threading;
using System.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;

namespace Gast.Application.UseCases.Interactions
{
    public class InteractUseCase
    {
        readonly IInteractionSystem interactionSystem;

        public InteractUseCase(IInteractionSystem interactionSystem)
        {
            this.interactionSystem = interactionSystem;
        }

        public ValueTask<bool> ExecuteAsync(CharacterId interactorId, InteractableId targetId, CancellationToken cancellationToken)
        {
            return interactionSystem.RequestInteractionAsync(interactorId, targetId, cancellationToken);
        }
    }
}
