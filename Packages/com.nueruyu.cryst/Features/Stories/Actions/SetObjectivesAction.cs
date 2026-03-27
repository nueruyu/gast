using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Lib.AI;

namespace Cryst.Features.Stories.Actions
{
    public class SetObjectivesAction : IAction<StoryActorContext, StoryWorldState>
    {
        readonly List<Assignment> assignments;

        public SetObjectivesAction(List<Assignment> assignments)
        {
            this.assignments = assignments;
        }

        public bool IsAvailable(StoryWorldState worldState) => true;

        public void Simulate(StoryWorldState worldState) { }

        public UniTask ExecuteAsync(StoryActorContext context, CancellationToken cancellationToken)
        {
            foreach (var assignment in assignments)
            {
                var brain = context.AIBrainFactory.Create();
                brain.SetObjectives(new[] { assignment.Objective });
                context.BrainManager.AttachBrain(assignment.CharacterId, brain);
            }

            return UniTask.CompletedTask;
        }

        public class Assignment
        {
            public CharacterId CharacterId { get; }
            public IAIObjective Objective { get; }

            public Assignment(CharacterId characterId, IAIObjective objective)
            {
                CharacterId = characterId;
                Objective = objective;
            }
        }
    }
}
