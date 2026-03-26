using Gast.Domain.Characters;

namespace Cryst.Features.Stories.Actions
{
    public class WaitForCharacterDefeatedActionFactory : StoryActionFactory<WaitForCharacterDefeatedActionFactory.Params>
    {
        public override string ActionName => "WaitForCharacterDefeated";

        protected override WaitForCharacterDefeatedAction Create(Params parameters)
            => new WaitForCharacterDefeatedAction(parameters.CharacterId);

        public class Params
        {
            public CharacterId CharacterId { get; set; }
        }
    }
}
