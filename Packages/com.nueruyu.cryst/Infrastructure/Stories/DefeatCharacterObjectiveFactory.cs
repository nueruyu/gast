using Cryst.Domain.AI.Objectives;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Unity.Features.Stories;

namespace Cryst.Infrastructure.Stories
{
    public class DefeatCharacterObjectiveFactory : StoryObjectiveFactory<DefeatCharacterObjectiveFactory.Params>
    {
        public override string ObjectiveType => "DefeatCharacter";

        protected override IAIObjective Create(Params parameters)
            => new DefeatCharacterObjective(parameters.TargetTypeId, 1);

        public class Params
        {
            public CharacterTypeId TargetTypeId { get; set; }
        }
    }
}
