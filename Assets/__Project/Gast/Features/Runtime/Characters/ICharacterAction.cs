using Gast.Domain.Characters;
using System;
using UnityEngine;

namespace Gast.Features.Characters
{
    /// <summary>
    /// Base non-generic interface for all character actions.
    /// </summary>
    public interface ICharacterAction
    {
        /// <summary>
        /// The type of command that triggers this action.
        /// </summary>
        Type CommandType { get; }

        /// <summary>
        /// Action priority for interruption handling.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Check if this action can be executed right now.
        /// </summary>
        bool CanExecute();

        /// <summary>
        /// Update logic called every frame while the action is active.
        /// </summary>
        /// <returns>True if the action should continue, false if it has finished.</returns>
        bool OnUpdate();

        /// <summary>
        /// Handle movement input while this action is active.
        /// </summary>
        void Move(Vector3 direction, float speed);

        /// <summary>
        /// Cleanup logic called when the action completes or is interrupted.
        /// </summary>
        void OnEnd();
    }

    /// <summary>
    /// Generic interface for character actions that are triggered by a specific command type.
    /// </summary>
    public interface ICharacterAction<TCommand> : ICharacterAction where TCommand : struct, ICharacterActionCommand
    {
        /// <summary>
        /// Begin executing this action with a specific command.
        /// </summary>
        void Execute(in TCommand command);
    }
}