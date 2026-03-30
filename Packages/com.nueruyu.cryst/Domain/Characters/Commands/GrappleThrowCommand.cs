using Gast.Domain.Characters;

namespace Cryst.Domain.Characters.Commands
{
    public readonly struct GrappleThrowCommand : ICharacterTriggerCommand
    {
        public ICharacter Victim { get; }

        public GrappleThrowCommand(ICharacter victim)
        {
            Victim = victim;
        }
    }
}
