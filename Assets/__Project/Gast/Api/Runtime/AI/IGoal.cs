namespace Gast.Api.AI
{
    public interface IGoal
    {
        bool IsCompleted { get; }
        // Future extensions could include progress properties.
    }
}