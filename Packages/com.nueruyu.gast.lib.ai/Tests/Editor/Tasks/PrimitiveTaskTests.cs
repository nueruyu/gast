using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Gast.Lib.AI.Tests.Editor.Tasks
{
    [TestFixture]
    public class PrimitiveTaskTests
    {
        [UnityTest]
        public IEnumerator ValidateAsync_WhenActionIsUnavailable_ReturnsFalse()
        {
            return Run_ValidateAsync_WhenActionIsUnavailable_ReturnsFalse().ToCoroutine();
        }

        async UniTask Run_ValidateAsync_WhenActionIsUnavailable_ReturnsFalse()
        {
            var action = new OpenDoorAction(); // Requires HasKey = true
            var task = new PrimitiveTask<TestActorContext, TestWorldState>(action);
            var worldState = new TestWorldState { HasKey = false };
            var context = new ValidationContext<TestWorldState>(worldState, new PlanningStateStore());

            var result = await task.ValidateAsync(context, CancellationToken.None);

            Assert.IsFalse(result);
        }

        [UnityTest]
        public IEnumerator ValidateAsync_WhenActionIsAvailable_ReturnsTrueAndSimulatesState()
        {
            return Run_ValidateAsync_WhenActionIsAvailable_ReturnsTrueAndSimulatesState().ToCoroutine();
        }

        async UniTask Run_ValidateAsync_WhenActionIsAvailable_ReturnsTrueAndSimulatesState()
        {
            var action = new FindKeyAction();
            var task = new PrimitiveTask<TestActorContext, TestWorldState>(action);
            var worldState = new TestWorldState { HasKey = false };
            var context = new ValidationContext<TestWorldState>(worldState, new PlanningStateStore());

            var result = await task.ValidateAsync(context, CancellationToken.None);

            Assert.IsTrue(result);
            Assert.IsTrue(worldState.HasKey);
        }

        [UnityTest]
        public IEnumerator RunAsync_ShouldExecuteAction()
        {
            return Run_RunAsync_ShouldExecuteAction().ToCoroutine();
        }

        async UniTask Run_RunAsync_ShouldExecuteAction()
        {
            var action = new FindKeyAction();
            var task = new PrimitiveTask<TestActorContext, TestWorldState>(action);
            var actorContext = new TestActorContext(new TestWorldState());
            var context = new ExecutionContext<TestActorContext>(new ContextKey("actor1", "domain1"), actorContext);

            await task.RunAsync(context, CancellationToken.None);

            Assert.AreEqual(1, action.ExecutionCount);
        }
    }
}
