using System;
using Gast.Lib.AI;

namespace Cryst.Features.Stories
{
    /// <summary>
    /// Convenience base class for <see cref="IStoryActionFactory"/> implementations.
    /// Handles the <see cref="ParameterType"/> property and the untyped <see cref="Create(object)"/>
    /// overload so subclasses only need to implement the typed <see cref="Create(TParams)"/> method.
    /// </summary>
    public abstract class StoryActionFactory<TParams> : IStoryActionFactory
        where TParams : class
    {
        public abstract string ActionName { get; }

        public Type ParameterType => typeof(TParams);

        public IAction<StoryActorContext, StoryWorldState> Create(object parameters)
            => Create((TParams)parameters);

        protected abstract IAction<StoryActorContext, StoryWorldState> Create(TParams parameters);
    }
}
