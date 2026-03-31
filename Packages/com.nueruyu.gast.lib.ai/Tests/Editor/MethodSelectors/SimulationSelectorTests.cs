using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.MethodSelectors;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Gast.Lib.AI.Tests.Editor.MethodSelectors
{
    [TestFixture]
    public class SimulationSelectorTests
    {
        [UnityTest]
        public IEnumerator SelectAsync_ShouldReturnMethodWithBestOutcome()
        {
            return Run_SelectAsync_ShouldReturnMethodWithBestOutcome().ToCoroutine();
        }

        async UniTask Run_SelectAsync_ShouldReturnMethodWithBestOutcome()
        {
            var envModel = new TestEnvironmentModel();
            var selector = new SimulationSelector<TestActorContext, TestWorldState>(s => s.Value, envModel);
            var methods = new TestScenarioBuilder()
                .AddMethod("A",
                    subTasks: new() { new SetValueAction(10) })
                .AddMethod("B",
                    subTasks: new() { new SetValueAction(100) })
                .AddMethod("C",
                    subTasks: new() { new SetValueAction(50) })
                .BuildMethods();

            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());
            var selectedMethod = await selector.SelectAsync(methods, context, CancellationToken.None);

            Assert.IsNotNull(selectedMethod);
            Assert.AreEqual("B", selectedMethod.Name);
            Assert.AreEqual(3, envModel.SimulationCount);
        }

        [UnityTest]
        public IEnumerator SelectInterruptsAsync_WhenNewMethodIsBetter_ShouldReturnNewMethod()
        {
            return Run_SelectInterruptsAsync_WhenNewMethodIsBetter().ToCoroutine();
        }

        async UniTask Run_SelectInterruptsAsync_WhenNewMethodIsBetter()
        {
            var selector = new SimulationSelector<TestActorContext, TestWorldState>(s => s.Value);
            var (methods, currentMethodInfo) = new TestScenarioBuilder()
                .AddMethod("Current",
                    subTasks: new() { new SetValueAction(50) })
                .AddMethod("Better",
                    subTasks: new() { new SetValueAction(100) })
                .SetCurrentMethod("Current")
                .Build();

            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());
            var selectedMethod =
                await selector.SelectInterruptsAsync(methods, currentMethodInfo, context, CancellationToken.None);

            Assert.IsNotNull(selectedMethod);
            Assert.AreEqual("Better", selectedMethod.Name);
        }

        [UnityTest]
        public IEnumerator SelectInterruptsAsync_WhenCurrentMethodIsStillBest_ShouldReturnNull()
        {
            return Run_SelectInterruptsAsync_WhenCurrentMethodIsStillBest().ToCoroutine();
        }

        async UniTask Run_SelectInterruptsAsync_WhenCurrentMethodIsStillBest()
        {
            var selector = new SimulationSelector<TestActorContext, TestWorldState>(s => s.Value);
            var (methods, currentMethodInfo) = new TestScenarioBuilder()
                .AddMethod("Current",
                    subTasks: new() { new SetValueAction(100) })
                .AddMethod("Worse",
                    subTasks: new() { new SetValueAction(50) })
                .SetCurrentMethod("Current")
                .Build();

            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());
            var selectedMethod =
                await selector.SelectInterruptsAsync(methods, currentMethodInfo, context, CancellationToken.None);

            Assert.IsNull(selectedMethod);
        }
    }
}