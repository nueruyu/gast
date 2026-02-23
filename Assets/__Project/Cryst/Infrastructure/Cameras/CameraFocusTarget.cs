using Gast.Domain.Economy;
using Gast.Features.Cameras;
using UnityEngine;

namespace Cryst.Infrastructure.Cameras
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