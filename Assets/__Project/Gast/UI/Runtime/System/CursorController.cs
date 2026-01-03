using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.Inputs;
using UnityEngine;

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
            inputModeManager.CurrentMode.SubscribeWithCurrent(mode =>
            {
                switch (mode)
                {
                    case InputMode.Gameplay:
                        SetCursorState(true);
                        break;

                    default:
                        SetCursorState(false);
                        break;
                }
            }).AddTo(cancellationToken);

            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        void SetCursorState(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}