using System.Collections.Generic;

namespace Gast.Domain.AI
{
    /// <summary>
    /// Interface for brain components that can receive and report goal assignments.
    /// </summary>
    public interface IAIGoalController
    {
        void SetGoals(IEnumerable<IGoal> goals);

        IReadOnlyList<IGoal> CurrentGoals { get; }
    }
}