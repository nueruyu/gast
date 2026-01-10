using System.Threading.Tasks;
using Gast.Api.Interactions;
using Gast.Application.UseCases.Interactions;

namespace Gast.Application.CommandHandlers
{
    public class InteractionCommandHandler : IAsyncCommandHandler<InteractCommand, bool>
    {
        readonly InteractUseCase interactUseCase;

        public InteractionCommandHandler(InteractUseCase interactUseCase)
        {
            this.interactUseCase = interactUseCase;
        }

        public ValueTask<bool> ExecuteAsync(InteractCommand command)
        {
            return interactUseCase.ExecuteAsync(command.InteractorId, command.TargetId, default);
        }
    }
}
