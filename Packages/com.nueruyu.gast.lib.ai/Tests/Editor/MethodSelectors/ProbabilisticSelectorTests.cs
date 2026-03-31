using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.MethodSelectors;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Gast.Lib.AI.Tests.Editor.MethodSelectors
{
    [TestFixture]
    public class ProbabilisticSelectorTests
    {
        [UnityTest]
        public IEnumerator SelectAsync_WhenTotalScoreIsZero_ShouldReturnMethodWithHighestBaseScore()
        {
            return Run_SelectAsync_WhenTotalScoreIsZero().ToCoroutine();
        }

        async UniTask Run_SelectAsync_WhenTotalScoreIsZero()
        {
            var selector = new ProbabilisticSelector<TestActorContext, TestWorldState>();
            var methods = new TestScenarioBuilder()
                .AddMethod("A", scorer: s => -10f)
                .AddMethod("B", scorer: s => 0f)
                .BuildMethods();

            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());

            var selectedMethod = await selector.SelectAsync(methods, context, CancellationToken.None);

            Assert.IsNotNull(selectedMethod);
            Assert.AreEqual("B", selectedMethod.Name);
        }

        [UnityTest]
        public IEnumerator SelectInterruptsAsync_WhenNewMethodUtilityIsHigher_ShouldReturnNewMethod()
        {
            return Run_SelectInterruptsAsync_WhenNewMethodUtilityIsHigher().ToCoroutine();
        }

        async UniTask Run_SelectInterruptsAsync_WhenNewMethodUtilityIsHigher()
        {
            var selector = new ProbabilisticSelector<TestActorContext, TestWorldState>();

            var (methods, currentMethodInfo) = new TestScenarioBuilder()
                .AddMethod("Current", scorer: s => 50f, interruptionCost: s => 20f)
                .AddMethod("Better", scorer: s => 80f)
                .SetCurrentMethod("Current")
                .Build();

            var store = new PlanningStateStore();
            store.Set(selector, 60f);
            var context = new ValidationContext<TestWorldState>(new TestWorldState(), store);

            var selectedMethod =
                await selector.SelectInterruptsAsync(methods, currentMethodInfo, context, CancellationToken.None);

            Assert.IsNotNull(selectedMethod);
            Assert.AreEqual("Better", selectedMethod.Name);
        }
    }
}
