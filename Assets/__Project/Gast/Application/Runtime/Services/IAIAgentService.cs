using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gast.Api.AI;

namespace Gast.Application.Services
{
    /// <summary>
    /// Interface for AI server communication.
    /// </summary>
    public interface IAIAgentService
    {
        Task<List<IGoal>> GetGoalsAsync(string instruction, CancellationToken cancellationToken);
    }
}