using Gast.Domain.Cameras;
using Gast.Domain.Characters;
using Gast.Domain.Inputs;
using Gast.Domain.Players;
using Cryst.Domain.Characters;
using UnityEngine;

namespace Cryst.Modules.Players
{
    public class PlayerCharacterController : IPlayerCharacterController
    {
        readonly IInputProvider inputProvider;
        readonly ICameraService cameraService;

        public PlayerCharacterController(IInputProvider inputProvider, ICameraService cameraService)
        {
            this.inputProvider = inputProvider;
            this.cameraService = cameraService;
        }

        public void HandleInput(ICharacter character)
        {
            var actor = character.As<ICrystCharacter>();

            var inputMove = inputProvider.Move;
            var moveDirection = CalculateMoveDirection(inputMove, cameraService, actor);

            actor.SetSprint(inputProvider.Sprint);
            actor.Move(moveDirection);

            if (inputProvider.Jump)
            {
                actor.Jump();
            }

            if (inputProvider.Dash)
            {
                var dashDir = moveDirection.sqrMagnitude < 0.01f ? actor.Body.Forward : moveDirection;
                actor.Dash(dashDir);
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

        Vector3 CalculateMoveDirection(Vector2 input, ICameraService cameraService, ICrystCharacter actor)
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