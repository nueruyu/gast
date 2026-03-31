using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Builders;
using Gast.Lib.AI.Tasks;
using Gast.Lib.AI.Testing;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Gast.Lib.AI.Tests.Editor
{
    [TestFixture]
    public class AISimulatorTests
    {
        AIDomain<TestActorContext, TestWorldState> domain;

        [SetUp]
        public void SetUp()
        {
            var builder = new AIDomainBuilder<TestActorContext, TestWorldState>();
            var openDoorTask = builder.DefineCompound("OpenDoorTask");
            openDoorTask.AddMethod("FindKeyAndOpen")
                .When(state => !state.HasKey)
                .Do(new FindKeyAction())
                .Do(new OpenDoorAction());
            openDoorTask.AddMethod("OpenImmediately")
                .When(state => state.HasKey)
                .Do(new OpenDoorAction());

            domain = builder.Build("OpenDoorTask");
        }

        [UnityTest]
        public IEnumerator SimulateAsync_WhenKeyIsHeld_ShouldGeneratePlanToOpenDoor()
        {
            return Run_SimulateAsync_WhenKeyIsHeld().ToCoroutine();
        }

        async UniTask Run_SimulateAsync_WhenKeyIsHeld()
        {
            var simulator = new AISimulator<TestActorContext, TestWorldState>(domain);
            var initialState = new TestWorldState { HasKey = true, IsDoorOpen = false };

            var result = await simulator.SimulateAsync(initialState);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.SimulatedTaskSequence.Count);
            Assert.IsInstanceOf<OpenDoorAction>(result.SimulatedTaskSequence[0]
                .Unwrap<TestActorContext, TestWorldState>());
            Assert.IsTrue(result.FinalWorldState.IsDoorOpen);
        }

        [UnityTest]
        public IEnumerator SimulateAsync_WhenKeyIsNotHeld_ShouldGeneratePlanToFindKeyAndOpenDoor()
        {
            return Run_SimulateAsync_WhenKeyIsNotHeld().ToCoroutine();
        }

        async UniTask Run_SimulateAsync_WhenKeyIsNotHeld()
        {
            var simulator = new AISimulator<TestActorContext, TestWorldState>(domain);
            var initialState = new TestWorldState { HasKey = false, IsDoorOpen = false };

            var result = await simulator.SimulateAsync(initialState);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.SimulatedTaskSequence.Count);
            Assert.IsInstanceOf<FindKeyAction>(result.SimulatedTaskSequence[0]
                .Unwrap<TestActorContext, TestWorldState>());
            Assert.IsInstanceOf<OpenDoorAction>(result.SimulatedTaskSequence[1]
                .Unwrap<TestActorContext, TestWorldState>());
            Assert.IsTrue(result.FinalWorldState.HasKey);
            Assert.IsTrue(result.FinalWorldState.IsDoorOpen);
        }
    }

    static class TaskExtensions
    {
        internal static IAction<TActorContext, TWorldState> Unwrap<TActorContext, TWorldState>(this ITask task)
            where TWorldState : class, IWorldState<TWorldState>
            where TActorContext : class, IActorContext<TWorldState>
        {
            if (task is PrimitiveTask<TActorContext, TWorldState> primitiveTask) 
                return primitiveTask.Action;
            throw new InvalidCastException($"Cannot unwrap {task.GetType().Name} to an IAction.");
        }
    }
}