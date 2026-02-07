using System;
using System.Collections.Generic;
using UnityEngine;
using Gast.Domain.Combat;
using Gast.Features.Characters.Actions;
using Gast.Features.Characters.Actions.Commands;

namespace Gast.Features.Characters
{
    /// <summary>
    /// Manages character actions through an ActionRouter.
    /// Provides command-based dispatch for action execution.
    /// </summary>
    public class CharacterActionController
    {
        readonly CharacterActionRouter router = new();
        readonly CharacterContext character;
        readonly Dictionary<Type, ICharacterAction> commandActionMap = new();

        public CharacterActionController(CharacterContext character)
        {
            this.character = character;
        }

        public void RegisterAction(ICharacterAction action)
        {
            router.Register(action);
            if (action.CommandType != null)
            {
                commandActionMap[action.CommandType] = action;
            }
        }

        /// <summary>
        /// Dispatch a command to trigger the corresponding action.
        /// </summary>
        public void Dispatch<TCommand>(in TCommand command) where TCommand : struct, ICharacterActionCommand
        {
            if (command is SetGuardCommand setGuardCmd)
            {
                HandleSetGuard(setGuardCmd);
                return;
            }

            if (commandActionMap.TryGetValue(typeof(TCommand), out var action))
            {
                router.TryExecute(action);
            }
        }

        void HandleSetGuard(in SetGuardCommand command)
        {
            if (!commandActionMap.TryGetValue(typeof(SetGuardCommand), out var action) || action is not GuardAction guardAction)
            {
                return;
            }

            if (command.IsActive)
            {
                router.TryExecute(guardAction);
            }
            else
            {
                if (guardAction.IsActive)
                {
                    guardAction.ManualStop();
                }
            }
        }

        /// <summary>
        /// Handle movement input.
        /// If an action is running, delegate to the action's Move method.
        /// Otherwise, apply normal movement.
        /// </summary>
        public void Move(Vector3 direction, float speed)
        {
            if (router.IsActionRunning)
            {
                router.CurrentAction?.Move(direction, speed);
                return;
            }

            var body = character.Body;
            body.SetInputVelocity(direction * speed);
            body.SetLookDirection(direction, 10f);
        }

        public void Die() => router.TryExecute<DieAction>();

        /// <summary>
        /// Apply hit reaction with damage information.
        /// This interrupts most actions due to HitAction's high priority.
        /// </summary>
        public void TakeHit(DamageInfo info)
        {
            var hitAction = router.GetAction<HitAction>();
            if (hitAction != null)
            {
                hitAction.Setup(info);
                router.TryExecute(hitAction);
            }
        }

        /// <summary>
        /// Update active actions. Call this from Character.Update().
        /// </summary>
        public void Update()
        {
            router.Update();
        }

        public bool CanAttack => CanActionExecute<AttackAction>();
        public bool CanGuard => CanActionExecute<GuardAction>();
        public bool IsDashing => IsActionActive<DashAction>();
        public bool IsGuarding => IsActionActive<GuardAction>();

        bool IsActionActive<T>() where T : class, ICharacterAction
        {
            var action = router.GetAction<T>();
            return action != null && action.IsActive;
        }

        bool CanActionExecute<T>() where T : class, ICharacterAction
        {
            var action = router.GetAction<T>();
            return action != null && action.CanExecute();
        }
    }
}
