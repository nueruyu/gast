using System;
using Cryst.Domain.Characters.Commands;
using Cryst.Features.CharacterActions.Actions.Attack;
using Gast.Unity.Features.Characters;

namespace Cryst.Features.CharacterActions.Actions.HeavyAttack
{
    public class HeavyAttackAction : AttackActionBase, ICharacterExecutableAction<HeavyAttackCommand>
    {
        public override Type CommandType => typeof(HeavyAttackCommand);

        public HeavyAttackAction(
            HeavyAttackActionSettings settings,
            CharacterAnimator animator,
            CharacterMovement movement,
            CharacterMovementSettings movementSettings,
            CharacterActionEffectDispatcher effectDispatcher)
            : base(settings, animator, movement, movementSettings, effectDispatcher)
        {
        }

        public void Execute(in HeavyAttackCommand command) => BeginAttack();
    }
}
