using Gast.Lib.AI.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI.Builders
{
    public class AIDomainBuilder<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly Dictionary<string, ITask<TWorldState, TContext>> taskRegistry = new();

        public AIDomainBuilder<TWorldState, TContext> RegisterTask(string name, IAction<TWorldState, TContext> action)
        {
            var task = new PrimitiveTask<TWorldState, TContext>(name, action);
            taskRegistry[task.Name] = task;
            return this;
        }

        public CompoundTaskBuilder<TWorldState, TContext> DefineCompound(string name)
        {
            return new CompoundTaskBuilder<TWorldState, TContext>(this, name);
        }

        internal AIDomainBuilder<TWorldState, TContext> CompleteCompound(
            CompoundTask<TWorldState, TContext> task)
        {
            taskRegistry[task.Name] = task;
            return this;
        }

        internal ITask<TWorldState, TContext> GetTask(string name)
        {
            return taskRegistry[name];
        }

        public AIDomain<TWorldState, TContext> Build(string rootTaskName)
        {
            return new AIDomain<TWorldState, TContext>(
                taskRegistry.Values,
                rootTaskName);
        }
    }
}