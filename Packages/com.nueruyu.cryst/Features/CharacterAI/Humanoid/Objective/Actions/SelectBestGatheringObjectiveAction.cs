using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Domain.Interactions;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Objective.Actions
{
    public class SelectBestGatheringObjectiveAction : IAction<ActorContext<ObjectiveState>, ObjectiveState>
    {
        public bool IsAvailable(ObjectiveState worldState)
        {
            return true;
        }

        public void Simulate(ObjectiveState worldState)
        {
            // Cognitive action, no direct world state change for planning.
        }

        public UniTask ExecuteAsync(ActorContext<ObjectiveState> context, CancellationToken cancellationToken)
        {
            var (objective, targetInfo) = context.ObjectiveQueries.FindBestGatheringObjective(context.WorldState);

            if (objective == null || targetInfo == null)
                return UniTask.CompletedTask;

            var memory = context.GetModule<HumanoidMemory>();
            var pickup = context.PickupRepository.Find(targetInfo.Id);
            var interactable = ((Component)pickup).GetComponentInChildren<IInteractable>();

            if (interactable != null)
            {
                memory.SetObjective(objective);
                memory.SetInteractableTarget(interactable);
            }

            return UniTask.CompletedTask;
        }
    }
}