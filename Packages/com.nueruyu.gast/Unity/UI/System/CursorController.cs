using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.Inputs;
using UnityEngine;

namespace Gast.Unity.UI.System
{
    public class CursorController : ILifecycleTask
    {
        readonly IInputModeManager inputModeManager;
        readonly IInputProvider inputProvider;

        public CursorController(IInputModeManager inputModeManager, IInputProvider inputProvider)
        {
            this.inputModeManager = inputModeManager;
            this.inputProvider = inputProvider;
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
            var isCursorOverridePressed = inputProvider.IsCursorOverridePressed;

            // Show cursor if we are in UI mode OR if the override key is held down
            bool shouldShowCursor = currentMode == InputMode.UI || isCursorOverridePressed;

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