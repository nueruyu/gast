using Gast.Lib.AI.Tasks;
using System;
using System.Collections.Generic;

namespace Gast.Lib.AI.Builders
{
    public class MethodBuilder<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly string methodName;
        Func<TWorldState, bool> condition = _ => true;
        Func<TWorldState, float> scorer;
        Func<TWorldState, float> interruptionCost;
        readonly List<ITask<TActorContext, TWorldState>> subTasks = new();

        internal MethodBuilder(string methodName)
        {
            this.methodName = methodName;
        }

        public MethodBuilder<TActorContext, TWorldState> Condition(Func<TWorldState, bool> predicate)
        {
            condition = predicate ?? throw new ArgumentNullException(nameof(predicate));
            return this;
        }

        public MethodBuilder<TActorContext, TWorldState> Score(Func<TWorldState, float> scoreFunc)
        {
            scorer = scoreFunc ?? throw new ArgumentNullException(nameof(scoreFunc));
            return this;
        }

        public MethodBuilder<TActorContext, TWorldState> InterruptCost(Func<TWorldState, float> costFunc)
        {
            interruptionCost = costFunc ?? throw new ArgumentNullException(nameof(costFunc));
            return this;
        }

        public MethodBuilder<TActorContext, TWorldState> Do(IAction<TActorContext, TWorldState> action)
        {
            subTasks.Add(new PrimitiveTask<TActorContext, TWorldState>(action));
            return this;
        }

        public MethodBuilder<TActorContext, TWorldState> Do(CompoundTaskBuilder<TActorContext, TWorldState> builder)
        {
            var task = builder.DomainBuilder.GetTask(builder.Name);
            subTasks.Add(task);
            return this;
        }

        internal Method<TActorContext, TWorldState> Build(int index)
        {
            return new Method<TActorContext, TWorldState>(
                methodName,
                index,
                subTasks,
                condition,
                scorer,
                interruptionCost);
        }
    }
}
