using Gast.Domain.AI;
using Gast.Domain.Interactions;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Gathering
{
    public class GatheringState : IWorldState<GatheringState>
    {
        public AIMode CurrentMode { get; set; }
        public IAIObjective CurrentGoal { get; set; }
        public bool HasGoal { get; set; }
        public bool HasInteractableTarget { get; set; }
        public InteractableId InteractableTargetId { get; set; }
        public Vector3 InteractableTargetPosition { get; set; }
        public bool IsInRangeToInteract { get; set; }
        public bool IsInCombat { get; set; }

        public void WriteTo(ref GatheringState dest)
        {
            dest ??= new();
            dest.CurrentMode = CurrentMode;
            dest.CurrentGoal = CurrentGoal;
            dest.HasGoal = HasGoal;
            dest.HasInteractableTarget = HasInteractableTarget;
            dest.InteractableTargetId = InteractableTargetId;
            dest.InteractableTargetPosition = InteractableTargetPosition;
            dest.IsInRangeToInteract = IsInRangeToInteract;
            dest.IsInCombat = IsInCombat;
        }
    }
}