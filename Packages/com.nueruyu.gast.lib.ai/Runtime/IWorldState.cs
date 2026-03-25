namespace Gast.Lib.AI
{
    /// <summary>
    /// Interface for world state objects used in AI planning.
    /// Ensures that states can be copied for simulation.
    /// </summary>
    public interface IWorldState<TWorldState> where TWorldState : IWorldState<TWorldState>
    {
        void WriteTo(ref TWorldState destination);
    }
}
