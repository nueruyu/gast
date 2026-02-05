using System.Collections.Generic;

namespace Gast.Application.AI
{
    public interface IObjectiveRegistry
    {
        List<ObjectiveDefinition> GetObjectiveDefinitions();
    }
}