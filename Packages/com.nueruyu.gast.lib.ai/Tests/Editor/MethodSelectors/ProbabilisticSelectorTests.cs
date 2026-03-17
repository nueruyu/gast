using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.MethodSelectors;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Gast.Lib.AI.Tests.MethodSelectors
{
    [TestFixture]
    public class ProbabilisticSelectorTests
    {
        [UnityTest]
        public System.Collections.IEnumerator SelectAsync_WhenTotalScoreIsZero_ShouldReturnMethodWithHighestBaseScore()
        {
            return Run_SelectAsync_WhenTotalScoreIsZero().ToCoroutine();
        }

        async UniTask Run_SelectAsync_WhenTotalScoreIsZero()
        {
            var selector = new ProbabilisticSelector<TestActorContext, TestWorldState>();

            // Method constructor: (name, index, subTasks, startCondition, continuationCondition, scorer, interruptionCost)
            // Mathf.Max(0f, -10f) = 0, Mathf.Max(0f, 0f) = 0 → totalScore = 0
            // Fallback: OrderByDescending(GetScore) → B (0f) > A (-10f) → selects B
            var methodA = new Method<TestActorContext, TestWorldState>(
                "A", 0,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null,
                s => -10f);
            var methodB = new Method<TestActorContext, TestWorldState>(
                "B", 1,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null,
                s => 0f);

            var methods = new List<Method<TestActorContext, TestWorldState>> { methodA, methodB };
            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());

            var selectedMethod = await selector.SelectAsync(methods, context, CancellationToken.None);

            Assert.IsNotNull(selectedMethod);
            Assert.AreEqual("B", selectedMethod.Name);
        }

        [UnityTest]
        public System.Collections.IEnumerator SelectInterruptsAsync_WhenNewMethodUtilityIsHigher_ShouldReturnNewMethod()
        {
            return Run_SelectInterruptsAsync_WhenNewMethodUtilityIsHigher().ToCoroutine();
        }

        async UniTask Run_SelectInterruptsAsync_WhenNewMethodUtilityIsHigher()
        {
            // Pre-seed the planning state store with the selector as key so that SelectAsync
            // deterministically picks betterMethod instead of relying on Random.Range.
            // candidates in order: [(currentMethod, 50f), (betterMethod, 80f)], totalScore=130f
            // randomValue=60f: 60 > 50 (currentMethod weight) → selects betterMethod
            // Interrupt check: 80 > 50 + 20 = 80 > 70 → true → returns betterMethod
            var store = new PlanningStateStore();
            var selector = new ProbabilisticSelector<TestActorContext, TestWorldState>();
            store.Set(selector, 60f);

            var currentMethod = new Method<TestActorContext, TestWorldState>(
                "Current", 0,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null,
                s => 50f,
                s => 20f);
            var betterMethod = new Method<TestActorContext, TestWorldState>(
                "Better", 1,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null,
                s => 80f);

            var methods = new List<Method<TestActorContext, TestWorldState>> { currentMethod, betterMethod };
            var context = new ValidationContext<TestWorldState>(new TestWorldState(), store);
            var currentMethodInfo = new CurrentMethodInfo<TestActorContext, TestWorldState>(currentMethod);

            var selectedMethod = await selector.SelectInterruptsAsync(methods, currentMethodInfo, context, CancellationToken.None);

            Assert.IsNotNull(selectedMethod);
            Assert.AreEqual("Better", selectedMethod.Name);
        }
    }
}
