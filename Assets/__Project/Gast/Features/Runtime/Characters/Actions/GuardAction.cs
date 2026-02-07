using System;
using UnityEngine;
using Gast.Features.Characters.Actions.Commands;

namespace Gast.Features.Characters.Actions
{
    /// <summary>
    /// Guard action that allows reduced-speed movement while guarding.
    /// Reads latest input from CharacterActionController and applies movement penalty.
    /// </summary>
    public class GuardAction : ICharacterAction<GuardCommand>
    {
        readonly CharacterBody body;
        readonly CharacterAnimator animator;
        readonly float moveSpeedPenalty = 0.5f;

        public Type CommandType => typeof(GuardCommand);
        public int Priority => 2;

        public GuardAction(CharacterContext character)
        {
            this.body = character.Body;
            this.animator = character.Animator;
        }

        public bool CanExecute() => true;

        public void Execute(in GuardCommand command)
        {
            animator?.SetGuard(true);
        }

        public bool OnUpdate()
        {
            return true;
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
            animator?.SetGuard(false);
        }
    }
}