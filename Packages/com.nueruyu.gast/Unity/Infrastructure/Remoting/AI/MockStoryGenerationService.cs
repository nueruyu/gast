using System.Threading;
using System.Threading.Tasks;
using Gast.Application.AIPlanning;
using Gast.Domain.Stories;
using Gast.Unity.Infrastructure.Stories;

namespace Gast.Unity.Infrastructure.Remoting.AI
{
    public class MockStoryGenerationService : IStoryGenerationService
    {
        readonly MockStoryGenerationSettings settings;

        public MockStoryGenerationService(MockStoryGenerationSettings settings)
        {
            this.settings = settings;
        }

        public Task<StoryBlueprint> GenerateStoryAsync(string instruction, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(settings.MockStoryJson))
                return Task.FromResult<StoryBlueprint>(null);

            return Task.FromResult(StoryBlueprintParser.Parse(settings.MockStoryJson));
        }
    }
}
