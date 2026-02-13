using Gast.Features.Characters;
using GastGame.Features.Characters;
using System;
using UnityEngine;

namespace GastGame.Features.CharacterActions
{
    /// <summary>
    /// Guard action that allows reduced-speed movement while guarding.
    /// Reads latest input from CharacterActionController and applies movement penalty.
    /// </summary>
    public class GuardAction : ICharacterExecutableAction<GuardCommand>
    {
        readonly CharacterActionContext context;
        readonly GuardActionSettings settings;

        public Type CommandType => typeof(GuardCommand);
        public int Priority => 2;

        public GuardAction(CharacterActionContext context, GuardActionSettings settings)
        {
            this.context = context;
            this.settings = settings;
        }

        public bool CanExecute() => true;

        public void Execute(in GuardCommand command)
        {
            context.CharacterAnimator?.SetGuard(true);
        }

        public bool OnUpdate()
        {
            return true;
        }

        public void Move(Vector3 direction)
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                var speed = context.CharacterContext.TypeDefinition.WalkSpeed * settings.MoveSpeedPenalty;
                context.CharacterContext.Body.SetInputVelocity(direction * speed);
                context.CharacterContext.Body.SetLookDirection(direction, settings.LookDirectionSpeed);
            }
        }

        public void OnEnd()
        {
            context.CharacterAnimator?.SetGuard(false);
        }
    }
}