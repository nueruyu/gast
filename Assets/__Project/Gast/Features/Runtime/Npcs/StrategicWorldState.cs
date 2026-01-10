using Gast.Api.AI;
using Gast.Domain.Interactions;
using UnityEngine;

namespace Gast.Features.Npcs
{
    public struct StrategicWorldState
    {
        // Goal
        public IGoal CurrentGoal { get; set; }
        public bool HasGoal { get; set; }

        // Interactable Target
        public bool HasInteractableTarget { get; set; }
        public InteractableId InteractableTargetId { get; set; }
        public Vector3 InteractableTargetPosition { get; set; }
        public bool IsInRangeToInteract { get; set; }
    }
}
