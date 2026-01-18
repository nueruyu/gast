namespace Gast.Domain.AI
{
    public interface IGoal
    {
        bool IsCompleted { get; }
        // Future extensions could include progress properties.
    }
}