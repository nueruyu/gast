using Gast.Application.AIPlanning;
using Gast.Lib.AI;

namespace Gast.Unity.Features.Stories
{
    public class PrimitiveTaskRef : IBlueprintTaskRef
    {
        public IAction<StoryActorContext, StoryWorldState> Action { get; }

        public PrimitiveTaskRef(IAction<StoryActorContext, StoryWorldState> action)
        {
            Action = action;
        }
    }
}
