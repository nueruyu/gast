using Cryst.Domain.AI.Objectives;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Unity.Features.Stories;

namespace Cryst.Infrastructure.Stories
{
    public class DefendTerritoryObjectiveFactory : IStoryObjectiveFactory<DefendTerritoryObjectiveFactory.Params>
    {
        public string ObjectiveType => "DefendTerritory";

        public IAIObjective Create(Params parameters)
            => new DefendTerritoryObjective(parameters.PriorityTargetId);

        public class Params
        {
            public CharacterId? PriorityTargetId { get; set; }
        }
    }
}
