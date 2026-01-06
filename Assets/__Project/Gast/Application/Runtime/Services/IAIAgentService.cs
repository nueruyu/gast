using System.Threading;
using System.Threading.Tasks;

namespace Gast.Application.Services
{
    /// <summary>
    /// Interface for AI server communication.
    /// </summary>
    public interface IAIAgentService
    {
        Task<AIAgentResult> GetGoalsAsync(string instruction, CancellationToken cancellationToken);
    }
}