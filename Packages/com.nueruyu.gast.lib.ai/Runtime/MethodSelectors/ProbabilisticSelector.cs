using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Gast.Lib.AI.MethodSelectors
{
    public class ProbabilisticSelector<TActorContext, TWorldState> : IMethodSelector<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        TWorldState simulationState;

        public async UniTask<Method<TActorContext, TWorldState>> SelectAsync(
            IReadOnlyList<Method<TActorContext, TWorldState>> methods,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            if (context.PlanningStateStore.TryGet<Method<TActorContext, TWorldState>>(this, out var cachedMethod))
                return cachedMethod;

            var candidates = new List<(Method<TActorContext, TWorldState> Method, float Score)>();
            var totalScore = 0f;

            foreach (var method in methods)
            {
                context.WorldState.WriteTo(ref simulationState);
                var validationContext = new ValidationContext<TWorldState>(simulationState, context.PlanningStateStore);

                if (!await ValidateMethod(method, 0, validationContext, cancellationToken))
                    continue;

                var score = Mathf.Max(0f, method.GetScore(context.WorldState));
                candidates.Add((method, score));
                totalScore += score;
            }

            if (candidates.Count == 0)
                return null;

            Method<TActorContext, TWorldState> selectedMethod;
            if (totalScore <= 0f)
            {
                selectedMethod = candidates
                    .OrderByDescending(x => x.Method.GetScore(context.WorldState))
                    .FirstOrDefault().Method;
            }
            else
            {
                var randomValue = Random.Range(0f, totalScore);
                var currentWeight = 0f;
                selectedMethod = candidates.Last().Method;

                foreach (var (method, score) in candidates)
                {
                    currentWeight += score;
                    if (randomValue <= currentWeight)
                    {
                        selectedMethod = method;
                        break;
                    }
                }
            }

            context.PlanningStateStore.Set(this, selectedMethod);
            return selectedMethod;
        }

        public async UniTask<Method<TActorContext, TWorldState>> SelectInterruptsAsync(
            IReadOnlyList<Method<TActorContext, TWorldState>> methods,
            CurrentMethodInfo<TActorContext, TWorldState> currentMethodInfo,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            context.WorldState.WriteTo(ref simulationState);
            var validationContext = new ValidationContext<TWorldState>(simulationState, context.PlanningStateStore);

            var isCurrentMethodStillValid = await ValidateMethod(
                currentMethodInfo.Method,
                currentMethodInfo.NextSubTaskIndex,
                validationContext,
                cancellationToken);

            if (!isCurrentMethodStillValid)
                return await SelectAsync(methods, context, cancellationToken);

            var preferredMethod = await SelectAsync(
                methods,
                context,
                cancellationToken);

            if (preferredMethod == null || preferredMethod == currentMethodInfo.Method)
                return null;

            var currentScore = currentMethodInfo.Method.GetScore(context.WorldState);
            var interruptionCost = currentMethodInfo.Method.GetInterruptionCost(context.WorldState);
            var newScore = preferredMethod.GetScore(context.WorldState);

            if (newScore > currentScore + interruptionCost)
                return preferredMethod;

            return null;
        }

        async UniTask<bool> ValidateMethod(
            Method<TActorContext, TWorldState> method,
            int startIndex,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            if (startIndex == 0)
            {
                if (!method.CheckStartCondition(context.WorldState))
                    return false;
            }

            for (var i = startIndex; i < method.SubTasks.Count; i++)
            {
                if (!method.CheckContinuationCondition(context.WorldState))
                    return false;

                var task = method.SubTasks[i];
                if (!await task.ValidateAsync(context, cancellationToken)) return false;
            }

            return true;
        }
    }
}