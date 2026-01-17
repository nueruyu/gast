using Cysharp.Threading.Tasks;
using System.Threading;

namespace Gast.Lib.AI
{
    /// <summary>
    /// Interface for an AI agent runner that executes a planning and action cycle.
    /// </summary>
    /// <typeparam name="TWorldState">The type of the world state.</typeparam>
    public interface IAgentRunner<in TWorldState, TContext>
       where TWorldState : class, IWorldState<TWorldState>, new()
       where TContext : struct, IContext<TContext, TWorldState>
    {
        /// <summary>
        /// Runs a single thinking cycle of the agent.
        /// </summary>
        UniTask RunAsync(TContext context);
    }
}