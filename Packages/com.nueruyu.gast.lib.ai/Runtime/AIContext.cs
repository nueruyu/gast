namespace Gast.Lib.AI
{
    public class AIContext<TActorContext> where TActorContext : class
    {
        public ContextKey Key { get; }
        public TActorContext ActorContext { get; }

        public AIContext(ContextKey key, TActorContext actorContext)
        {
            Key = key;
            ActorContext = actorContext;
        }
    }
}
