using Cryst.Domain.Characters.Commands;
using Gast.Domain.Characters;

namespace Cryst.Domain.Characters.Facets
{
    public class AttackableCharacter : ICharacterFacet
    {
        readonly ICharacterActionController actionController;

        public AttackableCharacter(ICharacterActionController actionController)
        {
            this.actionController = actionController;
        }

        public bool CanAttack() => actionController.CanExecuteAction<AttackCommand>();
        public void Attack() => actionController.ExecuteAction(new AttackCommand());
    }
}
