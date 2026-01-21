using Gast.Lib.AI.Tasks;
using System;
using System.Collections.Generic;

namespace Gast.Lib.AI.Builders
{
    public class MethodBuilder<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly CompoundTaskBuilder<TWorldState, TContext> compoundBuilder;
        readonly string methodName;
        Func<TWorldState, bool> condition = _ => true;
        Func<TWorldState, float> scorer = null;
        readonly List<ITask<TWorldState, TContext>> subTasks = new();

        internal MethodBuilder(
            CompoundTaskBuilder<TWorldState, TContext> compoundBuilder,
            string methodName)
        {
            this.compoundBuilder = compoundBuilder;
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

        public MethodBuilder<TWorldState, TContext> Do(params string[] taskNames)
        {
            foreach (var name in taskNames)
            {
                var task = compoundBuilder.DomainBuilder.GetTask(name);
                if (task == null)
                {
                    throw new InvalidOperationException($"Task '{name}' is not registered or is a parametric action.");
                }
                subTasks.Add(task);
            }
            return this;
        }

        public MethodBuilder<TWorldState, TContext> Do<TParam>(string actionName, TParam param)
        {
            var action = compoundBuilder.DomainBuilder.GetAction<TParam>(actionName);
            if (action == null)
            {
                throw new InvalidOperationException($"Parametric action '{actionName}' for parameter type '{typeof(TParam).Name}' is not registered.");
            }
            var task = new ParametricPrimitiveTask<TWorldState, TContext, TParam>(actionName, action, param);
            subTasks.Add(task);
            return this;
        }

        public CompoundTaskBuilder<TWorldState, TContext> End()
        {
            var method = new Method<TWorldState, TContext>(
                methodName,
                compoundBuilder.CurrentMethodCount,
                subTasks,
                condition,
                scorer);

            return compoundBuilder.CompleteMethod(method);
        }
    }
}