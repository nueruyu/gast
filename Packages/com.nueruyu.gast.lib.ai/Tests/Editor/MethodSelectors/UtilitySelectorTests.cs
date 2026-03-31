using System.Collections.Generic;
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
        public System.Collections.IEnumerator SelectAsync_ShouldReturnMethodWithHighestScore()
        {
            return Run_SelectAsync_ShouldReturnMethodWithHighestScore().ToCoroutine();
        }

        async UniTask Run_SelectAsync_ShouldReturnMethodWithHighestScore()
        {
            var selector = new UtilitySelector<TestActorContext, TestWorldState>();

            // Method constructor: (name, index, subTasks, startCondition, continuationCondition, scorer, interruptionCost)
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

        [UnityTest]
        public System.Collections.IEnumerator SelectInterruptsAsync_WhenNewMethodIsBetter_ShouldReturnNewMethod()
        {
            return Run_SelectInterruptsAsync_WhenNewMethodIsBetter().ToCoroutine();
        }

        async UniTask Run_SelectInterruptsAsync_WhenNewMethodIsBetter()
        {
            var selector = new UtilitySelector<TestActorContext, TestWorldState>();

            // currentMethod: score=50, interruptionCost=20
            // betterMethod: score=80
            // Interrupt check: 80 > 50 + 20 = 80 > 70 → true
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
            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());
            var currentMethodInfo = new CurrentMethodInfo<TestActorContext, TestWorldState>(currentMethod);

            var selectedMethod = await selector.SelectInterruptsAsync(methods, currentMethodInfo, context, CancellationToken.None);

            Assert.IsNotNull(selectedMethod);
            Assert.AreEqual("Better", selectedMethod.Name);
        }

        [UnityTest]
        public System.Collections.IEnumerator SelectInterruptsAsync_WhenNewMethodIsNotGoodEnough_ShouldReturnNull()
        {
            return Run_SelectInterruptsAsync_WhenNewMethodIsNotGoodEnough().ToCoroutine();
        }

        async UniTask Run_SelectInterruptsAsync_WhenNewMethodIsNotGoodEnough()
        {
            var selector = new UtilitySelector<TestActorContext, TestWorldState>();

            // currentMethod: score=50, interruptionCost=20
            // notGoodEnoughMethod: score=60
            // Interrupt check: 60 > 50 + 20 = 60 > 70 → false → null
            var currentMethod = new Method<TestActorContext, TestWorldState>(
                "Current", 0,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null,
                s => 50f,
                s => 20f);
            var notGoodEnoughMethod = new Method<TestActorContext, TestWorldState>(
                "NotGoodEnough", 1,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null,
                s => 60f);

            var methods = new List<Method<TestActorContext, TestWorldState>> { currentMethod, notGoodEnoughMethod };
            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());
            var currentMethodInfo = new CurrentMethodInfo<TestActorContext, TestWorldState>(currentMethod);

            var selectedMethod = await selector.SelectInterruptsAsync(methods, currentMethodInfo, context, CancellationToken.None);

            Assert.IsNull(selectedMethod);
        }
    }
}
