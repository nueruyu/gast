using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.MethodSelectors;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Gast.Lib.AI.Tests.Editor.MethodSelectors
{
    [TestFixture]
    public class PrioritySelectorTests
    {
        [UnityTest]
        public IEnumerator SelectAsync_ShouldReturnFirstValidMethodByIndex()
        {
            return Run_SelectAsync_ShouldReturnFirstValidMethodByIndex().ToCoroutine();
        }

        async UniTask Run_SelectAsync_ShouldReturnFirstValidMethodByIndex()
        {
            var selector = new PrioritySelector<TestActorContext, TestWorldState>();

            var methods = new TestScenarioBuilder()
                .AddMethod("Invalid", startCondition: s => false)
                .AddMethod("Valid")
                .AddMethod("AnotherValid")
                .BuildMethods();

            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());

            var selectedMethod = await selector.SelectAsync(methods, context, CancellationToken.None);

            Assert.IsNotNull(selectedMethod);
            Assert.AreEqual("Valid", selectedMethod.Name);
        }

        [UnityTest]
        public IEnumerator SelectInterruptsAsync_WhenHigherPriorityMethodIsValid_ShouldReturnNewMethod()
        {
            return Run_SelectInterruptsAsync_WhenHigherPriorityMethodIsValid().ToCoroutine();
        }

        async UniTask Run_SelectInterruptsAsync_WhenHigherPriorityMethodIsValid()
        {
            var selector = new PrioritySelector<TestActorContext, TestWorldState>();

            var (methods, currentMethodInfo) = new TestScenarioBuilder()
                .AddMethod("Higher")
                .AddMethod("Current")
                .SetCurrentMethod("Current")
                .Build();

            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());

            var selectedMethod =
                await selector.SelectInterruptsAsync(methods, currentMethodInfo, context, CancellationToken.None);

            Assert.IsNotNull(selectedMethod);
            Assert.AreEqual("Higher", selectedMethod.Name);
        }

        [UnityTest]
        public IEnumerator SelectInterruptsAsync_WhenOnlyLowerPriorityMethodIsValid_ShouldReturnNull()
        {
            return Run_SelectInterruptsAsync_WhenOnlyLowerPriorityMethodIsValid().ToCoroutine();
        }

        async UniTask Run_SelectInterruptsAsync_WhenOnlyLowerPriorityMethodIsValid()
        {
            var selector = new PrioritySelector<TestActorContext, TestWorldState>();

            var (methods, currentMethodInfo) = new TestScenarioBuilder()
                .AddMethod("Current")
                .AddMethod("Lower")
                .SetCurrentMethod("Current")
                .Build();

            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());

            var selectedMethod =
                await selector.SelectInterruptsAsync(methods, currentMethodInfo, context, CancellationToken.None);

            Assert.IsNull(selectedMethod);
        }
    }
}
