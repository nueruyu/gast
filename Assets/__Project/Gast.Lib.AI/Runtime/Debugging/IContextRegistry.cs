namespace Gast.Lib.AI.Debugging
{
    public interface IContextRegistry
    {
        void Register(ContextKey contextKey, object worldState);
        void Unregister(ContextKey contextKey);
    }
}
