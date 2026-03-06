namespace Gast.Lib.AI
{
    public interface IActorContext<out TWorldState> where TWorldState : class
    {
        TWorldState WorldState { get; }

        void UpdateWorldState();
    }
}
