using Gast.Lib.AI.Tasks;
using System;
using System.Collections.Generic;

namespace Gast.Lib.AI.Builders
{
    public class MethodBuilder<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly string methodName;
        Func<TWorldState, bool> condition = _ => true;
        Func<TWorldState, float> scorer;
        Func<TWorldState, float> interruptionCost;
        readonly List<ITask<TWorldState, TContext>> subTasks = new();

        internal MethodBuilder(string methodName)
        {
            this.methodName = methodName;
        }

        public MethodBuilder<TWorldState, TContext> Condition(Func<TWorldState, bool> predicate)
        {
            condition = predicate ?? throw new ArgumentNullException(nameof(predicate));
            return this;
        }

        public MethodBuilder<TWorldState, TContext> Score(Func<TWorldState, float> scoreFunc)
        {
            scorer = scoreFunc ?? throw new ArgumentNullException(nameof(scoreFunc));
            return this;
        }

        public MethodBuilder<TWorldState, TContext> InterruptCost(Func<TWorldState, float> costFunc)
        {
            interruptionCost = costFunc ?? throw new ArgumentNullException(nameof(costFunc));
            return this;
        }

        public MethodBuilder<TWorldState, TContext> Do(IAction<TWorldState, TContext> action)
        {
            subTasks.Add(new PrimitiveTask<TWorldState, TContext>(action));
            return this;
        }

        public MethodBuilder<TWorldState, TContext> Do(CompoundTaskBuilder<TWorldState, TContext> builder)
        {
            var task = builder.DomainBuilder.GetTask(builder.Name);
            subTasks.Add(task);
            return this;
        }

        internal Method<TWorldState, TContext> Build(int index)
        {
            return new Method<TWorldState, TContext>(
                methodName,
                index,
                subTasks,
                condition,
                scorer,
                interruptionCost);
        }
    }
}
