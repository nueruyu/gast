using System.Threading;
using System.Threading.Tasks;
using Gast.Application.AIPlanning;

namespace Gast.Unity.Infrastructure.Remoting.AI
{
    public class MockStoryGenerationService : IStoryGenerationService
    {
        readonly MockStoryGenerationSettings settings;

        public MockStoryGenerationService(MockStoryGenerationSettings settings)
        {
            this.settings = settings;
        }

        public Task<string> GenerateStoryAsync(string instruction, CancellationToken cancellationToken)
        {
            return Task.FromResult(settings.MockStoryJson);
        }
    }
}
