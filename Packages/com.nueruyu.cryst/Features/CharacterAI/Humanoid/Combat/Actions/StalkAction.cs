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
    public class StalkActionSettings
    {
        [SerializeField]
        FloatRange duration = new(0.2f, 0.6f);

        [SerializeField]
        StalkManeuverSettings maneuver = new();

        public FloatRange Duration => duration;
        public StalkManeuverSettings Maneuver => maneuver;
    }

    /// <summary>
    ///     An action where the AI moves around the target for a short period of time to time an attack.
    /// </summary>
    public class StalkAction : IAction<ActorContext<CombatState>, CombatState>
    {
        readonly StalkActionSettings settings;

        public StalkAction(StalkActionSettings settings = null)
        {
            this.settings = settings ?? new StalkActionSettings();
        }

        public bool CanExecute(CombatState worldState)
        {
            return worldState.HasTarget;
        }

        public void Simulate(CombatState worldState)
        {
        }

        public async UniTask ExecuteAsync(
            ActorContext<CombatState> context,
            CancellationToken cancellationToken)
        {
            var direction = Random.value > 0.5f ? ManeuverDirection.Left : ManeuverDirection.Right;
            var duration = settings.Duration.Sample();
            var maneuver = new StalkManeuver(direction, settings.Maneuver, context.WorldState.AttackRange);

            await context.Actor.ExecuteManeuverAsync(
                maneuver,
                static state => state.TargetPosition,
                static state => state.TargetForward,
                context.WorldState,
                duration,
                cancellationToken);
        }
    }
}