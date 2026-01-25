using System.Collections.Generic;
using Gast.Application.AI.Models;

namespace Gast.Application.AI.Objectives
{
    public interface IObjectiveRegistry
    {
        List<ObjectiveDefinition> GetObjectiveDefinitions();
    }
}
