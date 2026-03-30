using Cryst.Domain.Characters.Commands;
using Gast.Domain.Characters;

namespace Cryst.Domain.Characters.Facets
{
    public class GrappleableCharacter : ICharacterFacet
    {
        readonly ICharacterActionController actionController;

        public GrappleableCharacter(ICharacterActionController actionController)
        {
            this.actionController = actionController;
        }

        public bool CanGrapple() => actionController.CanExecuteAction<GrappleCommand>();
        public void Grapple() => actionController.ExecuteAction(new GrappleCommand());
        public void StartThrow(ICharacter victim) => actionController.ExecuteAction(new GrappleThrowCommand(victim));
    }
}
