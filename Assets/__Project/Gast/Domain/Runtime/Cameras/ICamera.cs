using UnityEngine;

namespace Gast.Domain.Cameras
{
    public interface ICamera
    {
        Vector3 Position { get; }
        Quaternion Rotation { get; }
    }
}