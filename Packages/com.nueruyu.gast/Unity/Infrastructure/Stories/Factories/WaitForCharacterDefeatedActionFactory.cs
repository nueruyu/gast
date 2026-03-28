using Gast.Domain.Characters;
using Gast.Lib.AI;
using Gast.Unity.Features.Stories;
using Gast.Unity.Features.Stories.Actions;

namespace Gast.Unity.Infrastructure.Stories.Factories
{
    public class WaitForCharacterDefeatedActionFactory : StoryActionFactory<WaitForCharacterDefeatedActionFactory.Params>
    {
        public override string ActionName => "WaitForCharacterDefeated";

        protected override IAction<StoryActorContext, StoryWorldState>  Create(Params parameters)
            => new WaitForCharacterDefeatedAction(parameters.CharacterId);

        public class Params
        {
            public CharacterId CharacterId { get; set; }
        }
    }
}
