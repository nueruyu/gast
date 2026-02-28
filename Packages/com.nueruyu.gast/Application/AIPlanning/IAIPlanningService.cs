using System.Threading;
using System.Threading.Tasks;

namespace Gast.Application.AIPlanning
{
    /// <summary>
    /// Interface for AI server communication.
    /// </summary>
    public interface IAIPlanningService
    {
        Task<AIPlanningResult> GetObjectivesAsync(string instruction, CancellationToken cancellationToken);
    }
}