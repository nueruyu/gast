using UnityEngine;

namespace Gast.Features.Combat
{
    public interface IHitAreaFactory
    {
        void Create(Pose pose, Vector3 size, float duration, object context);
    }
}
