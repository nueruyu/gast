using System;
using Gast.Domain.AI;

namespace Gast.Unity.Features.Stories
{
    public interface IStoryObjectiveFactory
    {
        string ObjectiveType { get; }
        Type ParameterType { get; }
        IAIObjective Create(object parameters);
    }

    public interface IStoryObjectiveFactory<in TParams> : IStoryObjectiveFactory where TParams : class
    {
        Type IStoryObjectiveFactory.ParameterType => typeof(TParams);
        IAIObjective IStoryObjectiveFactory.Create(object parameters) => Create((TParams)parameters);
        IAIObjective Create(TParams parameters);
    }
}