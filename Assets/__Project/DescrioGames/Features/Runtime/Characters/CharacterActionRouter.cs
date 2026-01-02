using System;
using System.Collections.Generic;

namespace DescrioGames.Features.Characters
{
    /// <summary>
    /// Manages action execution lifecycle and exclusive control via priority-based interruption.
    /// Only one action can be active at a time. Higher priority actions can interrupt lower priority ones.
    /// </summary>
    public class CharacterActionRouter
    {
        readonly Dictionary<Type, ICharacterAction> registeredActions = new();
        ICharacterAction currentAction;

        public bool IsActionRunning => currentAction != null && currentAction.IsActive;
        public ICharacterAction CurrentAction => currentAction;

        /// <summary>
        /// Register an action for later execution.
        /// </summary>
        public void Register(ICharacterAction action)
        {
            if (action == null)
                return;
            registeredActions[action.GetType()] = action;
        }

        /// <summary>
        /// Get a registered action by type.
        /// </summary>
        public T GetAction<T>() where T : class, ICharacterAction
        {
            return registeredActions.TryGetValue(typeof(T), out var action) ? action as T : null;
        }

        /// <summary>
        /// Attempt to execute an action by type.
        /// Handles precondition checks, interruption logic, and lifecycle management.
        /// </summary>
        public void TryExecute<T>() where T : class, ICharacterAction
        {
            if (!registeredActions.TryGetValue(typeof(T), out var newAction))
                return;

            // Check preconditions
            if (!newAction.CanExecute())
                return;

            // Handle interruption
            if (currentAction != null && currentAction.IsActive)
            {
                // Lower or equal priority cannot interrupt
                if (newAction.Priority <= currentAction.Priority)
                    return;

                // Interrupt current action
                currentAction.OnEnd();
            }

            // Execute new action
            currentAction = newAction;
            currentAction.Execute();
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
                if (currentAction.IsActive)
                {
                    currentAction.OnUpdate();
                }
                else
                {
                    // Action finished naturally
                    currentAction.OnEnd();
                    currentAction = null;
                }
            }
        }
    }
}