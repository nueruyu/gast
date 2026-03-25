using Cryst.Domain.Characters.Commands;
using Gast.Domain.Characters;

namespace Cryst.Domain.Characters.Facets
{
    public class JumpableCharacter : ICharacterFacet
    {
        readonly ICharacterActionController actionController;

        public JumpableCharacter(ICharacterActionController actionController)
        {
            this.actionController = actionController;
        }

        public void Jump() => actionController.ExecuteAction(new JumpCommand());
    }
}
