using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Gast.Lib.AI
{
    /// <summary>
    /// Internal implementation of IAgentRunner. Manages the simulation state for a single thinking cycle.
    /// </summary>
    class AgentRunner<TWorldState, TContext> : IAgentRunner<TWorldState, TContext>
       where TWorldState : class, IWorldState<TWorldState>, new()
       where TContext : struct, IContext<TWorldState>
    {
        readonly ITask<TWorldState, TContext> rootTask;

        public AgentRunner(ITask<TWorldState, TContext> rootTask)
        {
            this.rootTask = rootTask ?? throw new ArgumentNullException(nameof(rootTask));
        }

        public UniTask RunAsync(TContext context)
        {
            return rootTask.RunAsync(context, CheckOptions.Deep);
        }
    }
}