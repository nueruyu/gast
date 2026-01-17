using System;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI
{
    public class Domain<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly Dictionary<string, ITask<TWorldState, TContext>> tasks = new();

        public ITask<TWorldState, TContext> RootTask { get; }

        public Domain(
            IEnumerable<ITask<TWorldState, TContext>> tasks,
            string rootTaskName)
        {
            foreach (var task in tasks)
            {
                this.tasks[task.Name] = task;
            }

            RootTask = this.tasks[rootTaskName];
        }

        public AgentRunner<TWorldState, TContext> CreateAgentRunner()
        {
            return new AgentRunner<TWorldState, TContext>(RootTask);
        }
    }
}