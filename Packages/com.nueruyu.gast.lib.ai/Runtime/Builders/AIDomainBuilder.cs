using Gast.Lib.AI.Tasks;
using System;
using System.Collections.Generic;

namespace Gast.Lib.AI.Builders
{
    public class AIDomainBuilder<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly Dictionary<string, CompoundTaskBuilder<TActorContext, TWorldState>> builders = new();
        readonly Dictionary<string, ITask<TActorContext, TWorldState>> builtTasks = new();

        public CompoundTaskBuilder<TActorContext, TWorldState> DefineCompound(string name)
        {
            var builder = new CompoundTaskBuilder<TActorContext, TWorldState>(this, name);
            builders[name] = builder;
            return builder;
        }

        internal ITask<TActorContext, TWorldState> GetTask(string name)
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

        public AIDomain<TActorContext, TWorldState> Build(string rootTaskName)
        {
            foreach (var name in builders.Keys)
            {
                GetTask(name);
            }
            return new AIDomain<TActorContext, TWorldState>(builtTasks.Values, rootTaskName);
        }
    }
}
