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

        public ITask<TWorldState, TContext> RootTask { get; private set; }

        public void RegisterTask(ITask<TWorldState, TContext> task)
        {
            tasks[task.Name] = task;
        }

        public void SetRootTask(ITask<TWorldState, TContext> root)
        {
            RootTask = root;
        }

        public ITask<TWorldState, TContext> GetTask(string name)
        {
            return tasks.TryGetValue(name, out var task) ? task : null;
        }

        public IAgentRunner<TWorldState, TContext> CreateAgentRunner()
        {
            return new AgentRunner<TWorldState, TContext>(RootTask);
        }
    }
}