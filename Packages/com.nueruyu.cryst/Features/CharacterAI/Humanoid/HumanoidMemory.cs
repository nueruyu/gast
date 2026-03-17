using Cryst.Domain.Characters;
using Gast.Domain.AI;
using Gast.Domain.Interactions;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid
{
    public class HumanoidMemory
    {
        public AIMode CurrentMode { get; private set; } = AIMode.Idle;
        public BaseCharacter CombatTarget { get; private set; }
        public IInteractable InteractableTarget { get; private set; }
        public IAIObjective CurrentObjective { get; private set; }
        public bool HasTarget => CombatTarget != null && CombatTarget.Status.IsAlive.Value;

        public void SetMode(AIMode mode)
        {
            CurrentMode = mode;
        }

        public void SetObjective(IAIObjective objective)
        {
            CurrentObjective = objective;
        }

        public void SetCombatTarget(BaseCharacter character)
        {
            CombatTarget = character;
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

        public void Clear()
        {
            CombatTarget = null;
            CurrentObjective = null;
            InteractableTarget = null;
        }
    }
}