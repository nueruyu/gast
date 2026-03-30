using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Observables;
using Gast.Core.Tasks;
using Gast.Domain.Inputs;
using Gast.Unity.Shared.UnityExtensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Gast.Unity.Features.Inputs
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
        readonly InputAction heavyAttackAction;
        readonly InputAction grappleAction;
        readonly InputAction guardAction;
        readonly InputAction showMenuAction;
        readonly InputAction hideMenuAction;

        readonly Signal showMenu = new();
        readonly Signal hideMenu = new();
        readonly Signal<int> useItemSlot = new();

        Keyboard cachedKeyboard;
        KeyControl[] cachedDigitKeys;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Jump { get; private set; }
        public bool Sprint { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool InteractHeld { get; private set; }
        public bool Attack { get; private set; }
        public bool HeavyAttack { get; private set; }
        public bool Grapple { get; private set; }
        public bool Dash { get; private set; }
        public bool GuardHeld { get; private set; }
        public bool IsCursorOverridePressed { get; private set; }
        public ISignal ShowMenu => showMenu;
        public ISignal HideMenu => hideMenu;
        public ISignal<int> UseItemSlot => useItemSlot;

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
            heavyAttackAction = playerActionMap.FindAction("HeavyAttack");
            grappleAction = playerActionMap.FindAction("Grapple");
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
            heavyAttackAction.SubscribePerformed(OnHeavyAttackPerformed).AddTo(cancellationToken);
            grappleAction.SubscribePerformed(OnGrapplePerformed).AddTo(cancellationToken);
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

                    CheckItemUsageInput();

                    // Reset flags at end of frame
                    await UniTask.WaitForEndOfFrame(cancellationToken);

                    Jump = false;
                    InteractPressed = false;
                    Attack = false;
                    HeavyAttack = false;
                    Grapple = false;
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

        void CheckItemUsageInput()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard != cachedKeyboard)
            {
                cachedKeyboard = keyboard;
                cachedDigitKeys = new[]
                {
                    keyboard.digit1Key, keyboard.digit2Key, keyboard.digit3Key,
                    keyboard.digit4Key, keyboard.digit5Key, keyboard.digit6Key,
                    keyboard.digit7Key, keyboard.digit8Key, keyboard.digit9Key,
                    keyboard.digit0Key
                };
            }

            for (var i = 0; i < cachedDigitKeys.Length; i++)
            {
                if (cachedDigitKeys[i].wasPressedThisFrame)
                {
                    useItemSlot.Publish(i);
                    return;
                }
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

        void OnHeavyAttackPerformed(InputAction.CallbackContext _)
        {
            HeavyAttack = true;
        }

        void OnGrapplePerformed(InputAction.CallbackContext _)
        {
            Grapple = true;
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