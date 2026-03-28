using System;
using Gast.Lib.AI;

namespace Gast.Unity.Features.Stories
{
    /// <summary>
    /// Factory for creating a single named story action from its deserialized parameters object.
    /// Implement <see cref="IStoryActionFactory{TParams}"/> instead of this interface directly.
    /// </summary>
    public interface IStoryActionFactory
    {
        /// <summary>
        /// The action name as it appears in the story JSON, e.g. "WaitForCharacterDefeated".
        /// </summary>
        string ActionName { get; }

        /// <summary>
        /// The type that the "parameters" JSON object will be deserialized into before being
        /// passed to <see cref="Create"/>. Must match the concrete type accepted by <see cref="Create"/>.
        /// </summary>
        Type ParameterType { get; }

        /// <summary>
        /// Creates the action from already-deserialized parameters.
        /// <paramref name="parameters"/> is guaranteed to be an instance of <see cref="ParameterType"/>.
        /// </summary>
        IAction<StoryActorContext, StoryWorldState> Create(object parameters);
    }

    /// <summary>
    /// Convenience base class for <see cref="IStoryActionFactory"/> implementations.
    /// Handles the <see cref="IStoryActionFactory.ParameterType"/> property
    /// and the untyped <see cref="IStoryActionFactory.Create(object)"/>
    /// overload so subclasses only need to implement the typed <see cref="Create(TParams)"/> method.
    /// </summary>
    public interface IStoryActionFactory<in TParams> : IStoryActionFactory
        where TParams : class
    {
        Type IStoryActionFactory.ParameterType => typeof(TParams);

        IAction<StoryActorContext, StoryWorldState> IStoryActionFactory.Create(object parameters)
            => Create((TParams)parameters);

        IAction<StoryActorContext, StoryWorldState> Create(TParams parameters);
    }
}