using Gast.Domain.Cameras;
using Gast.Domain.Characters;
using Gast.Domain.Inputs;
using Gast.Domain.Players;
using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Facets;
using UnityEngine;

namespace Cryst.Features.Players
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
            var actor = character.As<BaseCharacter>();

            var inputMove = inputProvider.Move;
            var moveDirection = CalculateMoveDirection(inputMove, cameraService, actor);

            if (character.Is(out SprintableCharacter sprintable))
            {
                sprintable.SetSprint(inputProvider.Sprint);
            }
            actor.Move(moveDirection);

            if (inputProvider.Jump)
            {
                if (character.Is(out JumpableCharacter jumpable))
                {
                    jumpable.Jump();
                }
            }

            if (inputProvider.Dash)
            {
                if (character.Is(out DashableCharacter dashable))
                {
                    var dashDir = moveDirection.sqrMagnitude < 0.01f ? actor.Body.Forward : moveDirection;
                    dashable.Dash(dashDir);
                }
            }

            if (character.Is(out GuardableCharacter guardable))
            {
                if (inputProvider.GuardHeld)
                {
                    guardable.StartGuard();
                }
                else
                {
                    guardable.StopGuard();
                }
            }

            if (inputProvider.Attack)
            {
                if (character.Is(out AttackableCharacter attackable))
                {
                    attackable.Attack();
                }
            }

            if (inputProvider.HeavyAttack)
            {
                if (character.Is(out AttackableCharacter attackable))
                {
                    attackable.HeavyAttack();
                }
            }

            if (inputProvider.Grapple)
            {
                if (character.Is(out GrappleableCharacter grappleable))
                {
                    grappleable.Grapple();
                }
            }
        }

        Vector3 CalculateMoveDirection(Vector2 input, ICameraService cameraService, BaseCharacter actor)
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
