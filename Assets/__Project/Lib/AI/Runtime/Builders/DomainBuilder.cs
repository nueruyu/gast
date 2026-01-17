using System;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI.Builders
{
    public class DomainBuilder<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly Dictionary<string, ITask<TWorldState, TContext>> taskRegistry = new();
        string rootTaskName;

        public DomainBuilder<TWorldState, TContext> RegisterTask(ITask<TWorldState, TContext> task)
        {
            taskRegistry[task.Name] = task;
            return this;
        }

        public DomainBuilder<TWorldState, TContext> SetRoot(string taskName)
        {
            rootTaskName = taskName;
            return this;
        }

        public CompoundTaskBuilder<TWorldState, TContext> DefineCompound(string name)
        {
            return new CompoundTaskBuilder<TWorldState, TContext>(this, name);
        }

        internal DomainBuilder<TWorldState, TContext> CompleteCompound(
            CompoundTask<TWorldState, TContext> task)
        {
            RegisterTask(task);
            return this;
        }

        internal ITask<TWorldState, TContext> GetTask(string name)
        {
            return taskRegistry[name];
        }

        public Domain<TWorldState, TContext> Build()
        {
            return new Domain<TWorldState, TContext>(
                taskRegistry.Values,
                rootTaskName);
        }
    }
}