using System.Collections.Generic;
using System.Threading;
using Cryst.Domain.AI.Objectives;
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
                var character = context.CharacterRepository.Get(assignment.CharacterId);
                var brain = context.AIBrainFactory.Create();
                var objective = CreateObjective(assignment);
                brain.SetObjectives(new[] { objective });
                context.BrainManager.AttachBrain(assignment.CharacterId, brain);
            }

            return UniTask.CompletedTask;
        }

        IAIObjective CreateObjective(Assignment assignment)
        {
            return assignment.ObjectiveType switch
            {
                "DefeatCharacter" => CreateDefeatCharacterObjective(assignment),
                "DefendTerritory" => CreateDefendTerritoryObjective(assignment),
                _ => throw new System.NotSupportedException(
                    $"Unknown objective type '{assignment.ObjectiveType}' in SetObjectivesAction.")
            };
        }

        IAIObjective CreateDefeatCharacterObjective(Assignment assignment)
        {
            var typeIdStr = assignment.Parameters.TryGetValue("target_type_id", out var raw) ? raw : string.Empty;
            return new DefeatCharacterObjective(new CharacterTypeId(typeIdStr), 1);
        }

        IAIObjective CreateDefendTerritoryObjective(Assignment assignment)
        {
            CharacterId? priorityTargetId = null;
            if (assignment.Parameters.TryGetValue("priority_target_id", out var raw)
                && raw is string guidStr
                && System.Guid.TryParse(guidStr, out var guid))
            {
                priorityTargetId = CharacterId.FromGuid(guid);
            }

            return new DefendTerritoryObjective(priorityTargetId);
        }

        public class Assignment
        {
            public CharacterId CharacterId { get; set; }
            public string ObjectiveType { get; set; }
            public Dictionary<string, object> Parameters { get; set; } = new();
        }
    }
}
