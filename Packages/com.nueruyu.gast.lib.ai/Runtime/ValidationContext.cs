namespace Gast.Lib.AI
{
    public readonly struct ValidationContext<TWorldState> where TWorldState : class
    {
        public TWorldState WorldState { get; }
        public PlanningStateStore PlanningStateStore { get; }

        public ValidationContext(TWorldState worldState, PlanningStateStore planningStateStore)
        {
            WorldState = worldState;
            PlanningStateStore = planningStateStore;
        }
    }
}