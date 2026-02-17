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
            switch (e.Effect)
            {
                case IAttackEffect attackEffect:
                    OnAttackHit(e, attackEffect);
                    break;
            }
        }

        void OnAttackHit(CharacterHitEvent e, IAttackEffect effect)
        {
            var hitActor = e.HitCharacter.As<ICrystCharacter>();
            var attacker = characterRepository.Get(effect.SourceCharacterId).As<ICrystCharacter>();

            if (hitActor.Faction == attacker.Faction)
                return;

            var damageInfo = new DamageInfo(
                effect.Damage,
                e.HitPoint,
                effect.KnockbackForce,
                effect.SourceCharacterId
            );

            hitActor.TakeDamage(damageInfo);
        }
    }
}