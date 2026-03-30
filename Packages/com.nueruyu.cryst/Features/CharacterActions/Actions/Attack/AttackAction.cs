using System;
using Cryst.Domain.Characters.Commands;
using Gast.Unity.Features.Characters;

namespace Cryst.Features.CharacterActions.Actions.Attack
{
    public class AttackAction : AttackActionBase, ICharacterExecutableAction<AttackCommand>
    {
        public override Type CommandType => typeof(AttackCommand);

        public AttackAction(
            AttackActionSettings settings,
            CharacterAnimator animator,
            CharacterMovement movement,
            CharacterMovementSettings movementSettings,
            CharacterActionEffectDispatcher effectDispatcher)
            : base(settings, animator, movement, movementSettings, effectDispatcher)
        {
        }

        public void Execute(in AttackCommand command) => BeginAttack();
    }
}
