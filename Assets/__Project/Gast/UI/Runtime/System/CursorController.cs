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
        readonly IInputProvider inputProvider;
        bool isCursorLocked = true;

        public CursorController(IInputProvider inputProvider)
        {
            this.inputProvider = inputProvider;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            SetCursorState(true);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (inputProvider.MenuToggle)
                {
                    ToggleCursorState();
                }
                await UniTask.Yield(cancellationToken);
            }
        }

        void ToggleCursorState()
        {
            isCursorLocked = !isCursorLocked;
            SetCursorState(isCursorLocked);
        }

        void SetCursorState(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
