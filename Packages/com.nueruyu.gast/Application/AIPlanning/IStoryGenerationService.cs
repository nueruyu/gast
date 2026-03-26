using System.Threading;
using System.Threading.Tasks;

namespace Gast.Application.AIPlanning
{
    public interface IStoryGenerationService
    {
        Task<string> GenerateStoryAsync(string instruction, CancellationToken cancellationToken);
    }
}
