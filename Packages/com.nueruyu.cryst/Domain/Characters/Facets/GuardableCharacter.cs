using Cryst.Domain.Characters.Commands;
using Gast.Domain.Characters;

namespace Cryst.Domain.Characters.Facets
{
    public class GuardableCharacter : ICharacterFacet
    {
        readonly ICharacterActionController actionController;

        public GuardableCharacter(ICharacterActionController actionController)
        {
            this.actionController = actionController;
        }

        public bool CanGuard() => actionController.CanExecuteAction<GuardCommand>();
        public void StartGuard() => actionController.StartAction(new GuardCommand());
        public void StopGuard() => actionController.StopAction<GuardCommand>();
    }
}
