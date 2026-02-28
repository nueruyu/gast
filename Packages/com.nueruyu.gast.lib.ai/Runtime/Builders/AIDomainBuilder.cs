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
            registry[name] = action;
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

        internal object GetRegisteredItem(string name)
        {
            registry.TryGetValue(name, out var item);
            return item;
        }

        public AIDomain<TWorldState, TContext> Build(string rootTaskName)
        {
            return new AIDomain<TWorldState, TContext>(
                registry.Values.OfType<ITask<TWorldState, TContext>>(),
                rootTaskName);
        }
    }
}