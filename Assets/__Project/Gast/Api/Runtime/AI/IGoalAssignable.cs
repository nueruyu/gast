using System.Collections.Generic;
using Gast.Domain.Npcs.Goals;

namespace Gast.Domain.Npcs
{
    /// <summary>
    /// Interface for brain components that can receive goal assignments.
    /// </summary>
    public interface IGoalAssignable
    {
        void SetGoals(List<IGoal> goals);
    }
}
