using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gast.Lib.AI
{
    public class Method<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        public string Name { get; }
        public int Index { get; }
        public IReadOnlyList<ITask<TActorContext, TWorldState>> SubTasks { get; }

        readonly Func<TWorldState, bool> startCondition;
        readonly Func<TWorldState, bool> continuationCondition;
        readonly Func<TWorldState, float> scorer;
        readonly Func<TWorldState, float> interruptionCost;

        internal Method(
            string name,
            int index,
            IEnumerable<ITask<TActorContext, TWorldState>> subTasks,
            Func<TWorldState, bool> startCondition,
            Func<TWorldState, bool> continuationCondition,
            Func<TWorldState, float> scorer = null,
            Func<TWorldState, float> interruptionCost = null)
        {
            Name = name;
            Index = index;
            SubTasks = subTasks.ToArray();
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
