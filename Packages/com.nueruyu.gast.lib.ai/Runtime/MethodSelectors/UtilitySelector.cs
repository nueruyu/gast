using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Gast.Lib.AI.MethodSelectors
{
    public class UtilitySelector<TActorContext, TWorldState> : IMethodSelector<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly TWorldState simulationState = new();

        public async UniTask<Method<TActorContext, TWorldState>> SelectAsync(
            IReadOnlyList<Method<TActorContext, TWorldState>> methods,
            TWorldState worldState,
            CancellationToken cancellationToken)
        {
            var candidates = new List<(Method<TActorContext, TWorldState> Method, float Score)>();
            var totalScore = 0f;

            // 1. Validate methods and calculate scores
            foreach (var method in methods)
            {
                simulationState.CopyFrom(worldState);

                if (!await ValidateMethod(method, simulationState, cancellationToken))
                {
                    continue;
                }

                // Treat negative scores as zero for probability calculation
                var score = Mathf.Max(0f, method.GetScore(worldState));
                candidates.Add((method, score));
                totalScore += score;
            }

            if (candidates.Count == 0)
            {
                return null;
            }

            // 2. Fallback if total score is zero (all zero or negative)
            // In this case, choose the one with the highest raw score to ensure somewhat rational behavior.
            if (totalScore <= 0f)
            {
                return candidates
                    .OrderByDescending(x => x.Method.GetScore(worldState))
                    .FirstOrDefault().Method;
            }

            // 3. Weighted Random Selection (Roulette Wheel Selection)
            var randomValue = Random.Range(0f, totalScore);
            var currentWeight = 0f;

            foreach (var (method, score) in candidates)
            {
                currentWeight += score;
                if (randomValue <= currentWeight)
                {
                    return method;
                }
            }

            // Should not happen, but return the last one just in case
            return candidates.Last().Method;
        }

        public async UniTask<Method<TActorContext, TWorldState>> SelectInterruptsAsync(
            IReadOnlyList<Method<TActorContext, TWorldState>> methods,
            Method<TActorContext, TWorldState> currentMethod,
            TWorldState worldState,
            CancellationToken cancellationToken)
        {
            // Select a candidate using the same probabilistic logic
            var preferredMethod = await SelectAsync(
                methods,
                worldState,
                cancellationToken);

            if (preferredMethod == null || preferredMethod == currentMethod)
                return null;

            var currentScore = currentMethod.GetScore(worldState);
            var interruptionCost = currentMethod.GetInterruptionCost(worldState);
            var newScore = preferredMethod.GetScore(worldState);

            // Switch only if the new score exceeds the current score plus the cost to interrupt
            // Note: Since SelectAsync is probabilistic, a lower-score method might be selected as 'preferredMethod'.
            // However, this check prevents switching to a lower-score method, ensuring stability.
            if (newScore > currentScore + interruptionCost)
            {
                return preferredMethod;
            }

            return null;
        }

        async UniTask<bool> ValidateMethod(
           Method<TActorContext, TWorldState> method,
           TWorldState worldState,
           CancellationToken cancellationToken)
        {
            if (!method.CheckCondition(worldState))
                return false;

            foreach (var task in method.SubTasks)
            {
                if (!await task.ValidateAsync(worldState, cancellationToken))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
