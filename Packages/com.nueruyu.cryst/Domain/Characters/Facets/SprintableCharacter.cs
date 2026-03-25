using Gast.Domain.Characters;

namespace Cryst.Domain.Characters.Facets
{
    public class SprintableCharacter : ICharacterFacet
    {
        readonly CharacterActionStateStore stateStore;

        public SprintableCharacter(CharacterActionStateStore stateStore)
        {
            this.stateStore = stateStore;
        }

        public void SetSprint(bool isSprinting) => stateStore.IsSprinting = isSprinting;
    }
}
