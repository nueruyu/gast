using System;
using System.Collections.Generic;
using Gast.Domain.AI;

namespace Gast.Application.AIPlanning
{
    public readonly struct AIPlanningResult
    {
        public List<IAIObjective> Objectives { get; }

        public AIPlanningResult(List<IAIObjective> objectives)
        {
            Objectives = objectives;
        }
    }

    public class AIPlanningException : Exception
    {
        public AIPlanningException(string message) : base(message)
        {
        }

        public AIPlanningException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}