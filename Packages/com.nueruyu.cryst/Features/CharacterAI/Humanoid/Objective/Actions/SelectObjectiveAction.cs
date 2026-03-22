using System.Threading;
using Cryst.Domain.AI.Objectives;
using Cryst.Domain.Characters;
using Cysharp.Threading.Tasks;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Objective.Actions
{
    public class SelectObjectiveAction : IAction<ActorContext<ObjectiveState>, ObjectiveState>
    {
        readonly IAIObjective objective;

        public SelectObjectiveAction(IAIObjective objective)
        {
            this.objective = objective;
        }

        public bool IsAvailable(ObjectiveState worldState)
        {
            return true;
        }

        public void Simulate(ObjectiveState worldState)
        {
            worldState.SetActiveObjective();
        }

        public UniTask ExecuteAsync(ActorContext<ObjectiveState> context, CancellationToken cancellationToken)
        {
            var memory = context.GetModule<HumanoidMemory>();

            switch (objective)
            {
                case DefeatCharacterObjective combatObjective:
                    var targetInfo = context.ObjectiveQueries.FindBestTargetFor(combatObjective, context.WorldState);
                    if (targetInfo != null)
                    {
                        var targetCharacter = context.CharacterRepository.Get(targetInfo.Id).As<BaseCharacter>();
                        if (targetCharacter != null)
                        {
                            memory.SetObjective(objective);
                            memory.SetObjectiveCombatTarget(targetCharacter);
                        }
                    }

                    break;

                case AcquireItemObjective gatheringObjective:
                    var pickupInfo = context.ObjectiveQueries.FindBestTargetFor(gatheringObjective, context.WorldState);
                    if (pickupInfo != null)
                    {
                        var pickup = context.PickupRepository.Find(pickupInfo.Id);
                        var interactable = ((Component)pickup)?.GetComponentInChildren<IInteractable>();
                        if (interactable != null)
                        {
                            memory.SetObjective(objective);
                            memory.SetInteractableTarget(interactable);
                        }
                    }

                    break;
            }

            return UniTask.CompletedTask;
        }

        public override string ToString()
        {
            return $"{nameof(SelectObjectiveAction)}: {objective.GetType().Name}({objective})";
        }
    }
}