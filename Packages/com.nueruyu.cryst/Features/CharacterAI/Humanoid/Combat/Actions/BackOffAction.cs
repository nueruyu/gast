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

        public float Duration => duration;
    }

    public class BackOffAction : IAction<ActorContext<CombatState>, CombatState>
    {
        readonly BackOffActionSettings settings;

        public BackOffAction(BackOffActionSettings settings = null)
        {
            this.settings = settings ?? new BackOffActionSettings();
        }

        public bool IsAvailable(CombatState worldState)
        {
            return true;
        }

        public void Simulate(CombatState worldState)
        {
            worldState.SetDistanceToTarget(worldState.DistanceToTarget + 2);
        }

        public async UniTask ExecuteAsync(
            ActorContext<CombatState> context,
            CancellationToken cancellationToken)
        {
            var maneuver = new BackOffManeuver();

            await context.Actor.ExecuteManeuverAsync(
                maneuver,
                static state => state.TargetPosition,
                static state => state.TargetForward,
                context.WorldState,
                settings.Duration,
                cancellationToken);
        }
    }
}