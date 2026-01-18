using Gast.Domain.Characters;
using Gast.Domain.Interactions;

namespace Gast.Features.Npcs
{
    public class AIMemory
    {
        public ICharacter CombatTarget { get; set; }
        public IInteractable InteractableTarget { get; set; }
    }
}
