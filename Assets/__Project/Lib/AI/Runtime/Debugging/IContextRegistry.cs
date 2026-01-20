namespace Gast.Lib.AI.Debugging
{
    public interface IContextRegistry
    {
        void Register(ContextKey contextKey);
        void Unregister(ContextKey contextKey);
    }
}
