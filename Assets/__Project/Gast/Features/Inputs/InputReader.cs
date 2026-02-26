using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Gast.Core.Tasks;
using Gast.Domain.Inputs;
using Gast.Core.Observables;
using Gast.Shared.UnityExtensions;

namespace Gast.Features.Inputs
{
    /// <summary>
    /// Reads input from the Input System and provides a clean interface for gameplay code.
    /// Pure C# class with dependency injection.
    /// </summary>
    public class InputReader : IInputProvider, ILifecycleTask
    {
        readonly IInputModeManager inputModeManager;

        readonly InputActionMap playerActionMap;
        readonly InputActionMap menuActionMap;

        readonly InputAction moveAction;
        readonly InputAction lookAction;
        readonly InputAction jumpAction;
        readonly InputAction sprintAction;
        readonly InputAction interactAction;
        readonly InputAction attackAction;
        readonly InputAction guardAction;
        readonly InputAction showMenuAction;
        readonly InputAction hideMenuAction;

        readonly Signal showMenu = new();
        readonly Signal hideMenu = new();

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Jump { get; private set; }
        public bool Sprint { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool InteractHeld { get; private set; }
        public bool Attack { get; private set; }
        public bool Dash { get; private set; }
        public bool GuardHeld { get; private set; }
        public bool IsCursorOverridePressed { get; private set; }
        public ISignal ShowMenu => showMenu;
        public ISignal HideMenu => hideMenu;

        public InputReader(
            IInputModeManager inputModeManager,
            InputSettings settings)
        {
            this.inputModeManager = inputModeManager;

            playerActionMap = settings.InputActions.FindActionMap("Player");
            menuActionMap = settings.InputActions.FindActionMap("Menu");

            moveAction = playerActionMap.FindAction("Move");
            lookAction = playerActionMap.FindAction("Look");
            jumpAction = playerActionMap.FindAction("Jump");
            sprintAction = playerActionMap.FindAction("Sprint");
            interactAction = playerActionMap.FindAction("Interact");
            attackAction = playerActionMap.FindAction("Attack");
            guardAction = playerActionMap.FindAction("Guard");
            showMenuAction = playerActionMap.FindAction("ShowMenu");
            hideMenuAction = menuActionMap.FindAction("HideMenu");
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            inputModeManager.CurrentMode.SubscribeWithCurrent(mode =>
            {
                playerActionMap.SetEnabled(mode == InputMode.Gameplay);
                menuActionMap.SetEnabled(mode == InputMode.UI);
            }).AddTo(cancellationToken);

            jumpAction.SubscribePerformed(OnJumpPerformed).AddTo(cancellationToken);
            interactAction.SubscribePerformed(OnInteractPerformed).AddTo(cancellationToken);
            interactAction.SubscribeCanceled(OnInteractCanceled).AddTo(cancellationToken);
            attackAction.SubscribePerformed(OnAttackPerformed).AddTo(cancellationToken);
            sprintAction.SubscribePerformed(OnSprintPerformed).AddTo(cancellationToken);
            showMenuAction.SubscribePerformed(OnShowMenuPerformed).AddTo(cancellationToken);
            hideMenuAction.SubscribePerformed(OnHideMenuPerformed).AddTo(cancellationToken);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Move = moveAction.ReadValue<Vector2>();
                    Look = lookAction.ReadValue<Vector2>();
                    Sprint = sprintAction.IsPressed();
                    InteractHeld = interactAction.IsPressed();
                    GuardHeld = guardAction.IsPressed();
                    IsCursorOverridePressed = Keyboard.current != null && Keyboard.current.leftAltKey.isPressed;

                    // Reset flags at end of frame
                    await UniTask.WaitForEndOfFrame(cancellationToken);

                    Jump = false;
                    InteractPressed = false;
                    Attack = false;
                    Dash = false;

                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                }
            }
            finally
            {
                playerActionMap.Disable();
                menuActionMap.Disable();
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

        void OnShowMenuPerformed(InputAction.CallbackContext _)
        {
            showMenu.Publish();
        }

        void OnHideMenuPerformed(InputAction.CallbackContext _)
        {
            hideMenu.Publish();
        }
    }
}