using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gast.Domain.Npcs.Goals;

namespace Gast.Domain.Npcs
{
    /// <summary>
    /// Interface for AI server communication.
    /// </summary>
    public interface IAIAgentService
    {
        Task<List<IGoal>> GetGoalsAsync(string instruction, CancellationToken cancellationToken);
    }
}