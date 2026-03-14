namespace Gast.Lib.AI
{
    public readonly struct ExecutionContext<TActorContext> where TActorContext : class
    {
        public ContextKey Key { get; }
        public TActorContext ActorContext { get; }
        public PlanningStateStore PlanningStateStore { get; }

        public ExecutionContext(ContextKey key, TActorContext actorContext, PlanningStateStore planningStateStore)
        {
            Key = key;
            ActorContext = actorContext;
            PlanningStateStore = planningStateStore;
        }
    }
}
