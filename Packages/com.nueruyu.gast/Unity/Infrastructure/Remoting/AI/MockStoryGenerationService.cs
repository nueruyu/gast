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
        readonly StoryBlueprintParser parser;

        public MockStoryGenerationService(MockStoryGenerationSettings settings, StoryBlueprintParser parser)
        {
            this.settings = settings;
            this.parser = parser;
        }

        public Task<StoryBlueprint> GenerateStoryAsync(string instruction, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(settings.MockStoryJson))
                return Task.FromResult<StoryBlueprint>(null);

            return Task.FromResult(parser.Parse(settings.MockStoryJson));
        }
    }
}
