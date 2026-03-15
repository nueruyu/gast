namespace Gast.Lib.AI.Debugging
{
    public interface IContextRegistry
    {
        void Register(ContextKey contextKey, object worldState, string actorName);
        void Unregister(ContextKey contextKey);
    }
}
