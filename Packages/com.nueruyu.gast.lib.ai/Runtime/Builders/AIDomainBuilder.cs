using Gast.Lib.AI.Tasks;
using System;
using System.Collections.Generic;

namespace Gast.Lib.AI.Builders
{
    public class AIDomainBuilder<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly Dictionary<string, CompoundTaskBuilder<TWorldState, TContext>> builders = new();
        readonly Dictionary<string, ITask<TWorldState, TContext>> builtTasks = new();

        public CompoundTaskBuilder<TWorldState, TContext> DefineCompound(string name)
        {
            var builder = new CompoundTaskBuilder<TWorldState, TContext>(this, name);
            builders[name] = builder;
            return builder;
        }

        internal ITask<TWorldState, TContext> GetTask(string name)
        {
            if (builtTasks.TryGetValue(name, out var task))
            {
                return task;
            }

            if (builders.TryGetValue(name, out var builder))
            {
                var builtTask = builder.Build();
                builtTasks[name] = builtTask;
                return builtTask;
            }

            throw new InvalidOperationException($"Task '{name}' is not defined.");
        }

        public AIDomain<TWorldState, TContext> Build(string rootTaskName)
        {
            foreach (var name in builders.Keys)
            {
                GetTask(name);
            }
            return new AIDomain<TWorldState, TContext>(builtTasks.Values, rootTaskName);
        }
    }
}
