using UnityEngine;

namespace Gast.Unity.Features.HitDetection
{
    public interface IHitAreaFactory
    {
        void Create<TContext>(Pose pose, Vector3 size, float duration, TContext context);
    }
}
