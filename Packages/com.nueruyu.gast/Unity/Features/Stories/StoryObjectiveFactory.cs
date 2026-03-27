using System;
using Gast.Domain.AI;

namespace Gast.Unity.Features.Stories
{
    public abstract class StoryObjectiveFactory<TParams> : IStoryObjectiveFactory where TParams : class
    {
        public abstract string ObjectiveType { get; }
        public Type ParameterType => typeof(TParams);
        public IAIObjective Create(object parameters) => Create((TParams)parameters);
        protected abstract IAIObjective Create(TParams parameters);
    }
}
