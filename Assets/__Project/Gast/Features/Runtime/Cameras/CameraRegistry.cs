using UnityEngine;

namespace Gast.Features.Cameras
{
    public class CameraRegistry : MonoBehaviour
    {
        [SerializeField]
        Camera mainCamera;

        [SerializeField]
        ThirdPersonCamera cameraController;

        public Camera MainCamera => mainCamera;
        public ThirdPersonCamera CameraController => cameraController;
    }
}