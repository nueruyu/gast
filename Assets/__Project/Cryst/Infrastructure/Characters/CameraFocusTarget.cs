using Gast.Features.Cameras;
using UnityEngine;

namespace Cryst.Infrastructure.Characters
{
    class CameraFocusTarget : ICameraFocusTarget
    {
        public CameraFocusTarget(Transform focusTransform)
        {
            FocusTransform = focusTransform;
        }

        public Transform FocusTransform { get; }
    }
}