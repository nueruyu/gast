using Gast.Domain.AI;
using Gast.Domain.Interactions;
using Cryst.Domain.Characters;

namespace Cryst.Modules.CharacterAI
{
    public class AIMemory
    {
        public ICrystCharacter CombatTarget { get; set; }
        public IInteractable InteractableTarget { get; set; }
        public IAIObjective CurrentObjective { get; set; }
    }
}