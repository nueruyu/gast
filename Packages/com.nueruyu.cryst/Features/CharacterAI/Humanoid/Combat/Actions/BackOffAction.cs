using System;
using System.Threading;
using Cryst.Features.CharacterAI.Common;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Combat.Actions
{
    [Serializable]
    public class BackOffActionSettings
    {
        [SerializeField] float duration = 1.5f;
        [SerializeField] BackOffManeuverSettings maneuver = new();

        public float Duration => duration;
        public BackOffManeuverSettings Maneuver => maneuver;
    }

    public class BackOffAction : IAction<ActorContext<CombatState>, CombatState>
    {
        readonly BackOffActionSettings settings;

        public BackOffAction(BackOffActionSettings settings = null)
        {
            this.settings = settings ?? new BackOffActionSettings();
        }

        public bool CanExecute(CombatState worldState)
        {
            return worldState.IsInAttackRange && !worldState.IsReadyToAttack;
        }

        public void Simulate(CombatState worldState)
        {
            worldState.DistanceToTarget += 2.0f;
        }

        public async UniTask ExecuteAsync(ActorContext<CombatState> context, CancellationToken cancellationToken)
        {
            var actor = context.Actor;
            var worldState = context.WorldState;
            var maneuver = new BackOffManeuver(settings.Maneuver);

            await actor.ExecuteManeuverAsync(
                maneuver,
                static state => state.worldState.TargetPosition,
                static state => state.worldState.TargetForward,
                (actor, worldState),
                settings.Duration,
                cancellationToken);
        }
    }
}
