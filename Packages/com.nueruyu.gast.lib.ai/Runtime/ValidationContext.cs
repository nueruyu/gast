namespace Gast.Lib.AI
{
    public readonly struct ValidationContext<TWorldState> where TWorldState : class
    {
        public TWorldState WorldState { get; }
        public PlanningContext PlanningContext { get; }

        public ValidationContext(TWorldState worldState, PlanningContext planningContext)
        {
            WorldState = worldState;
            PlanningContext = planningContext;
        }
    }
}