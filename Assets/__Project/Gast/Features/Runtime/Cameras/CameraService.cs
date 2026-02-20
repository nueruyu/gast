using Gast.Domain.Cameras;
using Gast.Domain.Characters;
using Gast.Features.Characters;
using UnityEngine;

namespace Gast.Features.Cameras
{
    public class CameraService : ICameraService
    {
        readonly CameraRegistry cameraRegistry;
        readonly ICharacterRepository characterRepository;

        public CameraService(CameraRegistry cameraRegistry, ICharacterRepository characterRepository)
        {
            this.cameraRegistry = cameraRegistry;
            this.characterRepository = characterRepository;

            MainCamera = new CameraInfo(cameraRegistry.MainCamera);
        }

        public ICamera MainCamera { get; }

        public void SetFollowTarget(CharacterId targetCharacterId)
        {
            var character = characterRepository.Get(targetCharacterId);
            var focusTarget = character.Resolve<CameraFocusTarget>();
            cameraRegistry.CameraController.SetFollowTarget(focusTarget.transform);
        }

        public void UnsetFollowTarget()
        {
            cameraRegistry.CameraController.SetFollowTarget(null);
        }

        class CameraInfo : ICamera
        {
            readonly Camera camera;

            public CameraInfo(Camera camera)
            {
                this.camera = camera;
            }

            public Vector3 Position => camera.transform.position;

            public Quaternion Rotation => camera.transform.rotation;
        }
    }
}