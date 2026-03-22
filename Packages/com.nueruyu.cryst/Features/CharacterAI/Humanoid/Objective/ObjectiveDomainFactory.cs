using System.Linq;
using Cryst.Features.CharacterAI.Actions;
using Cryst.Features.CharacterAI.Humanoid.Objective.Actions;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;
using Gast.Lib.AI.MethodSelectors;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public class ObjectiveDomainFactory : IAIDomainFactory<ObjectiveState>
    {
        readonly AIDomain<ActorContext<ObjectiveState>, ObjectiveState> domain;

        public ObjectiveDomainFactory()
        {
            var builder = new AIDomainBuilder<ActorContext<ObjectiveState>, ObjectiveState>();

            var root = builder.DefineCompound("Root")
                .UseSelector(new UtilitySelector<ActorContext<ObjectiveState>, ObjectiveState>());

            root.AddMethod("SelectCombatObjective")
                .When(s => !s.HasActiveObjective && s.AvailableCombatObjectives.Any())
                .Score(CalculateBestCombatScore)
                .Do(new SelectBestCombatObjectiveAction());

            root.AddMethod("SelectGatheringObjective")
                .When(s => !s.HasActiveObjective && s.AvailableGatheringObjectives.Any())
                .Score(CalculateBestGatheringScore)
                .Do(new SelectBestGatheringObjectiveAction());

            root.AddMethod("Idle")
                .Score(s => 0.1f) // Low score, serves as a fallback.
                .Do(new IdleAction());

            domain = builder.Build("Root");
        }

        public AIDomain<ActorContext<ObjectiveState>, ObjectiveState> CreateDomain()
        {
            return domain;
        }

        static float CalculateBestCombatScore(ObjectiveState state)
        {
            var (_, target) = state.Queries.FindBestCombatObjective(state);
            if (target == null) return 0f;

            var distance = Vector3.Distance(state.Self.Position, target.Position);
            // Higher score for closer targets.
            return 100f / (1f + distance);
        }

        static float CalculateBestGatheringScore(ObjectiveState state)
        {
            var (_, target) = state.Queries.FindBestGatheringObjective(state);
            if (target == null) return 0f;

            var distance = Vector3.Distance(state.Self.Position, target.Position);
            // Combat is generally prioritized over gathering.
            return 50f / (1f + distance);
        }
    }
}