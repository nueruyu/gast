using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.Inputs;
using UnityEngine;
using UnityEngine.InputSystem; // Added

namespace Gast.UI.System
{
    public class CursorController : ILifecycleTask
    {
        readonly IInputModeManager inputModeManager;

        public CursorController(IInputModeManager inputModeManager)
        {
            this.inputModeManager = inputModeManager;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                UpdateCursorState();
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        void UpdateCursorState()
        {
            var currentMode = inputModeManager.CurrentMode.Value;
            var isAltPressed = Keyboard.current != null && Keyboard.current.leftAltKey.isPressed;

            // Show cursor if we are in UI mode OR if Alt is held down
            bool shouldShowCursor = currentMode == InputMode.UI || isAltPressed;

            SetCursorVisibility(shouldShowCursor);
        }

        void SetCursorVisibility(bool visible)
        {
            var targetLockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
            
            if (Cursor.lockState != targetLockState)
            {
                Cursor.lockState = targetLockState;
                Cursor.visible = visible;
            }
        }
    }
}