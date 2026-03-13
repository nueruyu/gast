namespace Gast.Lib.AI
{
    public readonly struct ExecutionContext<TActorContext> where TActorContext : class
    {
        public ContextKey Key { get; }
        public TActorContext ActorContext { get; }
        public PlanningContext PlanningContext { get; }

        public ExecutionContext(ContextKey key, TActorContext actorContext, PlanningContext planningContext)
        {
            Key = key;
            ActorContext = actorContext;
            PlanningContext = planningContext;
        }
    }
}
