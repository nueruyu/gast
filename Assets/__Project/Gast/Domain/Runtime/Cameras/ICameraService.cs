using DescrioGames.Domain.Characters;

namespace DescrioGames.Domain.Cameras
{
    public interface ICameraService
    {
        ICamera MainCamera { get; }

        void SetFollowTarget(CharacterId targetCharacterId);

        void UnsetFollowTarget();
    }
}