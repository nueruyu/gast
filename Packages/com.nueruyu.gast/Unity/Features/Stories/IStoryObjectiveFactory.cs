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
}
