using Gast.Domain.Characters;

namespace Gast.Domain.Cameras
{
    public interface ICameraService
    {
        ICamera MainCamera { get; }

        void SetFollowTarget(CharacterId targetCharacterId);

        void UnsetFollowTarget();
    }
}