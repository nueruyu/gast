using Gast.Core.Events;
using Gast.Domain.Combat;
using Gast.Features.Combat;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gast.Infrastructure.Combat
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
            IEffect effect)
        {
            var damageArea = Object.Instantiate(
                settings.HitAreaPrefab,
                pose.position,
                pose.rotation);

            damageArea.transform.localScale = size;

            damageArea.Initialize(
                effect,
                duration,
                eventPublisher);
        }
    }
}