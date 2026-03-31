using System;
using System.Collections.Generic;

namespace Gast.Lib.AI
{
    public class Method<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        public string Name { get; }
        public int Index { get; }

        readonly IReadOnlyList<Func<ITask<TActorContext, TWorldState>>> subTaskProviders;
        ITask<TActorContext, TWorldState>[] resolvedSubTasks;

        public IReadOnlyList<ITask<TActorContext, TWorldState>> SubTasks
        {
            get
            {
                if (resolvedSubTasks != null) return resolvedSubTasks;
                resolvedSubTasks = new ITask<TActorContext, TWorldState>[subTaskProviders.Count];
                for (var i = 0; i < subTaskProviders.Count; i++)
                    resolvedSubTasks[i] = subTaskProviders[i]();
                return resolvedSubTasks;
            }
        }

        readonly Func<TWorldState, bool> startCondition;
        readonly Func<TWorldState, bool> continuationCondition;
        readonly Func<TWorldState, float> scorer;
        readonly Func<TWorldState, float> interruptionCost;

        internal Method(
            string name,
            int index,
            IReadOnlyList<Func<ITask<TActorContext, TWorldState>>> subTaskProviders,
            Func<TWorldState, bool> startCondition,
            Func<TWorldState, bool> continuationCondition,
            Func<TWorldState, float> scorer = null,
            Func<TWorldState, float> interruptionCost = null)
        {
            Name = name;
            Index = index;
            this.subTaskProviders = subTaskProviders;
            this.startCondition = startCondition;
            this.continuationCondition = continuationCondition ?? (_ => true);
            this.scorer = scorer ?? (_ => 0f);
            this.interruptionCost = interruptionCost ?? (_ => 0f);
        }

        public bool CheckStartCondition(TWorldState state)
        {
            return startCondition(state);
        }

        public bool CheckContinuationCondition(TWorldState state)
        {
            return continuationCondition(state);
        }

        public float GetScore(TWorldState state)
        {
            return scorer(state);
        }

        public float GetInterruptionCost(TWorldState state)
        {
            return interruptionCost(state);
        }
    }

    public class CurrentMethodInfo<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        public Method<TActorContext, TWorldState> Method { get; }
        public int NextSubTaskIndex { get; set; }

        public CurrentMethodInfo(Method<TActorContext, TWorldState> method)
        {
            Method = method;
            NextSubTaskIndex = 0;
        }
    }
}
