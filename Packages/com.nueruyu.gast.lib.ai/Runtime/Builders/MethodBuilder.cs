using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Tasks;

namespace Gast.Lib.AI.Builders
{
    public class MethodBuilder<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly string methodName;
        readonly List<Func<ITask<TActorContext, TWorldState>>> subTaskProviders = new();
        Func<TWorldState, float> interruptionCost;
        Func<TWorldState, float> scorer;
        Func<TWorldState, bool> when = _ => true;
        Func<TWorldState, bool> whileCondition;

        internal MethodBuilder(string methodName)
        {
            this.methodName = methodName;
        }

        public MethodBuilder<TActorContext, TWorldState> When(Func<TWorldState, bool> predicate)
        {
            when = predicate ?? throw new ArgumentNullException(nameof(predicate));
            return this;
        }

        public MethodBuilder<TActorContext, TWorldState> While(Func<TWorldState, bool> predicate)
        {
            whileCondition = predicate ?? throw new ArgumentNullException(nameof(predicate));
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
            subTaskProviders.Add(() => new PrimitiveTask<TActorContext, TWorldState>(action));
            return this;
        }

        public MethodBuilder<TActorContext, TWorldState> Do(IAction action)
        {
            subTaskProviders.Add(() => new PrimitiveTask<TActorContext, TWorldState>(new ActionAdapter(action)));
            return this;
        }

        public MethodBuilder<TActorContext, TWorldState> Do(CompoundTaskBuilder<TActorContext, TWorldState> builder)
        {
            var domainBuilder = builder.DomainBuilder;
            var taskName = builder.Name;
            subTaskProviders.Add(() => domainBuilder.GetTask(taskName));
            return this;
        }

        internal Method<TActorContext, TWorldState> Build(int index)
        {
            return new Method<TActorContext, TWorldState>(
                methodName,
                index,
                subTaskProviders,
                when,
                whileCondition,
                scorer,
                interruptionCost);
        }

        class ActionAdapter : IAction<TActorContext, TWorldState>
        {
            readonly IAction action;

            public ActionAdapter(IAction action)
            {
                this.action = action;
            }

            public bool IsAvailable(TWorldState worldState)
            {
                return true;
            }

            public void Simulate(TWorldState worldState)
            {
            }

            public UniTask ExecuteAsync(
                TActorContext context,
                CancellationToken cancellationToken)
            {
                return action.ExecuteAsync(cancellationToken);
            }

            public override string ToString()
            {
                return action.ToString();
            }
        }
    }
}