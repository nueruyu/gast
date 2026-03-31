using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Builders;
using Gast.Lib.AI.MethodSelectors;
using Gast.Lib.AI.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Gast.Lib.AI.Tests.Editor.Tasks
{
    [TestFixture]
    public class CompoundTaskTests
    {
        [UnityTest]
        public IEnumerator RunAsync_ShouldExecuteAllSubTasksInOrder()
        {
            return Run_RunAsync_ShouldExecuteAllSubTasksInOrder().ToCoroutine();
        }

        async UniTask Run_RunAsync_ShouldExecuteAllSubTasksInOrder()
        {
            var findKeyAction = new FindKeyAction();
            var openDoorAction = new OpenDoorAction();
            var builder = new AIDomainBuilder<TestActorContext, TestWorldState>();
            var root = builder.DefineCompound("Root");
            root.AddMethod("Test").Do(findKeyAction).Do(openDoorAction);
            var domain = builder.Build("Root");
            var task = domain.RootTask;

            var context = new ExecutionContext<TestActorContext>(
                new ContextKey("actor1", "domain1"),
                new TestActorContext(new TestWorldState()));

            await task.RunAsync(context, CancellationToken.None);

            Assert.AreEqual(1, findKeyAction.ExecutionCount);
            Assert.AreEqual(1, openDoorAction.ExecutionCount);
        }

        [UnityTest]
        public IEnumerator RunAsync_WithMethodSelection_ShouldExecuteCorrectMethod()
        {
            return Run_RunAsync_WithMethodSelection_ShouldExecuteCorrectMethod().ToCoroutine();
        }

        async UniTask Run_RunAsync_WithMethodSelection_ShouldExecuteCorrectMethod()
        {
            var actionA = new SetValueAction(1);
            var actionB = new SetValueAction(2);

            var builder = new AIDomainBuilder<TestActorContext, TestWorldState>();
            var root = builder.DefineCompound("Root")
                .UseSelector(new PrioritySelector<TestActorContext, TestWorldState>());
            root.AddMethod("A").When(s => s.Value == 10).Do(actionA);
            root.AddMethod("B").When(s => s.Value == 20).Do(actionB);
            var domain = builder.Build("Root");
            var task = domain.RootTask;

            var worldState = new TestWorldState { Value = 20 };
            var context = new ExecutionContext<TestActorContext>(
                new ContextKey("actor1", "domain1"),
                new TestActorContext(worldState));

            await task.RunAsync(context, CancellationToken.None);

            Assert.AreEqual(0, actionA.ExecutionCount);
            Assert.AreEqual(1, actionB.ExecutionCount);
        }
    }
}