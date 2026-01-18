using Gast.Api.Interactions;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using System.Threading;
using System.Threading.Tasks;

namespace Gast.Application.UseCases.Interactions
{
    public class InteractUseCase : IAsyncCommandHandler<InteractCommand, bool>
    {
        readonly IInteractionSystem interactionSystem;

        public InteractUseCase(IInteractionSystem interactionSystem)
        {
            this.interactionSystem = interactionSystem;
        }

        public ValueTask<bool> ExecuteAsync(InteractCommand command, CancellationToken cancellationToken)
        {
            return interactionSystem.RequestInteractionAsync(
                command.InteractorId,
                command.TargetId,
                cancellationToken);
        }
    }
}