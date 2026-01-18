using System.Linq;
using Gast.Domain.AI;
using Gast.Domain.Inputs;

namespace Gast.Features.Players
{
    /// <summary>
    /// Pure C# monitor that checks for goal completion and player input.
    /// Runs in PlayerManager's update loop.
    /// </summary>
    public class AIControlMonitor
    {
        readonly IInputProvider inputProvider;

        IAIGoalController currentTarget;

        public AIControlMonitor(IInputProvider inputProvider)
        {
            this.inputProvider = inputProvider;
        }

        /// <summary>
        /// Set the current monitoring target. Pass null to stop monitoring.
        /// </summary>
        public void SetTarget(IAIGoalController target)
        {
            currentTarget = target;
        }

        /// <summary>
        /// Check if monitoring should trigger restoration.
        /// Call this every frame from PlayerManager.
        /// </summary>
        /// <returns>True if restoration should occur</returns>
        public bool ShouldRestore()
        {
            if (currentTarget == null)
                return false;

            // Priority 1: Check for player input
            if (HasPlayerInput())
                return true;

            // Priority 2: Check for goal completion
            if (AreAllGoalsCompleted())
                return true;

            return false;
        }

        bool HasPlayerInput()
        {
            return inputProvider.Move.sqrMagnitude > 0.01f
                || inputProvider.Look.sqrMagnitude > 0.01f
                || inputProvider.Jump
                || inputProvider.Sprint
                || inputProvider.InteractPressed
                || inputProvider.Attack
                || inputProvider.Dash
                || inputProvider.GuardHeld;
        }

        bool AreAllGoalsCompleted()
        {
            var goals = currentTarget.CurrentGoals;

            if (goals == null || goals.Count == 0)
                return true;

            return goals.All(g => g.IsCompleted);
        }
    }
}