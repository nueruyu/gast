using Gast.Domain.AI;
using Gast.Domain.Interactions;
using GastGame.Domain.Characters;

namespace GastGame.AI
{
    public class AIMemory
    {
        public IGameCharacter CombatTarget { get; set; }
        public IInteractable InteractableTarget { get; set; }
        public IAIObjective CurrentObjective { get; set; }
    }
}