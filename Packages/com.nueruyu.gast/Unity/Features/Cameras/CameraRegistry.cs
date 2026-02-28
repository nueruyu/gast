using UnityEngine;

namespace Gast.Unity.Features.Cameras
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