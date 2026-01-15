using System;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI.Builders
{
    public class DomainBuilder<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TWorldState>
    {
        readonly Dictionary<string, ITask<TWorldState, TContext>> taskRegistry = new();
        ITask<TWorldState, TContext> rootTask;

        public DomainBuilder<TWorldState, TContext> RegisterTask(ITask<TWorldState, TContext> task)
        {
            taskRegistry[task.Name] = task;
            return this;
        }

        public CompoundTaskBuilder<TWorldState, TContext> DefineCompound(string name)
        {
            return new CompoundTaskBuilder<TWorldState, TContext>(this, name);
        }

        public CompoundTaskBuilder<TWorldState, TContext> DefineRoot()
        {
            return new CompoundTaskBuilder<TWorldState, TContext>(this, taskName: "Root", isRoot: true);
        }

        internal DomainBuilder<TWorldState, TContext> CompleteCompound(
            string name,
            CompoundTask<TWorldState, TContext> task,
            bool isRoot)
        {
            RegisterTask(task);
            if (isRoot)
                rootTask = task;
            return this;
        }

        internal ITask<TWorldState, TContext> GetTask(string name)
        {
            return taskRegistry.TryGetValue(name, out var task) ? task : null;
        }

        public Domain<TWorldState, TContext> Build()
        {
            var domain = new Domain<TWorldState, TContext>();
            foreach (var task in taskRegistry.Values)
            {
                domain.RegisterTask(task);
            }
            if (rootTask != null)
            {
                domain.SetRootTask(rootTask);
            }
            return domain;
        }
    }
}