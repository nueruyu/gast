using System.Collections.Generic;
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
        public System.Collections.IEnumerator SelectAsync_ShouldReturnFirstValidMethodByIndex()
        {
            return Run_SelectAsync_ShouldReturnFirstValidMethodByIndex().ToCoroutine();
        }

        async UniTask Run_SelectAsync_ShouldReturnFirstValidMethodByIndex()
        {
            var selector = new PrioritySelector<TestActorContext, TestWorldState>();

            // Method constructor: (name, index, subTasks, startCondition, continuationCondition, scorer, interruptionCost)
            var methodInvalid = new Method<TestActorContext, TestWorldState>(
                "Invalid", 0,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => false,
                null);
            var methodValid = new Method<TestActorContext, TestWorldState>(
                "Valid", 1,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null);
            var anotherValid = new Method<TestActorContext, TestWorldState>(
                "AnotherValid", 2,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null);

            var methods = new List<Method<TestActorContext, TestWorldState>>
            {
                methodInvalid, methodValid, anotherValid
            };
            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());

            var selectedMethod = await selector.SelectAsync(methods, context, CancellationToken.None);

            Assert.IsNotNull(selectedMethod);
            Assert.AreEqual("Valid", selectedMethod.Name);
        }

        [UnityTest]
        public System.Collections.IEnumerator SelectInterruptsAsync_WhenHigherPriorityMethodIsValid_ShouldReturnNewMethod()
        {
            return Run_SelectInterruptsAsync_WhenHigherPriorityMethodIsValid().ToCoroutine();
        }

        async UniTask Run_SelectInterruptsAsync_WhenHigherPriorityMethodIsValid()
        {
            var selector = new PrioritySelector<TestActorContext, TestWorldState>();

            // higherPriorityMethod: index=0, currentMethod: index=1
            // SelectAsync returns higherPriority (index 0 < current index 1) → interrupt
            var higherPriorityMethod = new Method<TestActorContext, TestWorldState>(
                "Higher", 0,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null);
            var currentMethod = new Method<TestActorContext, TestWorldState>(
                "Current", 1,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null);

            var methods = new List<Method<TestActorContext, TestWorldState>> { higherPriorityMethod, currentMethod };
            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());
            var currentMethodInfo = new CurrentMethodInfo<TestActorContext, TestWorldState>(currentMethod);

            var selectedMethod = await selector.SelectInterruptsAsync(methods, currentMethodInfo, context, CancellationToken.None);

            Assert.IsNotNull(selectedMethod);
            Assert.AreEqual("Higher", selectedMethod.Name);
        }

        [UnityTest]
        public System.Collections.IEnumerator SelectInterruptsAsync_WhenOnlyLowerPriorityMethodIsValid_ShouldReturnNull()
        {
            return Run_SelectInterruptsAsync_WhenOnlyLowerPriorityMethodIsValid().ToCoroutine();
        }

        async UniTask Run_SelectInterruptsAsync_WhenOnlyLowerPriorityMethodIsValid()
        {
            var selector = new PrioritySelector<TestActorContext, TestWorldState>();

            // currentMethod: index=0, lowerPriorityMethod: index=1
            // SelectAsync returns currentMethod (index 0); 0 < 0 is false → null
            var currentMethod = new Method<TestActorContext, TestWorldState>(
                "Current", 0,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null);
            var lowerPriorityMethod = new Method<TestActorContext, TestWorldState>(
                "Lower", 1,
                new List<ITask<TestActorContext, TestWorldState>>(),
                s => true,
                null);

            var methods = new List<Method<TestActorContext, TestWorldState>> { currentMethod, lowerPriorityMethod };
            var context = new ValidationContext<TestWorldState>(new TestWorldState(), new PlanningStateStore());
            var currentMethodInfo = new CurrentMethodInfo<TestActorContext, TestWorldState>(currentMethod);

            var selectedMethod = await selector.SelectInterruptsAsync(methods, currentMethodInfo, context, CancellationToken.None);

            Assert.IsNull(selectedMethod);
        }
    }
}
