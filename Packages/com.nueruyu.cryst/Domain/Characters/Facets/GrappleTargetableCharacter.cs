using Cryst.Domain.Characters.Commands;
using Gast.Domain.Characters;

namespace Cryst.Domain.Characters.Facets
{
    public class GrappleTargetableCharacter : ICharacterFacet
    {
        readonly ICharacterActionController actionController;

        public GrappleTargetableCharacter(ICharacterActionController actionController)
        {
            this.actionController = actionController;
        }

        public void GetGrappledBy(ICharacter attacker) => actionController.ExecuteAction(new GrappledCommand(attacker));
    }
}
