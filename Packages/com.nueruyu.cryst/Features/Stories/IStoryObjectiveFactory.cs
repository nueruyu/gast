using System;
using Gast.Domain.AI;

namespace Cryst.Features.Stories
{
    public interface IStoryObjectiveFactory
    {
        string ObjectiveType { get; }
        Type ParameterType { get; }
        IAIObjective Create(object parameters);
    }
}
