using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.Lib.AI.Tests
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

        public void UpdateWorldState() { }
    }

    public abstract class TestAction : IAction<TestActorContext, TestWorldState>
    {
        private readonly string name;
        public int ExecutionCount { get; private set; }

        protected TestAction(string name)
        {
            this.name = name;
        }

        public abstract bool IsAvailable(TestWorldState worldState);
        public abstract void Simulate(TestWorldState worldState);

        public UniTask ExecuteAsync(TestActorContext context, CancellationToken cancellationToken)
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
        public FindKeyAction() : base(nameof(FindKeyAction)) { }
        public override bool IsAvailable(TestWorldState worldState) => !worldState.HasKey;
        public override void Simulate(TestWorldState worldState) => worldState.HasKey = true;
    }

    public class OpenDoorAction : TestAction
    {
        public OpenDoorAction() : base(nameof(OpenDoorAction)) { }
        public override bool IsAvailable(TestWorldState worldState) => worldState.HasKey && !worldState.IsDoorOpen;
        public override void Simulate(TestWorldState worldState) => worldState.IsDoorOpen = true;
    }
}
