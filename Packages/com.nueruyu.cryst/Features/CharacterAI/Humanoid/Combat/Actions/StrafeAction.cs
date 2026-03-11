using System;
using System.Threading;
using Cryst.Features.CharacterAI.Common;
using Cysharp.Threading.Tasks;
using Gast.Core.Values;
using Gast.Lib.AI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cryst.Features.CharacterAI.Humanoid.Combat.Actions
{
    [Serializable]
    public class StrafeActionSettings
    {
        [SerializeField] FloatRange duration = new(0.5f, 2.5f);
        [SerializeField] StrafeManeuverSettings maneuver = new();

        public FloatRange Duration => duration;
        public StrafeManeuverSettings Maneuver => maneuver;
    }

    public class StrafeAction : IAction<ActorContext<CombatState>, CombatState>
    {
        readonly StrafeActionSettings settings;

        public StrafeAction(StrafeActionSettings settings = null)
        {
            this.settings = settings ?? new StrafeActionSettings();
        }

        public bool CanExecute(CombatState worldState)
        {
            return worldState.HasTarget && worldState.IsInCombatRange;
        }

        public void Simulate(CombatState worldState)
        {
        }

        public async UniTask ExecuteAsync(ActorContext<CombatState> context, CancellationToken cancellationToken)
        {
            var direction = Random.value > 0.5f ? ManeuverDirection.Left : ManeuverDirection.Right;
            var duration = settings.Duration.Sample();
            var worldState = context.WorldState;
            var maneuver = new StrafeManeuver(direction, settings.Maneuver, worldState.CombatRange);

            await context.Actor.ExecuteManeuverAsync(
                maneuver,
                static state => state.worldState.TargetPosition,
                static state => state.worldState.TargetForward,
                (context.Actor, worldState),
                duration,
                cancellationToken);
        }
    }
}
