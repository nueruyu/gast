using Gast.Domain.Characters;
using Gast.Domain.Interactions;

namespace Gast.Features.Npcs
{
    public class SharedAIState
    {
        public ICharacter StrategicTarget { get; set; }
        public IInteractable InteractableTarget { get; set; }
    }
}
