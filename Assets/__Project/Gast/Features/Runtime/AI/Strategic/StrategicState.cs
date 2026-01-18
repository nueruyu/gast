using Gast.Api.AI;
using Gast.Domain.Interactions;
using Gast.Lib.AI;
using UnityEngine;

namespace Gast.Features.AI.Strategic
{
    public class StrategicState : IWorldState<StrategicState>
    {
        public IGoal CurrentGoal { get; set; }
        public bool HasGoal { get; set; }
        public bool HasInteractableTarget { get; set; }
        public InteractableId InteractableTargetId { get; set; }
        public Vector3 InteractableTargetPosition { get; set; }
        public bool IsInRangeToInteract { get; set; }
        public bool IsThreatened { get; set; }

        public void CopyFrom(StrategicState source)
        {
            CurrentGoal = source.CurrentGoal;
            HasGoal = source.HasGoal;
            HasInteractableTarget = source.HasInteractableTarget;
            InteractableTargetId = source.InteractableTargetId;
            InteractableTargetPosition = source.InteractableTargetPosition;
            IsInRangeToInteract = source.IsInRangeToInteract;
            IsThreatened = source.IsThreatened;
        }
    }
}