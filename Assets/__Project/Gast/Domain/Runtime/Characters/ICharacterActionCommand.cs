namespace Gast.Domain.Characters
{
    /// <summary>
    /// Base marker interface for character action commands.
    /// Commands are lightweight structs that trigger actions via the CharacterActionController.
    /// </summary>
    public interface ICharacterActionCommand
    {
    }

    /// <summary>
    /// For commands that trigger an action once.
    /// </summary>
    public interface ICharacterTriggerCommand : ICharacterActionCommand
    {
    }

    /// <summary>
    /// For commands that manage an action's state (e.g., on/off).
    /// </summary>
    public interface ICharacterStateCommand : ICharacterActionCommand
    {
    }
}