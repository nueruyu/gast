using System;
using System.Collections.Generic;

namespace DescrioGames.Lib.AI.Selectors
{
    public class SimulationSelector<T> : IMethodSelector<T> where T : struct
    {
        readonly Func<T, float> worldEvaluator;
        readonly IEnvironmentModel<T> envModel;

        public SimulationSelector(Func<T, float> worldEvaluator, IEnvironmentModel<T> envModel = null)
        {
            this.worldEvaluator = worldEvaluator;
            this.envModel = envModel;
        }

        public bool Select(IReadOnlyList<Method<T>> methods, ref T state, CheckOptions options, ISimulationContext context, out Method<T> selectedMethod)
        {
            selectedMethod = null;
            Method<T> bestMethod = null;
            float bestOutcomeScore = float.NegativeInfinity;
            T bestState = state;
            var simOptions = CheckOptions.Deep;

            foreach (var method in methods)
            {
                if (!method.CheckCondition(state)) continue;

                var futureState = state;
                if (TrySimulateMethod(method, ref futureState, simOptions, context))
                {
                    float outcomeScore = worldEvaluator(futureState);
                    if (outcomeScore > bestOutcomeScore)
                    {
                        bestOutcomeScore = outcomeScore;
                        bestMethod = method;
                        bestState = futureState;
                    }
                }
            }

            if (bestMethod != null)
            {
                state = bestState;
                selectedMethod = bestMethod;
                return true;
            }

            return false;
        }

        bool TrySimulateMethod(Method<T> method, ref T state, CheckOptions options, ISimulationContext context)
        {
            var nextOptions = options.StepDown();
            foreach (var subTask in method.SubTasks)
            {
                if (!subTask.Validate(ref state, nextOptions, context, envModel))
                {
                    return false;
                }
            }
            return true;
        }
    }
}