using System.Collections.Generic;

namespace Gast.Lib.AI.Selectors
{
    public class UtilitySelector<T> : IMethodSelector<T> where T : struct
    {
        public bool Select(IReadOnlyList<Method<T>> methods, ref T state, CheckOptions options, ISimulationContext context, out Method<T> selectedMethod)
        {
            selectedMethod = null;
            Method<T> bestMethod = null;
            float bestScore = float.NegativeInfinity;
            T bestState = state;

            foreach (var method in methods)
            {
                if (!method.CheckCondition(state)) continue;

                if (options.MaxDepth != 0)
                {
                    var tempState = state;
                    var nextOptions = options.StepDown();
                    bool valid = true;

                    foreach (var task in method.SubTasks)
                    {
                        if (!task.Validate(ref tempState, nextOptions, context, null))
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (!valid) continue;

                    float score = method.GetScore(tempState);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMethod = method;
                        bestState = tempState;
                    }
                }
                else
                {
                    float score = method.GetScore(state);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMethod = method;
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
    }
}