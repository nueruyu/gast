using System;
using Cryst.Domain.Characters.Commands;
using Cryst.Features.CharacterActions.Actions.Attack;
using Gast.Unity.Features.Characters;

namespace Cryst.Features.CharacterActions.Actions.Grapple
{
    /// <summary>
    /// Grapple initiation action. Spawns a grapple detection area via
    /// SpawnGrappleAreaEffect when the animation event fires.
    /// On hit, GrappleHitHandler transitions attacker to GrappleThrowAction
    /// and victim to GrappledAction.
    /// </summary>
    public class GrappleAction : AttackActionBase, ICharacterExecutableAction<GrappleCommand>
    {
        public override Type CommandType => typeof(GrappleCommand);

        public GrappleAction(
            GrappleActionSettings settings,
            CharacterAnimator animator,
            CharacterMovement movement,
            CharacterMovementSettings movementSettings,
            CharacterActionEffectDispatcher effectDispatcher)
            : base(settings, animator, movement, movementSettings, effectDispatcher)
        {
        }

        public void Execute(in GrappleCommand command) => BeginAttack();
    }
}
