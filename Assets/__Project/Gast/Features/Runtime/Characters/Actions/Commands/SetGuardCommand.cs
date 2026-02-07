namespace Gast.Features.Characters.Actions.Commands
{
    public readonly struct SetGuardCommand : ICharacterActionCommand
    {
        public bool IsActive { get; }

        public SetGuardCommand(bool isActive)
        {
            IsActive = isActive;
        }
    }
}
