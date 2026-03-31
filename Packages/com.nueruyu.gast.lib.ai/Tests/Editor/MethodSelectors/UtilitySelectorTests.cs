using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.MethodSelectors;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Gast.Lib.AI.Tests.Editor.MethodSelectors
{
    [TestFixture]
    public class UtilitySelectorTests
    {
        [UnityTest]
        public IEnumerator SelectAsync_ShouldReturnMethodWithHighestScore()
        {
            return Run_SelectAsync_ShouldReturnMethodWithHighestScore().ToCoroutine();
        }

        async UniTask Run_SelectAsync_ShouldReturnMethodWithHighestScore()
        {
            var selector = new UtilitySelector<TestActorContext, TestWorldState>();
            var methods = new TestScenarioBuilder()
                .AddMethod("Low", scorer: s => 10f)
                .AddMethod("Mid", scorer: s => 50f)
                .AddMethod("High", scorer: s => 100f)
                .AddMethod("Invalid", startCondition: s => false, scorer: s => 200f)
                .BuildMethods();

            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());

            var selectedMethod = await selector.SelectAsync(methods, context, CancellationToken.None);

            Assert.IsNotNull(selectedMethod);
            Assert.AreEqual("High", selectedMethod.Name);
        }

        [UnityTest]
        public IEnumerator SelectInterruptsAsync_WhenNewMethodIsBetter_ShouldReturnNewMethod()
        {
            return Run_SelectInterruptsAsync_WhenNewMethodIsBetter().ToCoroutine();
        }

        async UniTask Run_SelectInterruptsAsync_WhenNewMethodIsBetter()
        {
            var selector = new UtilitySelector<TestActorContext, TestWorldState>();

            var (methods, currentMethodInfo) = new TestScenarioBuilder()
                .AddMethod("Current", scorer: s => 50f, interruptionCost: s => 20f)
                .AddMethod("Better", scorer: s => 80f)
                .SetCurrentMethod("Current")
                .Build();

            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());

            var selectedMethod =
                await selector.SelectInterruptsAsync(methods, currentMethodInfo, context, CancellationToken.None);

            Assert.IsNotNull(selectedMethod);
            Assert.AreEqual("Better", selectedMethod.Name);
        }

        [UnityTest]
        public IEnumerator SelectInterruptsAsync_WhenNewMethodIsNotGoodEnough_ShouldReturnNull()
        {
            return Run_SelectInterruptsAsync_WhenNewMethodIsNotGoodEnough().ToCoroutine();
        }

        async UniTask Run_SelectInterruptsAsync_WhenNewMethodIsNotGoodEnough()
        {
            var selector = new UtilitySelector<TestActorContext, TestWorldState>();

            var (methods, currentMethodInfo) = new TestScenarioBuilder()
                .AddMethod("Current", scorer: s => 50f, interruptionCost: s => 20f)
                .AddMethod("NotGoodEnough", scorer: s => 60f)
                .SetCurrentMethod("Current")
                .Build();

            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());

            var selectedMethod =
                await selector.SelectInterruptsAsync(methods, currentMethodInfo, context, CancellationToken.None);

            Assert.IsNull(selectedMethod);
        }
    }
}
