namespace Gast.Lib.AI
{
    /// <summary>
    /// Interface for world state objects used in AI planning.
    /// Ensures that states can be copied for simulation.
    /// </summary>
    public interface IWorldState<in T> where T : class
    {
        /// <summary>
        /// Copies the state from another instance.
        /// </summary>
        /// <param name="source">The source state to copy from.</param>
        void CopyFrom(T source);
    }
}
