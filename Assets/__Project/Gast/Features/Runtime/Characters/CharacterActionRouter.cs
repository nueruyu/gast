using System;
using System.Collections.Generic;
using Gast.Features.Characters.Actions.Commands;

namespace Gast.Features.Characters
{
    /// <summary>
    /// Manages action execution lifecycle and exclusive control via priority-based interruption.
    /// Only one action can be active at a time. Higher priority actions can interrupt lower priority ones.
    /// </summary>
    public class CharacterActionRouter
    {
        readonly Dictionary<Type, ICharacterAction> registeredActions = new();
        ICharacterAction currentAction;

        public bool IsActionRunning => currentAction != null;
        public ICharacterAction CurrentAction => currentAction;

        /// <summary>
        /// Register an action for later execution.
        /// </summary>
        public void Register(ICharacterAction action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            registeredActions[action.CommandType] = action;
        }

        /// <summary>
        /// Attempt to execute an action by command type.
        /// Handles precondition checks, interruption logic, and lifecycle management.
        /// </summary>
        public bool TryExecute<TCommand>(in TCommand command)
            where TCommand : struct, ICharacterActionCommand
        {
            if (!TryGetAction<TCommand>(out var action))
                return false;

            if (!action.CanExecute())
                return false;

            if (currentAction != null)
            {
                if (action.Priority <= currentAction.Priority)
                    return false;

                currentAction.OnEnd();
            }

            currentAction = action;
            action.Execute(in command);

            return true;
        }

        /// <summary>
        /// Forcibly stops the currently running action if it matches the command type.
        /// </summary>
        public void Stop<TCommand>() where TCommand : struct, IStateActionCommand
        {
            if (currentAction?.CommandType == typeof(TCommand))
            {
                CancelCurrent();
            }
        }

        /// <summary>
        /// Cancel the currently active action.
        /// </summary>
        public void CancelCurrent()
        {
            if (currentAction != null)
            {
                currentAction.OnEnd();
                currentAction = null;
            }
        }

        /// <summary>
        /// Update the currently active action.
        /// Call this every frame from MonoBehaviour.Update().
        /// </summary>
        public void Update()
        {
            if (currentAction != null)
            {
                if (!currentAction.OnUpdate())
                {
                    // Action finished naturally
                    currentAction.OnEnd();
                    currentAction = null;
                }
            }
        }

        public bool TryGetAction<TCommand>(out ICharacterAction<TCommand> action)
            where TCommand : struct, ICharacterActionCommand
        {
            action = default;

            if (!registeredActions.TryGetValue(typeof(TCommand), out var rawAction))
                return false;

            if (rawAction is not ICharacterAction<TCommand> typedAction)
                return false;

            action = typedAction;
            return true;
        }

        public bool IsActionActive<TCommand>() where TCommand : struct, ICharacterActionCommand
        {
            return currentAction != null && currentAction.CommandType == typeof(TCommand);
        }
    }
}