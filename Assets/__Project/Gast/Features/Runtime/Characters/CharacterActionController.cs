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
        readonly Dictionary<Type, ICharacterAction> triggerActionMap = new();
        readonly Dictionary<Type, IStatefulCharacterAction> stateActionMap = new();

        public CharacterActionController(CharacterContext character)
        {
            this.character = character;
        }

        public void RegisterAction(ICharacterAction action)
        {
            router.Register(action);

            if (action is IStatefulCharacterAction statefulAction)
            {
                stateActionMap[action.CommandType] = statefulAction;
            }
            else
            {
                triggerActionMap[action.CommandType] = action;
            }
        }

        public void Dispatch<TCommand>(in TCommand command) where TCommand : struct, ITriggerActionCommand
        {
            var commandType = typeof(TCommand);

            if (triggerActionMap.TryGetValue(commandType, out var triggerAction))
            {
                router.TryExecute(triggerAction);
            }
        }

        public void Start<TCommand>(in TCommand command) where TCommand : struct, IStateActionCommand
        {
            var commandType = typeof(TCommand);

            if (stateActionMap.TryGetValue(commandType, out var stateAction))
            {
                router.TryExecute(stateAction);
            }
        }

        public void Stop<TCommand>() where TCommand : struct, IStateActionCommand
        {
            var commandType = typeof(TCommand);

            if (stateActionMap.TryGetValue(commandType, out var stateAction))
            {
                stateAction.Stop();
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