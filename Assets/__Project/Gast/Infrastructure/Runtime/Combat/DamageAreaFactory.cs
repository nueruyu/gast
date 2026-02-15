using Gast.Core.Events;
using Gast.Domain.Combat;
using Gast.Features.Combat;
using UnityEngine;

namespace Gast.Infrastructure.Combat
{
    public class DamageAreaFactory
    {
        readonly DamageAreaSettings settings;
        readonly IDomainEventPublisher eventPublisher;

        public DamageAreaFactory(DamageAreaSettings settings, IDomainEventPublisher eventPublisher)
        {
            this.settings = settings;
            this.eventPublisher = eventPublisher;
        }

        public void Create(
            Pose pose,
            Vector3 size,
            float duration,
            AttackInfo attackInfo)
        {
            var damageArea = Object.Instantiate(
                settings.DamageAreaPrefab,
                pose.position,
                pose.rotation);

            damageArea.transform.localScale = size;

            damageArea.Initialize(
                attackInfo,
                duration,
                eventPublisher);
        }
    }
}
