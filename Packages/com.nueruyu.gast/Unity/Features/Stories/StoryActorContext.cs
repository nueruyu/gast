using Gast.Core.Commands;
using Gast.Core.Events;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Lib.AI;

namespace Gast.Unity.Features.Stories
{
    /// <summary>
    /// Actor context for the story HTN domain.
    /// Provides story actions with access to game services.
    /// </summary>
    public class StoryActorContext : IActorContext<StoryWorldState>
    {
        public StoryWorldState WorldState { get; } = new();

        public IDomainEventSubscriber EventSubscriber { get; }
        public ICharacterRepository CharacterRepository { get; }
        public ICommandDispatcher CommandDispatcher { get; }
        public ICharacterAIBrainFactory AIBrainFactory { get; }
        public ICharacterBrainManager BrainManager { get; }

        public StoryActorContext(
            IDomainEventSubscriber eventSubscriber,
            ICharacterRepository characterRepository,
            ICommandDispatcher commandDispatcher,
            ICharacterAIBrainFactory aiBrainFactory,
            ICharacterBrainManager brainManager)
        {
            EventSubscriber = eventSubscriber;
            CharacterRepository = characterRepository;
            CommandDispatcher = commandDispatcher;
            AIBrainFactory = aiBrainFactory;
            BrainManager = brainManager;
        }

        public void UpdateWorldState() { }
    }
}
