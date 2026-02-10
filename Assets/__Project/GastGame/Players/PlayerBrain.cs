using Cysharp.Threading.Tasks;
using Gast.Domain.Cameras;
using Gast.Domain.Characters;
using Gast.Domain.Inputs;
using GastGame.Actors;
using System;
using System.Threading;
using UnityEngine;

namespace GastGame.Players
{
    public class PlayerBrain : ICharacterBrain
    {
        readonly IInputProvider inputProvider;
        readonly ICameraService cameraService;
        CancellationTokenSource cts;

        public PlayerBrain(IInputProvider inputProvider, ICameraService cameraService)
        {
            this.inputProvider = inputProvider;
            this.cameraService = cameraService;
        }

        public void OnAttached(ICharacter character)
        {
            if (character is null)
                throw new ArgumentNullException(nameof(character));

            if (cts != null)
                throw new InvalidOperationException();

            cts = new();
            var actor = new Actor(character);
            RunAsync(actor, cts.Token).Forget();
        }

        public void OnDetached()
        {
            cts?.Cancel();
            cts = null;
        }

        async UniTask RunAsync(Actor actor, CancellationToken cancellationToken)
        {
            while (true)
            {
                await UniTask.NextFrame(cancellationToken);

                var inputMove = inputProvider.Move;

                var moveDirection = CalculateMoveDirection(inputMove);

                actor.SetSprint(inputProvider.Sprint);

                if (moveDirection.magnitude > 0.1f)
                {
                    actor.Move(moveDirection);
                }

                if (inputProvider.Jump)
                {
                    actor.Jump();
                }

                if (inputProvider.Dash)
                {
                    var moveDir = CalculateMoveDirection(inputMove);
                    if (moveDir.sqrMagnitude < 0.01f)
                    {
                        moveDir = actor.Body.Forward;
                    }
                    actor.Dash(moveDir);
                }

                if (inputProvider.GuardHeld)
                {
                    actor.StartGuard();
                }
                else
                {
                    actor.StopGuard();
                }

                if (inputProvider.Attack)
                {
                    actor.Attack();
                }
            }
        }

        Vector3 CalculateMoveDirection(Vector2 input)
        {
            var cameraRotation = cameraService.MainCamera.Rotation;

            var forward = cameraRotation * Vector3.forward;
            var right = cameraRotation * Vector3.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            return forward * input.y + right * input.x;
        }
    }
}
