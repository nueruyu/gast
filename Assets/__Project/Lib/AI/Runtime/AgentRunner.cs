using Cysharp.Threading.Tasks;
using System;

namespace Gast.Lib.AI
{
    public class AgentRunner<TWorldState, TContext>
       where TWorldState : class, IWorldState<TWorldState>, new()
       where TContext : struct, IContext<TContext, TWorldState>
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