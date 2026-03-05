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

        public PrimitiveTaskToken RegisterAction(string name, IAction<TWorldState, TContext> action)
        {
            registry[name] = action;
            return new PrimitiveTaskToken(name);
        }

        public ParametricTaskToken<TParam> RegisterAction<TParam>(string name, IAction<TWorldState, TContext, TParam> action)
        {
            registry[name] = action;
            return new ParametricTaskToken<TParam>(name);
        }

        public CompoundTaskToken DefineCompound(string name, Action<CompoundTaskBuilder<TWorldState, TContext>> buildAction)
        {
            var compoundBuilder = new CompoundTaskBuilder<TWorldState, TContext>(this, name);
            buildAction(compoundBuilder);
            var compoundTask = compoundBuilder.Build();
            registry[name] = compoundTask;
            return new CompoundTaskToken(name);
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
