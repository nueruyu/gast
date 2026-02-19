using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Events;
using Gast.Core.Tasks;
using Gast.Domain.Characters;
using Gast.Domain.Combat;
using Cryst.Domain.Characters;
using R3;
using Cryst.Domain.Combat;
using Gast.Shared.Phantoms;

namespace Cryst.Infrastructure.EventHandlers
{
    public class HitEffectHandler : ILifecycleTask
    {
        readonly IDomainEventSubscriber eventSubscriber;
        readonly ICharacterRepository characterRepository;

        public HitEffectHandler(
            IDomainEventSubscriber eventSubscriber,
            ICharacterRepository characterRepository)
        {
            this.eventSubscriber = eventSubscriber;
            this.characterRepository = characterRepository;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            eventSubscriber.Subscribe<CharacterHitEvent>(OnCharacterHit)
                .AddTo(cancellationToken);
            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        void OnCharacterHit(CharacterHitEvent e)
        {
            if (e.Context is not Phantom context)
                return;

            if (!context.TryGet(AttackContextKeys.SourceCharacterId, out var attackerId))
                return;

            var hitActor = e.HitCharacter.As<ICrystCharacter>();
            var attacker = characterRepository.Get(attackerId).As<ICrystCharacter>();

            if (hitActor.Faction == attacker.Faction)
                return;

            var damage = context.Get(AttackContextKeys.Damage);
            var knockback = context.Get(AttackContextKeys.KnockbackForce);

            var damageInfo = new DamageInfo(
                damage,
                e.HitPoint,
                knockback,
                attackerId
            );

            hitActor.TakeDamage(damageInfo);
        }
    }
}