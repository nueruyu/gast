namespace Gast.Lib.AI
{
    public readonly struct ValidationContext<TWorldState> where TWorldState : class
    {
        public TWorldState WorldState { get; }
        public PlanningStateStore PlanningStateStore { get; }
        public ITask CurrentlyExecutingTask { get; }

        public ValidationContext(
            TWorldState worldState,
            PlanningStateStore planningStateStore,
            ITask currentlyExecutingTask = null)
        {
            WorldState = worldState;
            PlanningStateStore = planningStateStore;
            CurrentlyExecutingTask = currentlyExecutingTask;
        }
    }
}