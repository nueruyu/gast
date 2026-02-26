using System.Threading;
using System.Threading.Tasks;

namespace Gast.Core.Tasks
{
    /// <summary>
    /// Represents a task that runs continuously during the application or scope lifetime.
    /// </summary>
    public interface ILifecycleTask
    {
        /// <summary>
        /// Run the task asynchronously until cancellation is requested.
        /// </summary>
        /// <param name="cancellationToken">Token to signal cancellation</param>
        Task RunAsync(CancellationToken cancellationToken);
    }
}