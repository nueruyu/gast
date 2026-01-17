using System;
using System.Collections.Generic;

namespace Gast.Lib.AI
{
    public class Method<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        public string Name { get; }
        public int Index { get; }
        public Func<TWorldState, bool> Condition { get; }
        public Func<TWorldState, float> Scorer { get; }
        public List<ITask<TWorldState, TContext>> SubTasks { get; } = new List<ITask<TWorldState, TContext>>();

        public Method(
            string name,
            int index,
            Func<TWorldState, bool> condition,
            Func<TWorldState, float> scorer = null)
        {
            Name = name;
            Index = index;
            Condition = condition ?? (_ => true);
            Scorer = scorer ?? (_ => 1.0f);
        }

        public bool CheckCondition(TWorldState state) => Condition(state);

        public float GetScore(TWorldState state) => Scorer(state);
    }
}