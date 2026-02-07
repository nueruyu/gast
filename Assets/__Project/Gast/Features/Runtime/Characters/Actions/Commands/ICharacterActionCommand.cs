namespace Gast.Features.Characters.Actions.Commands
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
    public interface ITriggerActionCommand : ICharacterActionCommand
    {
    }

    /// <summary>
    /// For commands that manage an action's state (e.g., on/off).
    /// </summary>
    public interface IStateActionCommand : ICharacterActionCommand
    {
    }
}