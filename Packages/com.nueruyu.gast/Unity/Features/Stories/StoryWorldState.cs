using Gast.Lib.AI;

namespace Gast.Unity.Features.Stories
{
    /// <summary>
    /// World state for the story HTN domain.
    /// Stories are event-driven rather than state-driven, so this is intentionally minimal.
    /// </summary>
    public class StoryWorldState : IWorldState<StoryWorldState>
    {
        public void WriteTo(ref StoryWorldState destination) { }
    }
}
