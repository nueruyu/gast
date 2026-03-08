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

        readonly Func<TWorldState, bool> condition;
        readonly Func<TWorldState, float> scorer;
        readonly Func<TWorldState, float> interruptionCost;

        internal Method(
            string name,
            int index,
            IEnumerable<ITask<TActorContext, TWorldState>> subTasks,
            Func<TWorldState, bool> condition,
            Func<TWorldState, float> scorer = null,
            Func<TWorldState, float> interruptionCost = null)
        {
            Name = name;
            Index = index;
            SubTasks = subTasks.ToArray();
            this.condition = condition;
            this.scorer = scorer ?? (_ => 0f);
            this.interruptionCost = interruptionCost ?? (_ => 0f);
        }

        public bool CheckCondition(TWorldState state)
        {
            return condition(state);
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
}
