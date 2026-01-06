namespace Gast.Domain.Npcs.Goals
{
    public interface IGoal
    {
        bool IsCompleted { get; }
        // Future extensions could include progress properties.
    }
}
