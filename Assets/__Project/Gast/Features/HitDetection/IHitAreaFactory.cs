using UnityEngine;

namespace Gast.Features.HitDetection
{
    public interface IHitAreaFactory
    {
        void Create<TContext>(Pose pose, Vector3 size, float duration, TContext context);
    }
}
