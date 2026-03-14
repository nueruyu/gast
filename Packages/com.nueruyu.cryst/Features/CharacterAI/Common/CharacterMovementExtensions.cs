using System;
using System.Threading;
using Cryst.Domain.Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Common
{
    public static class CharacterMovementExtensions
    {
        public static async UniTask MoveToAsync<TState>(
            this BaseCharacter actor,
            Func<TState, Vector3> destinationProvider,
            Func<TState, bool> until,
            TState state,
            CancellationToken cancellationToken)
        {
            var navigator = actor.NavigationProvider;
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (until(state))
                        return;

                    navigator.SetDestination(destinationProvider(state));
                    var direction = navigator.NextSteeringDirection;
                    if (direction != Vector3.zero)
                        actor.Move(direction);

                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

                    if (navigator.HasArrived)
                        return;
                }
            }
            finally
            {
                navigator.Stop();
                actor.Move(Vector3.zero);
            }
        }

        public static async UniTask FaceTowardsAsync<TState>(
            this BaseCharacter actor,
            Func<TState, Vector3> targetPositionProvider,
            TState state,
            float angleThreshold,
            float timeout,
            CancellationToken cancellationToken)
        {
            var navigator = actor.NavigationProvider;
            var timer = 0f;
            try
            {
                while (timer < timeout && !cancellationToken.IsCancellationRequested)
                {
                    var targetPos = targetPositionProvider(state);
                    var toTarget = targetPos - actor.Body.Position;
                    toTarget.y = 0;

                    if (toTarget.sqrMagnitude < 0.01f ||
                        Vector3.Angle(actor.Body.Forward, toTarget.normalized) <= angleThreshold)
                        return;

                    navigator.SetDestination(actor.Body.Position + toTarget.normalized);
                    actor.Move(navigator.NextSteeringDirection);

                    timer += Time.deltaTime;
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }
            }
            finally
            {
                navigator.Stop();
                actor.Move(Vector3.zero);
            }
        }

        public static async UniTask ExecuteManeuverAsync<TState, TManeuver>(
            this BaseCharacter actor,
            TManeuver maneuver,
            Func<TState, Vector3> targetPositionProvider,
            Func<TState, Vector3> targetForwardProvider,
            TState state,
            float duration,
            CancellationToken cancellationToken)
            where TManeuver : struct, IManeuverStrategy
        {
            var navigator = actor.NavigationProvider;
            var timer = 0f;
            try
            {
                while (timer < duration && !cancellationToken.IsCancellationRequested)
                {
                    var targetPosition = targetPositionProvider(state);
                    var targetForward = targetForwardProvider(state);
                    var moveDir = maneuver.CalculateMoveDirection(actor, targetPosition, targetForward);

                    navigator.SetDestination(actor.Body.Position + moveDir);
                    actor.Move(navigator.NextSteeringDirection);

                    timer += Time.deltaTime;
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }
            }
            finally
            {
                navigator.Stop();
                actor.Move(Vector3.zero);
            }
        }
    }
}