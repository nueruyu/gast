using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Gast.Core.Tasks;
using Gast.Domain.Inputs;

namespace Gast.Features.Inputs
{
    /// <summary>
    /// Reads input from the Input System and provides a clean interface for gameplay code.
    /// Pure C# class with dependency injection.
    /// </summary>
    public class InputReader : IInputProvider, ILifecycleTask
    {
        readonly InputSettings settings;

        readonly InputActionMap playerActionMap;
        readonly InputAction moveAction;
        readonly InputAction lookAction;
        readonly InputAction jumpAction;
        readonly InputAction sprintAction;
        readonly InputAction interactAction;
        readonly InputAction attackAction;
        readonly InputAction guardAction;
        readonly InputAction menuToggleAction;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Jump { get; private set; }
        public bool Sprint { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool InteractHeld { get; private set; }
        public bool Attack { get; private set; }
        public bool Dash { get; private set; }
        public bool GuardHeld { get; private set; }
        public bool MenuToggle { get; private set; }

        public InputReader(InputSettings settings)
        {
            this.settings = settings;

            playerActionMap = settings.InputActions.FindActionMap("Player");

            moveAction = playerActionMap.FindAction("Move");
            lookAction = playerActionMap.FindAction("Look");
            jumpAction = playerActionMap.FindAction("Jump");
            sprintAction = playerActionMap.FindAction("Sprint");
            interactAction = playerActionMap.FindAction("Interact");
            attackAction = playerActionMap.FindAction("Attack");
            guardAction = playerActionMap.FindAction("Guard");
            menuToggleAction = playerActionMap.FindAction("MenuToggle");
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            playerActionMap.Enable();

            jumpAction.performed += OnJumpPerformed;
            interactAction.performed += OnInteractPerformed;
            interactAction.canceled += OnInteractCanceled;
            attackAction.performed += OnAttackPerformed;
            sprintAction.performed += OnSprintPerformed;
            menuToggleAction.performed += OnMenuTogglePerformed;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Move = moveAction.ReadValue<Vector2>();
                    Look = lookAction.ReadValue<Vector2>();
                    Sprint = sprintAction.IsPressed();
                    InteractHeld = interactAction.IsPressed();
                    GuardHeld = guardAction.IsPressed();

                    // Reset flags at end of frame
                    await UniTask.WaitForEndOfFrame(cancellationToken);

                    Jump = false;
                    InteractPressed = false;
                    Attack = false;
                    Dash = false;
                    MenuToggle = false;

                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }
            }
            finally
            {
                playerActionMap.Disable();
                jumpAction.performed -= OnJumpPerformed;
                interactAction.performed -= OnInteractPerformed;
                interactAction.canceled -= OnInteractCanceled;
                attackAction.performed -= OnAttackPerformed;
                sprintAction.performed -= OnSprintPerformed;
                menuToggleAction.performed -= OnMenuTogglePerformed;
            }
        }

        void OnJumpPerformed(InputAction.CallbackContext _)
        {
            Jump = true;
        }

        void OnInteractPerformed(InputAction.CallbackContext _)
        {
            InteractPressed = true;
        }

        void OnInteractCanceled(InputAction.CallbackContext _)
        {
            InteractHeld = false;
        }

        void OnAttackPerformed(InputAction.CallbackContext _)
        {
            Attack = true;
        }

        void OnSprintPerformed(InputAction.CallbackContext context)
        {
            // Sprint action with Tap interaction triggers Dash
            if (context.interaction is UnityEngine.InputSystem.Interactions.TapInteraction)
            {
                Dash = true;
            }
        }

        void OnMenuTogglePerformed(InputAction.CallbackContext _)
        {
            MenuToggle = true;
        }
    }
}