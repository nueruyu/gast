using UnityEngine;

namespace DescrioGames.Domain.Cameras
{
    public interface ICamera
    {
        Vector3 Position { get; }
        Quaternion Rotation { get; }
    }
}