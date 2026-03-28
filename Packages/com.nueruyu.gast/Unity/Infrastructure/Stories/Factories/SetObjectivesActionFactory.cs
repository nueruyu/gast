using Gast.Domain.Characters;
using Gast.Lib.AI;
using Gast.Unity.Features.Stories;
using Gast.Unity.Features.Stories.Actions;
using Gast.Unity.Infrastructure.Remoting.AI;
using System.Collections.Generic;
using System.Linq;

namespace Gast.Unity.Infrastructure.Stories.Factories
{
    public class SetObjectivesActionFactory : IStoryActionFactory<SetObjectivesActionFactory.Params>
    {
        readonly GoalInstantiator goalInstantiator;

        public SetObjectivesActionFactory(GoalInstantiator goalInstantiator)
        {
            this.goalInstantiator = goalInstantiator;
        }

        public string ActionName => "SetObjectives";

        public IAction<StoryActorContext, StoryWorldState> Create(Params parameters)
        {
            var assignments = parameters.Assignments.Select(def =>
            {
                var objective = goalInstantiator.CreateGoal(
                    def.ObjectiveType,
                    def.ObjectiveParameters ?? new Dictionary<string, object>());

                return new SetObjectivesAction.Assignment(def.CharacterId, objective);
            }).ToList();

            return new SetObjectivesAction(assignments);
        }

        public class Params
        {
            public List<AssignmentDef> Assignments { get; set; }
        }

        public class AssignmentDef
        {
            public CharacterId CharacterId { get; set; }
            public string ObjectiveType { get; set; }
            public Dictionary<string, object> ObjectiveParameters { get; set; }
        }
    }
}
