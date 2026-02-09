using Gast.Domain.AI;
using Gast.Domain.Interactions;
using Gast.Lib.AI;
using UnityEngine;

namespace GastGame.AI.Gathering
{
    public class GatheringState : IWorldState<GatheringState>
    {
        public IAIObjective CurrentGoal { get; set; }
        public bool HasGoal { get; set; }
        public bool HasInteractableTarget { get; set; }
        public InteractableId InteractableTargetId { get; set; }
        public Vector3 InteractableTargetPosition { get; set; }
        public bool IsInRangeToInteract { get; set; }
        public bool IsInCombat { get; set; }

        public void CopyFrom(GatheringState source)
        {
            CurrentGoal = source.CurrentGoal;
            HasGoal = source.HasGoal;
            HasInteractableTarget = source.HasInteractableTarget;
            InteractableTargetId = source.InteractableTargetId;
            InteractableTargetPosition = source.InteractableTargetPosition;
            IsInRangeToInteract = source.IsInRangeToInteract;
            IsInCombat = source.IsInCombat;
        }
    }
}
