using Cryst.Domain.Characters;
using Gast.Domain.AI;
using Gast.Domain.Interactions;

namespace Cryst.Features.CharacterAI
{
    public class AIMemory
    {
        public BaseCharacter CombatTarget { get; set; }
        public IInteractable InteractableTarget { get; set; }
        public IAIObjective CurrentObjective { get; set; }
        public bool HasTarget => CombatTarget != null && CombatTarget.Status.IsAlive.Value;
    }
}