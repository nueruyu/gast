using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Lib.AI;
using Gast.Unity.Features.Stories;
using Gast.Unity.Features.Stories.Actions;

namespace Gast.Unity.Infrastructure.Stories.Factories
{
    public class WaitForItemAcquiredActionFactory : IStoryActionFactory<WaitForItemAcquiredActionFactory.Params>
    {
        public string ActionName => "WaitForItemAcquired";

        public IAction<StoryActorContext, StoryWorldState> Create(Params parameters)
            => new WaitForItemAcquiredAction(parameters.AcquirerCharacterId, parameters.ItemId);

        public class Params
        {
            public CharacterId AcquirerCharacterId { get; set; }
            public ItemId ItemId { get; set; }
        }
    }
}