using Gast.Core.Events;
using Gast.Domain.Characters;
using Gast.Unity.Features.HitDetection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gast.Unity.Infrastructure.HitDetection
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

        public void Create<TContext>(
            Pose pose,
            Vector3 size,
            float duration,
            TContext context)
        {
            var damageArea = Object.Instantiate(
                settings.HitAreaPrefab,
                pose.position,
                pose.rotation);

            damageArea.transform.localScale = size;

            damageArea.Initialize(
                duration,
                (character, hitPoint) =>
                {
                    eventPublisher.Publish(new CharacterHitEvent<TContext>(character, hitPoint, context));
                });
        }
    }
}
