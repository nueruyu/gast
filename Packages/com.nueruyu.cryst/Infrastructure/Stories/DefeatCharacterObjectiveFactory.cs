using Cryst.Domain.AI.Objectives;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Unity.Features.Stories;

namespace Cryst.Infrastructure.Stories
{
    public class DefeatCharacterObjectiveFactory : IStoryObjectiveFactory<DefeatCharacterObjectiveFactory.Params>
    {
        public string ObjectiveType => "DefeatCharacter";

        public IAIObjective Create(Params parameters)
            => new DefeatCharacterObjective(parameters.TargetTypeId, 1);

        public class Params
        {
            public CharacterTypeId TargetTypeId { get; set; }
        }
    }
}
