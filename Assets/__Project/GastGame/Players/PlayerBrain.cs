using Cysharp.Threading.Tasks;
using Gast.Domain.Cameras;
using Gast.Domain.Characters;
using Gast.Domain.Inputs;
using GastGame.Actions.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GastGame.Players
{
    /// <summary>
    /// Player-controlled brain implementation.
    /// Converts input and camera state into camera-relative movement commands.
    /// </summary>
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
            RunAsync(character, cts.Token).Forget();
        }

        public void OnDetached()
        {
            cts?.Cancel();
            cts = null;
        }

        async UniTask RunAsync(ICharacter character, CancellationToken cancellationToken)
        {
            while (true)
            {
                await UniTask.NextFrame(cancellationToken);

                var inputMove = inputProvider.Move;

                // Convert input to camera-relative direction
                var moveDirection = CalculateMoveDirection(inputMove);

                // Set sprint state
                character.SetSprint(inputProvider.Sprint);

                // Move character
                if (moveDirection.magnitude > 0.1f)
                {
                    character.Move(moveDirection);
                }

                // Handle jump
                if (inputProvider.Jump)
                {
                    character.ActionController.ExecuteAction(new JumpCommand());
                }

                // Handle dash
                if (inputProvider.Dash)
                {
                    var moveDir = CalculateMoveDirection(inputMove);
                    if (moveDir.sqrMagnitude < 0.01f)
                    {
                        moveDir = character.Body.Forward;
                    }
                    character.ActionController.ExecuteAction(new DashCommand(moveDir));
                }

                // Handle guard
                if (inputProvider.GuardHeld)
                {
                    character.ActionController.StartAction(new GuardCommand());
                }
                else
                {
                    character.ActionController.StopAction<GuardCommand>();
                }

                // Handle attack
                if (inputProvider.Attack)
                {
                    character.ActionController.ExecuteAction(new AttackCommand());
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