using Gast.Domain.Cameras;
using Gast.Domain.Characters;
using Gast.Features.Characters;
using UnityEngine;

namespace Gast.Features.Cameras
{
    public class CameraService : ICameraService
    {
        readonly CameraRegistry cameraRegistry;
        readonly ICharacterActorRepository characterActorRepository;

        public CameraService(CameraRegistry cameraRegistry, ICharacterActorRepository characterActorRepository)
        {
            this.cameraRegistry = cameraRegistry;
            this.characterActorRepository = characterActorRepository;

            MainCamera = new CameraInfo(cameraRegistry.MainCamera);
        }

        public ICamera MainCamera { get; }

        public void SetFollowTarget(CharacterId targetCharacterId)
        {
            var character = characterActorRepository.Get(targetCharacterId);
            cameraRegistry.CameraController.SetFollowTarget(character.transform);
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