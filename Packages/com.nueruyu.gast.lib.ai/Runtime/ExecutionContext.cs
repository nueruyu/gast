namespace Gast.Lib.AI
{
    public readonly struct ExecutionContext<TActorContext> where TActorContext : class
    {
        public ContextKey Key { get; }
        public TActorContext ActorContext { get; }

        public ExecutionContext(
            ContextKey key,
            TActorContext actorContext)
        {
            Key = key;
            ActorContext = actorContext;
        }
    }
}