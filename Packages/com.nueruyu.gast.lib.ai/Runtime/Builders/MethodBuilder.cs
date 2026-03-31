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
        readonly List<ITask<TActorContext, TWorldState>> subTasks = new();
        readonly List<DeferredTaskRef> deferredRefs = new();
        Func<TWorldState, bool> when = _ => true;
        Func<TWorldState, bool> whileCondition;
        Func<TWorldState, float> interruptionCost;
        Func<TWorldState, float> scorer;

        internal MethodBuilder(string methodName)
        {
            this.methodName = methodName;
        }

        public MethodBuilder<TActorContext, TWorldState> Condition(Func<TWorldState, bool> predicate)
        {
            return When(predicate);
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
            subTasks.Add(new PrimitiveTask<TActorContext, TWorldState>(action));
            return this;
        }

        public MethodBuilder<TActorContext, TWorldState> Do(IAction action)
        {
            subTasks.Add(new PrimitiveTask<TActorContext, TWorldState>(new ActionAdapter(action)));
            return this;
        }

        public MethodBuilder<TActorContext, TWorldState> Do(CompoundTaskBuilder<TActorContext, TWorldState> builder)
        {
            var insertIndex = subTasks.Count;
            subTasks.Add(default); // placeholder
            deferredRefs.Add(new DeferredTaskRef(builder.DomainBuilder, builder.Name, insertIndex));
            return this;
        }

        internal Method<TActorContext, TWorldState> Build(int index)
        {
            foreach (var deferred in deferredRefs)
                subTasks[deferred.Index] = new LazyTask<TActorContext, TWorldState>(
                    () => deferred.DomainBuilder.GetTask(deferred.TaskName));

            return new Method<TActorContext, TWorldState>(
                methodName,
                index,
                subTasks,
                when,
                whileCondition,
                scorer,
                interruptionCost);
        }

        readonly struct DeferredTaskRef
        {
            public AIDomainBuilder<TActorContext, TWorldState> DomainBuilder { get; }
            public string TaskName { get; }
            public int Index { get; }

            public DeferredTaskRef(
                AIDomainBuilder<TActorContext, TWorldState> domainBuilder,
                string taskName,
                int index)
            {
                DomainBuilder = domainBuilder;
                TaskName = taskName;
                Index = index;
            }
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