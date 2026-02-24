using Gast.Core.Events;
using Gast.Features.HitDetection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gast.Infrastructure.HitDetection
{
    public class HitAreaFactory : IHitAreaFactory
    {
        readonly HitAreaSettings settings;
        readonly IDomainEventPublisher eventPublisher;

        public HitAreaFactory(HitAreaSettings settings, IDomainEventPublisher eventPublisher)
        {
            this.settings = settings;
            this.eventPublisher = eventPublisher;
        }

        public void Create(
            Pose pose,
            Vector3 size,
            float duration,
            object context)
        {
            var damageArea = Object.Instantiate(
                settings.HitAreaPrefab,
                pose.position,
                pose.rotation);

            damageArea.transform.localScale = size;

            damageArea.Initialize(
                context,
                duration,
                eventPublisher);
        }
    }
}
