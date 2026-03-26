using Gast.Lib.AI;

namespace Cryst.Features.Stories
{
    /// <summary>
    /// Factory for creating a single named story action from its JSON parameters string.
    /// Implement this interface and register with DI to add new story actions.
    /// </summary>
    public interface IStoryActionFactory
    {
        /// <summary>
        /// The action name as it appears in the story JSON, e.g. "WaitForCharacterDefeated".
        /// </summary>
        string ActionName { get; }

        /// <summary>
        /// Creates the action. <paramref name="parametersJson"/> is the raw JSON object string
        /// of the "parameters" field from the primitive task definition.
        /// Implementations are responsible for deserializing their own parameters.
        /// </summary>
        IAction<StoryActorContext, StoryWorldState> Create(string parametersJson);
    }
}
