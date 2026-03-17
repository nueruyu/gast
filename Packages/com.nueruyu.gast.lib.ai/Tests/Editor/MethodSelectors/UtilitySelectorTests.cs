using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.MethodSelectors;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Gast.Lib.AI.Tests.MethodSelectors
{
    [TestFixture]
    public class UtilitySelectorTests
    {
        [UnityTest]
        public System.Collections.IEnumerator SelectAsync_ShouldReturnMethodWithHighestScore()
        {
            return Run_SelectAsync_ShouldReturnMethodWithHighestScore().ToCoroutine();
        }

        async UniTask Run_SelectAsync_ShouldReturnMethodWithHighestScore()
        {
            var selector = new UtilitySelector<TestActorContext, TestWorldState>();

            // Method constructor signature:
            // Method(name, index, subTasks, startCondition, continuationCondition, scorer, interruptionCost)
            var methodLow = new Method<TestActorContext, TestWorldState>(
                "Low", 0,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null,
                s => 10f);
            var methodMid = new Method<TestActorContext, TestWorldState>(
                "Mid", 1,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null,
                s => 50f);
            var methodHigh = new Method<TestActorContext, TestWorldState>(
                "High", 2,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null,
                s => 100f);
            var methodInvalid = new Method<TestActorContext, TestWorldState>(
                "Invalid", 3,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => false,
                null,
                s => 200f);

            var methods = new List<Method<TestActorContext, TestWorldState>>
            {
                methodLow, methodMid, methodHigh, methodInvalid
            };

            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());

            var selectedMethod = await selector.SelectAsync(methods, context, CancellationToken.None);

            Assert.IsNotNull(selectedMethod);
            Assert.AreEqual("High", selectedMethod.Name);
        }
    }
}
