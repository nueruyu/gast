using System.Collections.Generic;

namespace Gast.Api.AI
{
    /// <summary>
    /// Interface for brain components that can receive goal assignments.
    /// </summary>
    public interface IGoalAssignable
    {
        void SetGoals(IEnumerable<IGoal> goals);
    }
}