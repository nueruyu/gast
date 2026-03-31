using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.Lib.AI.Tests.Editor
{
    public class TestWorldState : IWorldState<TestWorldState>
    {
        public bool HasKey { get; set; }
        public bool IsDoorOpen { get; set; }

        public void WriteTo(ref TestWorldState destination)
        {
            destination ??= new TestWorldState();
            destination.HasKey = HasKey;
            destination.IsDoorOpen = IsDoorOpen;
        }

        public override string ToString()
        {
            return $"State(HasKey: {HasKey}, IsDoorOpen: {IsDoorOpen})";
        }
    }

    public class TestActorContext : IActorContext<TestWorldState>
    {
        public TestWorldState WorldState { get; }

        public TestActorContext(TestWorldState worldState)
        {
            WorldState = worldState;
        }

        public void UpdateWorldState()
        {
        }
    }

    public abstract class TestAction : IAction<TestActorContext, TestWorldState>
    {
        readonly string name;
        public int ExecutionCount { get; private set; }

        protected TestAction(string name)
        {
            this.name = name;
        }

        public abstract bool IsAvailable(TestWorldState worldState);
        public abstract void Simulate(TestWorldState worldState);

        public UniTask ExecuteAsync(
            TestActorContext context,
            CancellationToken cancellationToken)
        {
            ExecutionCount++;
            return UniTask.CompletedTask;
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class FindKeyAction : TestAction
    {
        public FindKeyAction() : base(nameof(FindKeyAction))
        {
        }

        public override bool IsAvailable(TestWorldState worldState) => !worldState.HasKey;
        public override void Simulate(TestWorldState worldState) => worldState.HasKey = true;
    }

    public class OpenDoorAction : TestAction
    {
        public OpenDoorAction() : base(nameof(OpenDoorAction))
        {
        }

        public override bool IsAvailable(TestWorldState worldState) => worldState.HasKey && !worldState.IsDoorOpen;
        public override void Simulate(TestWorldState worldState) => worldState.IsDoorOpen = true;
    }

    class TestScenarioBuilder
    {
        readonly List<Method<TestActorContext, TestWorldState>> methods = new();
        string currentMethodName;

        public TestScenarioBuilder AddMethod(
            string name,
            Func<TestWorldState, bool> startCondition = null,
            Func<TestWorldState, float> scorer = null,
            Func<TestWorldState, float> interruptionCost = null)
        {
            var method = new Method<TestActorContext, TestWorldState>(
                name,
                methods.Count,
                new List<ITask<TestActorContext, TestWorldState>>(),
                startCondition ?? (_ => true),
                null,
                scorer,
                interruptionCost
            );
            methods.Add(method);
            return this;
        }

        public TestScenarioBuilder SetCurrentMethod(string name)
        {
            currentMethodName = name;
            return this;
        }

        public (List<Method<TestActorContext, TestWorldState>> Methods,
            CurrentMethodInfo<TestActorContext, TestWorldState> CurrentMethodInfo) Build()
        {
            var methodList = new List<Method<TestActorContext, TestWorldState>>(methods);
            CurrentMethodInfo<TestActorContext, TestWorldState> currentMethodInfo = null;

            if (currentMethodName != null)
            {
                var currentMethod = methodList.Find(m => m.Name == currentMethodName);
                if (currentMethod == null)
                    throw new InvalidOperationException($"Method with name '{currentMethodName}' not found.");
                currentMethodInfo = new CurrentMethodInfo<TestActorContext, TestWorldState>(currentMethod);
            }

            return (methodList, currentMethodInfo);
        }

        public List<Method<TestActorContext, TestWorldState>> BuildMethods()
        {
            return new List<Method<TestActorContext, TestWorldState>>(methods);
        }
    }
}