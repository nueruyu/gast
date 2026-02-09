using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;

namespace GastGame.AI
{
    public class AIMemory
    {
        public ICharacter CombatTarget { get; set; }
        public IInteractable InteractableTarget { get; set; }
        public IAIObjective CurrentObjective { get; set; }
    }
}