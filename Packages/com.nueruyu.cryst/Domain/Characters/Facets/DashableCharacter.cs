using Cryst.Domain.Characters.Commands;
using Gast.Domain.Characters;
using UnityEngine;

namespace Cryst.Domain.Characters.Facets
{
    public class DashableCharacter : ICharacterFacet
    {
        readonly ICharacterActionController actionController;

        public DashableCharacter(ICharacterActionController actionController)
        {
            this.actionController = actionController;
        }

        public void Dash(Vector3 direction) => actionController.ExecuteAction(new DashCommand(direction));
    }
}
