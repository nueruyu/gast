using System.Threading;
using System.Threading.Tasks;
using Gast.Domain.Stories;

namespace Gast.Application.AIPlanning
{
    public interface IStoryGenerationService
    {
        Task<StoryBlueprint> GenerateStoryAsync(string instruction, CancellationToken cancellationToken);
    }
}
