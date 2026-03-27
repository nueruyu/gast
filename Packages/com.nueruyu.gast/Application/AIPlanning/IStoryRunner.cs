using Gast.Domain.Stories;

namespace Gast.Application.AIPlanning
{
    public interface IStoryRunner
    {
        void StartStory(StoryBlueprint blueprint);
    }
}
