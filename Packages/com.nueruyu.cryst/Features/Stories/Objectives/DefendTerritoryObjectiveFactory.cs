using Cryst.Domain.AI.Objectives;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Unity.Features.Stories;

namespace Cryst.Features.Stories.Objectives
{
    public class DefendTerritoryObjectiveFactory : StoryObjectiveFactory<DefendTerritoryObjectiveFactory.Params>
    {
        public override string ObjectiveType => "DefendTerritory";

        protected override IAIObjective Create(Params parameters)
            => new DefendTerritoryObjective(parameters.PriorityTargetId);

        public class Params
        {
            public CharacterId? PriorityTargetId { get; set; }
        }
    }
}
