using System.Linq;
using Cryst.Domain.AI.Objectives;
using Cryst.Domain.Characters;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Interactions;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid
{
    public class HumanoidMemory
    {
        IAIObjective CurrentObjective { get; set; }
        public AIMode CurrentMode { get; private set; } = AIMode.Idle;
        public BaseCharacter ObjectiveCombatTarget { get; private set; }
        public IInteractable InteractableTarget { get; private set; }

        public BaseCharacter ThreatTarget { get; private set; }

        public ItemId? TargetItemId =>  (CurrentObjective as AcquireItemObjective)?.TargetItemId;
        public bool HasGatheringTarget => InteractableTarget != null;

        public bool HasObjectiveCombatTarget =>
            ObjectiveCombatTarget != null && ObjectiveCombatTarget.Status.IsAlive.Value;

        public bool IsThreatened =>
            ThreatTarget != null && ThreatTarget.Status.IsAlive.Value;

        public void Update(BaseCharacter actor)
        {
            ThreatTarget = actor.VisionSensor.VisibleCharacters
                .Select(c => c.As<BaseCharacter>())
                .Where(a => a.IsThreatTo(actor))
                .OrderBy(a => Vector3.Distance(actor.VisionSensor.EyePosition, a.Body.Position))
                .FirstOrDefault();

            if (CurrentObjective?.IsCompleted.Value == true)
                ClearObjective();

            if (InteractableTarget is Component interactableComponent && !interactableComponent)
                InteractableTarget = null;
        }

        public void SetMode(AIMode mode)
        {
            CurrentMode = mode;
        }

        public void SetObjective(IAIObjective objective)
        {
            CurrentObjective = objective;
        }

        public void SetObjectiveCombatTarget(BaseCharacter character)
        {
            ObjectiveCombatTarget = character;
        }

        public void SetInteractableTarget(IInteractable interactable)
        {
            InteractableTarget = interactable;
        }

        void ClearObjective()
        {
            CurrentObjective = null;
            ObjectiveCombatTarget = null;
            InteractableTarget = null;
        }
    }
}