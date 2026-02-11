using Gast.Domain.AI;
using Gast.Domain.Interactions;
using GastGame.Actors;

namespace GastGame.AI
{
    public class AIMemory
    {
        public IActor CombatTarget { get; set; }
        public IInteractable InteractableTarget { get; set; }
        public IAIObjective CurrentObjective { get; set; }
    }
}