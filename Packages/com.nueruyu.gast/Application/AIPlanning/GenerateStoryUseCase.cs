using System.Threading;
using System.Threading.Tasks;
using Gast.Core.Commands;
using Gast.Core.Events;
using Gast.Domain.Stories;

namespace Gast.Application.AIPlanning
{
    public class GenerateStoryUseCase : IAsyncCommandHandler<GenerateStoryCommand, bool>
    {
        readonly IStoryGenerationService storyGenerationService;
        readonly IDomainEventPublisher eventPublisher;

        public GenerateStoryUseCase(
            IStoryGenerationService storyGenerationService,
            IDomainEventPublisher eventPublisher)
        {
            this.storyGenerationService = storyGenerationService;
            this.eventPublisher = eventPublisher;
        }

        public async ValueTask<bool> ExecuteAsync(GenerateStoryCommand command, CancellationToken cancellationToken = default)
        {
            var storyJson = await storyGenerationService.GenerateStoryAsync(command.Instruction, cancellationToken);

            if (string.IsNullOrWhiteSpace(storyJson))
                return false;

            eventPublisher.Publish(new StoryGeneratedEvent(storyJson));
            return true;
        }
    }
}
