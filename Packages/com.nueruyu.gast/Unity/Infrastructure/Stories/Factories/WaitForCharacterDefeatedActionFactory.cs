using Gast.Domain.Characters;
using Gast.Lib.AI;
using Gast.Unity.Features.Stories;
using Gast.Unity.Features.Stories.Actions;

namespace Gast.Unity.Infrastructure.Stories.Factories
{
    public class WaitForCharacterDefeatedActionFactory :
        IStoryActionFactory<WaitForCharacterDefeatedActionFactory.Params>
    {
        public string ActionName => "WaitForCharacterDefeated";

        public IAction<StoryActorContext, StoryWorldState> Create(Params parameters)
            => new WaitForCharacterDefeatedAction(parameters.CharacterId);

        public class Params
        {
            public CharacterId CharacterId { get; set; }
        }
    }
}