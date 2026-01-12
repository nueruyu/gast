using System;
using System.Collections.Generic;

namespace Gast.Lib.AI.Builders
{
    public class MethodBuilder<TWorldState> where TWorldState : struct
    {
        readonly CompoundTaskBuilder<TWorldState> compoundBuilder;
        readonly string methodName;
        Func<TWorldState, bool> condition = _ => true;
        Func<TWorldState, float> scorer = null;
        readonly List<ITask<TWorldState>> subTasks = new();

        internal MethodBuilder(
            CompoundTaskBuilder<TWorldState> compoundBuilder,
            string methodName)
        {
            this.compoundBuilder = compoundBuilder;
            this.methodName = methodName;
        }

        public MethodBuilder<TWorldState> Condition(Func<TWorldState, bool> predicate)
        {
            condition = predicate;
            return this;
        }

        public MethodBuilder<TWorldState> Score(Func<TWorldState, float> scoreFunc)
        {
            scorer = scoreFunc;
            return this;
        }

        public MethodBuilder<TWorldState> Do(params string[] taskNames)
        {
            foreach (var name in taskNames)
            {
                var task = compoundBuilder.DomainBuilder.GetTask(name);
                if (task != null)
                    subTasks.Add(task);
            }
            return this;
        }

        public MethodBuilder<TWorldState> Do(params ITask<TWorldState>[] tasks)
        {
            subTasks.AddRange(tasks);
            return this;
        }

        public CompoundTaskBuilder<TWorldState> End()
        {
            var method = new Method<TWorldState>(
                methodName,
                compoundBuilder.CurrentMethodCount,
                condition,
                scorer);
            method.SubTasks.AddRange(subTasks);
            return compoundBuilder.CompleteMethod(method);
        }
    }
}