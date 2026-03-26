using Gast.Core.Events;

namespace Gast.Domain.Stories
{
    public readonly struct StoryGeneratedEvent : IDomainEvent
    {
        public string StoryJson { get; }

        public StoryGeneratedEvent(string storyJson)
        {
            StoryJson = storyJson;
        }
    }
}
