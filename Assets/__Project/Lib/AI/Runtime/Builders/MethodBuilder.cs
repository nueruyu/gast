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
                var item = compoundBuilder.DomainBuilder.GetRegisteredItem(name);
                if (item == null)
                {
                    throw new InvalidOperationException($"'{name}' is not registered.");
                }

                if (item is ITask<TWorldState, TContext> task)
                {
                    subTasks.Add(task);
                }
                else if (item is IAction<TWorldState, TContext> action)
                {
                    subTasks.Add(new PrimitiveTask<TWorldState, TContext>(name, action));
                }
                else
                {
                    throw new InvalidOperationException($"'{name}' requires parameters. Use Do(\"{name}\", param) instead.");
                }
            }
            return this;
        }

        public MethodBuilder<TWorldState, TContext> Do<TParam>(string taskName, TParam param)
        {
            var item = compoundBuilder.DomainBuilder.GetRegisteredItem(taskName);
            if (item == null)
            {
                throw new InvalidOperationException($"'{taskName}' is not registered.");
            }

            if (item is IAction<TWorldState, TContext, TParam> action)
            {
                subTasks.Add(new ParametricPrimitiveTask<TWorldState, TContext, TParam>(taskName, action, param));
            }
            else
            {
                throw new InvalidOperationException($"'{taskName}' is not registered as a parametric action with type '{typeof(TParam).Name}'.");
            }
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