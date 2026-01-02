using System.Collections.Generic;

namespace Gast.Lib.AI.Selectors
{
    public class PrioritySelector<T> : IMethodSelector<T> where T : struct
    {
        public bool Select(IReadOnlyList<Method<T>> methods, ref T state, CheckOptions options, ISimulationContext context, out Method<T> selectedMethod)
        {
            selectedMethod = null;

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

                    state = tempState;
                    selectedMethod = method;
                    return true;
                }

                selectedMethod = method;
                return true;
            }

            return false;
        }
    }
}