using System.Threading;
using System.Threading.Tasks;
using Gast.Core.Commands;

namespace Gast.Application.AIPlanning
{
    public class GenerateStoryUseCase : IAsyncCommandHandler<GenerateStoryCommand, bool>
    {
        readonly IStoryGenerationService storyGenerationService;
        readonly IStoryRunner storyRunner;

        public GenerateStoryUseCase(
            IStoryGenerationService storyGenerationService,
            IStoryRunner storyRunner)
        {
            this.storyGenerationService = storyGenerationService;
            this.storyRunner = storyRunner;
        }

        public async ValueTask<bool> ExecuteAsync(GenerateStoryCommand command,
            CancellationToken cancellationToken = default)
        {
            var blueprint = await storyGenerationService.GenerateStoryAsync(command.Instruction, cancellationToken);
            storyRunner.StartStory(blueprint);
            return true;
        }
    }
}