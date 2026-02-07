using System;
using UnityEngine;
using Gast.Features.Characters.Actions.Commands;

namespace Gast.Features.Characters.Actions
{
    /// <summary>
    /// Guard action that allows reduced-speed movement while guarding.
    /// Reads latest input from CharacterActionController and applies movement penalty.
    /// </summary>
    public class GuardAction : IStatefulCharacterAction
    {
        readonly CharacterBody body;
        readonly CharacterAnimator animator;
        readonly float moveSpeedPenalty = 0.5f;

        bool isActive;

        public Type CommandType => typeof(GuardCommand);
        public int Priority => 2;
        public bool IsActive => isActive;

        public GuardAction(CharacterContext character)
        {
            this.body = character.Body;
            this.animator = character.Animator;
        }

        public bool CanExecute() => true;

        public void Execute()
        {
            isActive = true;
            animator?.SetGuard(true);
        }

        public void Stop()
        {
            isActive = false;
        }

        public void OnUpdate()
        {
        }

        public void Move(Vector3 direction, float speed)
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                body.SetInputVelocity(direction * (speed * moveSpeedPenalty));
                body.SetLookDirection(direction, 5f);
            }
        }

        public void OnEnd()
        {
            isActive = false;
            animator?.SetGuard(false);
        }
    }
}