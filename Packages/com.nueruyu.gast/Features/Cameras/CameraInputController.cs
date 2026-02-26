using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.Inputs;
using UnityEngine;

namespace Gast.Features.Cameras
{
    public class CameraInputController : ILifecycleTask
    {
        readonly IInputProvider inputProvider;
        readonly CameraRegistry cameraRegistry;

        public CameraInputController(IInputProvider inputProvider, CameraRegistry cameraRegistry)
        {
            this.inputProvider = inputProvider;
            this.cameraRegistry = cameraRegistry;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    var lookDelta = inputProvider.Look;
                    if (lookDelta.sqrMagnitude > 0.01f)
                    {
                        cameraRegistry.CameraController.Rotate(lookDelta);
                    }
                }

                await UniTask.Yield(cancellationToken);
            }
        }
    }
}
