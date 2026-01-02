using UnityEngine;
using Gast.Domain.Combat;
using Gast.Features.Characters.Actions;

namespace Gast.Features.Characters
{
    /// <summary>
    /// Manages character actions through an ActionRouter.
    /// Provides facade methods for action execution.
    /// </summary>
    public class CharacterActionController
    {
        readonly CharacterActionRouter router = new();
        readonly CharacterContext character;

        public CharacterActionController(CharacterContext character)
        {
            this.character = character;
        }

        public void RegisterAction(ICharacterAction action)
        {
            router.Register(action);
        }

        /// <summary>
        /// Handle movement input.
        /// If an action is running, delegate to the action's Move method.
        /// Otherwise, apply normal movement.
        /// </summary>
        public void Move(Vector3 direction, float speed)
        {
            // If an action is running, let it handle movement
            if (router.IsActionRunning)
            {
                router.CurrentAction?.Move(direction, speed);
                return;
            }

            // Apply normal movement
            var body = character.Body;
            body.SetInputVelocity(direction * speed);
            body.SetLookDirection(direction, 10f);
        }

        public void Dash() => router.TryExecute<DashAction>();

        public void Attack() => router.TryExecute<AttackAction>();

        public void Guard() => router.TryExecute<GuardAction>();

        public void Die() => router.TryExecute<DieAction>();

        /// <summary>
        /// Stop guard action if it's currently active.
        /// Called when guard button is released.
        /// </summary>
        public void StopGuard()
        {
            var guardAction = router.GetAction<GuardAction>();
            if (guardAction != null && guardAction.IsActive)
            {
                guardAction.ManualStop();
            }
        }

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
                router.TryExecute<HitAction>();
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