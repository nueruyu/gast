using Gast.Features.Characters;
using System;
using UnityEngine;

namespace GastGame.Features.CharacterActions
{
    /// <summary>
    /// Guard action that allows reduced-speed movement while guarding.
    /// Reads latest input from CharacterActionController and applies movement penalty.
    /// </summary>
    public class GuardAction : ICharacterAction<GuardCommand>
    {
        readonly CharacterBody body;
        readonly CharacterAnimator animator;
        readonly GuardActionSettings settings;

        public Type CommandType => typeof(GuardCommand);
        public int Priority => 2;

        public GuardAction(CharacterContext character, GuardActionSettings settings)
        {
            body = character.Body;
            animator = character.Animator;
            this.settings = settings;
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
                body.SetInputVelocity(direction * (speed * settings.MoveSpeedPenalty));
                body.SetLookDirection(direction, settings.LookDirectionSpeed);
            }
        }

        public void OnEnd()
        {
            animator?.SetGuard(false);
        }
    }
}