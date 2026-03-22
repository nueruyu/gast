using System;
using System.Collections.Generic;
using Cryst.Domain.AI.Objectives;
using Cryst.Features.CharacterAI.Actions;
using Cryst.Features.CharacterAI.Humanoid.Objective.Actions;
using Gast.Domain.AI;
using Gast.Lib.AI;
using Gast.Lib.AI.Builders;
using Gast.Lib.AI.MethodSelectors;
using R3;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public class ObjectiveDomainFactory : IAIDomainFactory<ObjectiveState>, IDisposable
    {
        readonly CompositeDisposable disposables = new();
        AIDomain<ActorContext<ObjectiveState>, ObjectiveState> domain;

        public ObjectiveDomainFactory(ObjectiveManager objectiveManager)
        {
            objectiveManager.CurrentObjectivesObservable
                .Subscribe(RebuildDomain)
                .AddTo(disposables);
        }

        public AIDomain<ActorContext<ObjectiveState>, ObjectiveState> GetDomain()
        {
            return domain;
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

        void RebuildDomain(IReadOnlyList<IAIObjective> objectives)
        {
            var builder = new AIDomainBuilder<ActorContext<ObjectiveState>, ObjectiveState>();

            var root = builder.DefineCompound("Root")
                .UseSelector(new UtilitySelector<ActorContext<ObjectiveState>, ObjectiveState>());

            foreach (var objective in objectives)
                switch (objective)
                {
                    case DefeatCharacterObjective combatObjective:
                        root.AddMethod($"SelectCombat_{combatObjective.TargetTypeId}")
                            .While(s => !combatObjective.IsCompleted.Value)
                            .Score(state => CalculateCombatScore(state, combatObjective))
                            .Do(new SelectCombatObjectiveAction(combatObjective))
                            .Do(new IdleAction());
                        break;
                    case AcquireItemObjective gatheringObjective:
                        root.AddMethod($"SelectGathering_{gatheringObjective.TargetItemId}")
                            .While(s => !gatheringObjective.IsCompleted.Value)
                            .Score(state => CalculateGatheringScore(state, gatheringObjective))
                            .Do(new SelectGatheringObjectiveAction(gatheringObjective))
                            .Do(new IdleAction());
                        break;
                }

            root.AddMethod("Idle")
                .Score(s => 0)
                .Do(new IdleAction());

            domain = builder.Build("Root");
        }

        static float CalculateCombatScore(ObjectiveState state, DefeatCharacterObjective objective)
        {
            var target = state.Queries.FindBestTargetFor(objective, state);
            if (target == null)
                return float.NegativeInfinity;

            var distance = Vector3.Distance(state.Self.Position, target.Position);
            // Higher score for closer targets.
            return 100f / (1f + distance);
        }

        static float CalculateGatheringScore(ObjectiveState state, AcquireItemObjective objective)
        {
            var target = state.Queries.FindBestTargetFor(objective, state);
            if (target == null)
                return float.NegativeInfinity;

            var distance = Vector3.Distance(state.Self.Position, target.Position);
            // Combat is generally prioritized over gathering.
            return 50f / (1f + distance);
        }
    }
}