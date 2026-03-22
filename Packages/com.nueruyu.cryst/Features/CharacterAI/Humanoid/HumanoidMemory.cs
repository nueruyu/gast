using Cryst.Domain.Characters;
using Gast.Domain.AI;
using Gast.Domain.Interactions;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid
{
    public class HumanoidMemory
    {
        public AIMode CurrentMode { get; private set; } = AIMode.Idle;
        public BaseCharacter ThreatTarget { get; private set; }
        public BaseCharacter ObjectiveCombatTarget { get; private set; }
        public IInteractable InteractableTarget { get; private set; }
        public IAIObjective CurrentObjective { get; private set; }
        public bool HasThreatTarget => ThreatTarget != null && ThreatTarget.Status.IsAlive.Value;
        public bool HasObjectiveCombatTarget => ObjectiveCombatTarget != null && ObjectiveCombatTarget.Status.IsAlive.Value;

        public void SetMode(AIMode mode)
        {
            CurrentMode = mode;
        }

        public void SetObjective(IAIObjective objective)
        {
            CurrentObjective = objective;
        }

        public void SetThreatTarget(BaseCharacter character)
        {
            ThreatTarget = character;
        }

        public void SetObjectiveCombatTarget(BaseCharacter character)
        {
            ObjectiveCombatTarget = character;
        }

        public void SetInteractableTarget(IInteractable interactable)
        {
            InteractableTarget = interactable;
        }

        public void PurgeInteractableTarget()
        {
            if (InteractableTarget is Component interactableTargetComponent &&
                !interactableTargetComponent)
                InteractableTarget = null;
        }

        public void ClearThreat()
        {
            ThreatTarget = null;
        }

        public void ClearObjective()
        {
            CurrentObjective = null;
            ObjectiveCombatTarget = null;
            InteractableTarget = null;
        }

        public void Clear()
        {
            ThreatTarget = null;
            ObjectiveCombatTarget = null;
            CurrentObjective = null;
            InteractableTarget = null;
        }
    }
}
