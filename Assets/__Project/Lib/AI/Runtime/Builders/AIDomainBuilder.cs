using Gast.Lib.AI.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gast.Lib.AI.Builders
{
    public class AIDomainBuilder<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly Dictionary<string, object> registry = new();

        public AIDomainBuilder<TWorldState, TContext> RegisterAction(string name, IAction<TWorldState, TContext> action)
        {
            var task = new PrimitiveTask<TWorldState, TContext>(name, action);
            registry[task.Name] = task;
            return this;
        }

        public AIDomainBuilder<TWorldState, TContext> RegisterAction<TParam>(string name, IAction<TWorldState, TContext, TParam> action)
        {
            registry[name] = action;
            return this;
        }

        public CompoundTaskBuilder<TWorldState, TContext> DefineCompound(string name)
        {
            return new CompoundTaskBuilder<TWorldState, TContext>(this, name);
        }

        internal AIDomainBuilder<TWorldState, TContext> CompleteCompound(
            CompoundTask<TWorldState, TContext> task)
        {
            registry[task.Name] = task;
            return this;
        }

        internal ITask<TWorldState, TContext> GetTask(string name)
        {
            if (registry.TryGetValue(name, out var item) && item is ITask<TWorldState, TContext> task)
            {
                return task;
            }
            return null;
        }

        internal IAction<TWorldState, TContext, TParam> GetAction<TParam>(string name)
        {
            if (registry.TryGetValue(name, out var item) && item is IAction<TWorldState, TContext, TParam> action)
            {
                return action;
            }
            return null;
        }

        public AIDomain<TWorldState, TContext> Build(string rootTaskName)
        {
            return new AIDomain<TWorldState, TContext>(
                registry.Values.OfType<ITask<TWorldState, TContext>>(),
                rootTaskName);
        }
    }
}