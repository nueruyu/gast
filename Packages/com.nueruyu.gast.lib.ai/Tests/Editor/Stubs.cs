using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.MethodSelectors;
using Gast.Lib.AI.Tasks;

namespace Gast.Lib.AI.Tests.Editor
{
    public class TestWorldState : IWorldState<TestWorldState>
    {
        public bool HasKey { get; set; }
        public bool IsDoorOpen { get; set; }
        public int Value { get; set; }

        public void WriteTo(ref TestWorldState destination)
        {
            destination ??= new TestWorldState();
            destination.HasKey = HasKey;
            destination.IsDoorOpen = IsDoorOpen;
            destination.Value = Value;
        }

        public override string ToString()
        {
            return $"State(HasKey: {HasKey}, IsDoorOpen: {IsDoorOpen}, Value: {Value})";
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

    public class SetValueAction : TestAction
    {
        readonly int value;

        public SetValueAction(int value) : base($"{nameof(SetValueAction)}({value})")
        {
            this.value = value;
        }

        public override bool IsAvailable(TestWorldState worldState) => true;
        public override void Simulate(TestWorldState worldState) => worldState.Value = value;
    }

    class TestEnvironmentModel : IEnvironmentModel<TestWorldState>
    {
        public int SimulationCount { get; private set; }

        public void Simulate(TestWorldState state)
        {
            SimulationCount++;
        }
    }

    class TestScenarioBuilder
    {
        readonly List<Method<TestActorContext, TestWorldState>> methods = new();
        string currentMethodName;

        public TestScenarioBuilder AddMethod(
            string name,
            Func<TestWorldState, bool> startCondition = null,
            Func<TestWorldState, float> scorer = null,
            Func<TestWorldState, float> interruptionCost = null,
            List<IAction<TestActorContext, TestWorldState>> subTasks = null)
        {
            var providers = new List<Func<ITask<TestActorContext, TestWorldState>>>();
            if (subTasks != null)
            {
                foreach (var action in subTasks)
                    providers.Add(() => new PrimitiveTask<TestActorContext, TestWorldState>(action));
            }

            var method = new Method<TestActorContext, TestWorldState>(
                name,
                methods.Count,
                providers,
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
