using Gast.Domain.Characters;
using System;
using System.Collections.Generic;

namespace Gast.Features.Characters
{
    /// <summary>
    /// Manages action execution lifecycle and exclusive control via priority-based interruption.
    /// Only one action can be active at a time. Higher priority actions can interrupt lower priority ones.
    /// </summary>
    public class CharacterActionRouter
    {
        readonly Dictionary<Type, ICharacterExecutableAction> registeredActions = new();
        ICharacterAction defaultAction;
        ICharacterExecutableAction activeAction;

        public ICharacterAction CurrentAction => activeAction ?? defaultAction;

        public void RegisterDefaultAction(ICharacterAction action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (defaultAction is not null)
                throw new InvalidOperationException();

            defaultAction = action;
        }

        /// <summary>
        /// Register an action for later execution.
        /// Actions with null CommandType are treated as the default action.
        /// </summary>
        public void Register(ICharacterExecutableAction action)
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

            var active = activeAction ?? defaultAction;
            if (active != null)
            {
                if (action.Priority <= active.Priority)
                    return false;

                active.OnEnd();
            }

            activeAction = action;
            action.Execute(in command);

            return true;
        }

        /// <summary>
        /// Forcibly stops the currently running action if it matches the command type.
        /// </summary>
        public void Stop<TCommand>() where TCommand : struct, ICharacterStateCommand
        {
            if (activeAction?.CommandType == typeof(TCommand))
            {
                CancelCurrent();
            }
        }

        /// <summary>
        /// Cancel the currently active action.
        /// </summary>
        public void CancelCurrent()
        {
            if (activeAction != null)
            {
                activeAction.OnEnd();
                activeAction = null;
            }
        }

        /// <summary>
        /// Update the currently active action.
        /// Call this every frame from MonoBehaviour.Update().
        /// </summary>
        public void Update()
        {
            if (activeAction != null)
            {
                if (!activeAction.OnUpdate())
                {
                    // Action finished naturally
                    activeAction.OnEnd();
                    activeAction = null;
                }
            }
        }

        public bool TryGetAction<TCommand>(out ICharacterExecutableAction<TCommand> action)
            where TCommand : struct, ICharacterActionCommand
        {
            action = default;

            if (!registeredActions.TryGetValue(typeof(TCommand), out var rawAction))
                return false;

            if (rawAction is not ICharacterExecutableAction<TCommand> typedAction)
                return false;

            action = typedAction;
            return true;
        }

        public bool IsActionActive<TCommand>() where TCommand : struct, ICharacterActionCommand
        {
            return activeAction != null && activeAction.CommandType == typeof(TCommand);
        }
    }
}