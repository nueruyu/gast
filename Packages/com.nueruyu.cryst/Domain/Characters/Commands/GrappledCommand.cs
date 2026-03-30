using Gast.Domain.Characters;

namespace Cryst.Domain.Characters.Commands
{
    public readonly struct GrappledCommand : ICharacterTriggerCommand
    {
        public ICharacter Attacker { get; }

        public GrappledCommand(ICharacter attacker)
        {
            Attacker = attacker;
        }
    }
}
