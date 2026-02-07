using System;
using UnityEngine;

namespace Gast.Features.Characters
{
    /// <summary>
    /// Base interface for all character actions.
    /// Actions are discrete, composable units with clear preconditions, execution logic, and lifecycle.
    /// </summary>
    public interface ICharacterAction
    {
        /// <summary>
        /// The type of command that triggers this action.
        /// Returns null if this action is not triggered by a command (e.g., DieAction, HitAction).
        /// </summary>
        Type CommandType { get; }

        /// <summary>
        /// Whether this action is currently active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Action priority for interruption handling.
        /// Higher priority actions can interrupt lower priority ones.
        /// Example: Movement=0, Guard=2, Attack=5, Dash=10, Damage=99
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Check if this action can be executed right now.
        /// Called before Execute() to validate preconditions (cooldowns, state, etc.).
        /// </summary>
        bool CanExecute();

        /// <summary>
        /// Begin executing this action.
        /// Called once when the action starts.
        /// </summary>
        void Execute();

        /// <summary>
        /// Update logic called every frame while the action is active.
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// Handle movement input while this action is active.
        /// Each action can decide how to handle movement (allow, restrict, modify speed, etc.).
        /// </summary>
        void Move(Vector3 direction, float speed);

        /// <summary>
        /// Cleanup logic called when the action completes or is interrupted.
        /// </summary>
        void OnEnd();
    }
}